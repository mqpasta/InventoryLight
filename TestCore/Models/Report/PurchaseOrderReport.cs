using System;
using System.Collections.Generic;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TestCore.Models.Reports
{
    public class PurchaseOrderReport
    {
        [DisplayName("Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime? StartDate { get; set; }

        [DisplayName("End Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime? EndDate { get; set; }

        [DisplayName("Location")]
        public long LocationId { get; set; }

        [DisplayName("Product")]
        public long ProductId { get; set; }

        [DisplayName("Received Orders")]
        public bool IsReceived { get; set; }

        [DisplayName("Orders with Balance Quantity")]
        public bool IsLessQuantity { get; set; }

        public List<PurchaseOrder> PurchaseOrders { get; set; }
    }
}
