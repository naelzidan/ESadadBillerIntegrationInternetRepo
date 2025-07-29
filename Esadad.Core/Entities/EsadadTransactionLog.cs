using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esadad.Core.Entities
{
    [Table("EsadadTransactionsLogs")]
    public class EsadadTransactionLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string TransactionType { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string ApiName { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string Guid { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime Timestamp { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string BillingNumber { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string BillNumber { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string Currency { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string ServiceType { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string PrepaidCat { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string ValidationCode { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string TranXmlElement { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime InsertDate { get; set; } = DateTime.Now;
    }
}
