using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TestCore.Models.IRepository;
using TestCore.Models.SqlRepository;

namespace TestCore.Models
{
    public class PurchaseOrder
    {
        IProductRepository _prodRep;

        public PurchaseOrder()
        {
            _prodRep = new SqlProductRepository();
        }

        [ReadOnly(true)]
        [Key]
        [DisplayName("PO")]
        public long? PurchaseOrderId { get; set; }

        [DisplayName("Date")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime PODate { get; set; }

        [DisplayName("Product")]
        [Required]
        public long ProductId { get; set; }

        public Product Product
        {
            get
            {
                return _prodRep.Find(ProductId);
            }
        }

        [DisplayName("Conv. Rate")]
        [Required]
        [DefaultValue(true)]
        public decimal ConvRate { get; set; }

        [DisplayName("RMB Price")]
        [Required]
        [DefaultValue(true)]
        public decimal RMBRate { get; set; }

        [DisplayName("PKR Price")]
        [ReadOnly(true)]
        public decimal PKRCost
        {
            get { return Math.Round(RMBRate * ConvRate, 2); }
        }

        [DisplayName("Amount PKR")]
        [ReadOnly(true)]
        public decimal PKRAmount
        {
            get { return Math.Round(PKRCost * Quantity, 2); }
        }

        [DisplayName("Amount RMB")]
        [ReadOnly(true)]
        public decimal RMBAmount
        {
            get
            {
                return Math.Round(RMBRate * Quantity, 4);
            }
        }

        [DisplayName("Qty")]
        [Required]
        [DefaultValue(true)]
        public int Quantity { get; set; }

        [DisplayName("Qty Rcvd")]
        [DefaultValue(true)]
        public int ReceivedQuantity { get; set; }

        [DisplayName("Balance")]
        [ReadOnly(true)]
        public int BalanceQuantity
        {
            get { return Quantity - ReceivedQuantity; }
        }

        [DisplayName("Received")]
        [Required]
        [DefaultValue(false)]
        public bool IsReceived { get; set; }

        [DisplayName("Cost Price")]
        public decimal CostPrice { get; set; }
    }
}
