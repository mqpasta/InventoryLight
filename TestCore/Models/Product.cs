using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestCore.Models
{
    public class Product
    {
        [HiddenInput(DisplayValue=false)]
        [Key]
        public long? ProductId { get; set; }

        [Required]
        [DisplayName("Product Code")]
        public Int16 ProductCode { get; set; }

        [Required]
        [MaxLength(20, ErrorMessage = "Product name can contain only 20 characters." )]
        [DisplayName("Product Name")]
        public string ProductName { get; set; }

        [DefaultValue(0.0)]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString ="{0:N2}")]
        [DisplayName("Pruchase Price")]
        public decimal PurchasePrice { get; set; }

        [DefaultValue(0.0)]
        [DisplayName("Sale Price")]
        public decimal SalePrice { get; set; }

        public string ProductCodeName
        {
            get
            {
                return string.Format("{0} - {1}", ProductCode, ProductName);
            }
        }

    }
}
