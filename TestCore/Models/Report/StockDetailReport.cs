using System;
using System.Data;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TestCore.Models.Reports
{
    public class StockDetailReport
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

        [DisplayName("Transaction Type")]
        public StockMovementType Type { get; set; }

        public DataTable Result { get; set; }


    }
}
