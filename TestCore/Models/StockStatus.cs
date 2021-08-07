using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TestCore.Models
{
    public class StockStatus
    {
        [DisplayName("Product Name")]
        public string ProductName { get; set; }

        [DisplayName("Location Name")]
        public string LocationName { get; set; }

        [DisplayName("In Quantity")]
        public int InQuantity { get; set; }

        [DisplayName("Out Quantity")]
        public int OutQuantity { get; set; }

        [DisplayName("Balance Quantity")]
        public int BalanceQuantity { get; set; }

    }
}
