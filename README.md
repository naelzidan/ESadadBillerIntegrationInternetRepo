E-Sadad Biller Integration

o	Separate Middleware that communicates with billing system through:
  	API
  	Direct read and write to database
  -	Implement Clean Architecture by separating the billing system requirements from the eSadad integration requirements.
  -	It is advisable to use API versioning	
o	Integration Technology (REST / XML), Query string parameters, URL Routing 
o	Create Table (Config File) for Biller Information
BillerCode, BillerName
o	Create Table  (Config File) for eSadad Services Configurations
  ServiceTypeCode (Characters, Numbers, Symbols), ServiceNameArabic, ServiceNameEnglish , PaymentType (PostPaid, PrePaid), Currency ( JOD,USD,ILS), BankCode (Integer), IBAN, PartialPayment (True, False), LowerValue , UpperValue 
o	Create Tabke (Config File) for Digital Signature Certificates 
  CertificateOwner(Esadad, Biller Name), CertificateType (Public, Private), CertificatePassword, CertficiatePath
o	Handle decimal Places appropriate for currencies (JOD:3, USD:2, ILS:2)
o	Create Log tables 
  -	EsadadTransactionsLogs (Id, Transaction type (Request, Response), API (BillPull, PrepaidValidation, ReceivePaymentNotification), Guid, Billing Number, Bill Number, Service type, Currency, XmlElement, Insert date time) // include any other fields as necessary.
  -	EsadadPaymetsLogs (Id, Guid, BillingNumber, BillNumber, ServiceType, Currency, PaidAmount, IsPaid (True, False)) // include any other fields as necessary.

BillPull:
1.	Get Inputs:
  o	guid from query
  o	xmlElement from body
  o	username from query (optional)
  o	password from query (optional)
2.	Log the request into TransactionLogs table
  Id, Transaction type (Request), API (BillPull), Guid, Billing Number, Bill Number, Service type, Currency, XmlElement, Insert date time
3.	Validate Inputs
4.	Verify Request Signature: False
  Return XML response for invalid signature
5.	Verify Request Signature is True: Continue
6.	Retrieve Billing Information from Billing System
  o	If not exists: Return Invalid Billing Number
  o	If exists with due amount: Generate Bill with due XML Response
  o	If exists and no due amount: Generate Bill without due amount XML Response
  o	General Exception: Return generate exception XML response
7.	Sign MsgBody in response and put the signature in the response
8.	Log the response into EsadadTransactionLogs table
   
    Id, Transaction type (Response), API (BillPull), Guid, Billing Number, Bill Number, Service type, Currency, XmlElement, Insert date time
9.	Return the response


RecivePaymentNotifications:
1.	Get Inputs:
  o	guid from query
  o	xmlElement from body
  o	username from query (optional)
  o	password from query (optional)
2.	Log the request into TransactionLogs table with:
  Id, Transaction type (Request, Response), API (ReceivePaymentNotification), Guid, Billing Number, Bill Number, Service type, Currency, XmlElement, Insert date time
3.	Log the required fields in EsadadPaymentLogs Table (Required for handling Api call retries):
  Id, Guid, BillingNumber, BillNumber, ServiceType, Currency, PaidAmount, IsPaid (False)
4.	Validate Inputs
5.	Verify Request Signature Is False
  Return XML response for invalid signature
6.	Verify Request Signature Is True: Continue
7.	Send a payment to Internal System (Billing System)
  o	Inputs (Payment Guid, ServiceType, BillingNumber, BillNumber, PaidAmount)
  o	Query and check EsadadPaymentLogs table by Guid and IsPaid is true

    o	Record does not exist:
  	
      -	Execute payment to Internal System (Billing system)
  	
      -	Update EsadadPaymentLogs IsPaid field to True for the most recent record with same Guid.
  	
      -	Generate RecivePaymentNotifications XML Response
  	
    o	Record exist: 
  
      -	Generate RecivePaymentNotifications XML Response
    
8.	Sign MsgBody in response and put the signature in the response
9.	Log the response into EsadadTransactionLogs table
    
    Id, Transaction type (Response), API (ReceivePaymentNotification), Guid, Billing Number, Bill Number, Service type, Currency, XmlElement, Insert date time
10.	Return the response


