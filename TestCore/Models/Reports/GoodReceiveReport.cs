using System;
using System.Collections.Generic;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TestCore.Models.IRepository;
using TestCore.Models.SqlRepository;
namespace TestCore.Models.Reports
{
    public class GoodReceiveReport
    {
        [DisplayName("Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime? StartDate { get; set; }

        [DisplayName("End Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime? EndDate { get; set; }

        [DisplayName("Location")]
        public long LocationId { get; set; }

        [DisplayName("Product")]
        public long ProductId { get; set; }

        public List<PurchaseMovement> PurchaseMovements { get; set; }
    }
}
