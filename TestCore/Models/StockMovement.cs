using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections;

using TestCore.Models.IRepository;
using TestCore.Models.FakeRepository;
using TestCore.Models.SqlRepository;

namespace TestCore.Models
{
    public enum StockMovementType
    {
        Purchase,
        Sale
    }

    public abstract class StockMovement
    {
        IProductRepository _prodRep;

        public StockMovement()
        {
            _prodRep = new SqlProductRepository();
        }

        public StockMovement(IProductRepository productRepository)
        {
            _prodRep = productRepository;
        }

        [HiddenInput(DisplayValue = false)]
        [Key]
        public long? StockMovementId { get; set; }

        [Required]
        [DisplayName("Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime Date { get; set; }

        [Required]
        [DisplayName("Product")]
        public long ProductId
        {
            get; set;
        }

        [Required]
        public int Quantity { get; set; }

        [HiddenInput(DisplayValue = false)]
        public StockMovementType? MovementType { get; set; }

        public Product Product { get { return _prodRep.Find(ProductId); } }

    }
}
