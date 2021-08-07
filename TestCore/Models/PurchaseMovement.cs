using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using TestCore.Models;
using TestCore.Models.IRepository;
using TestCore.Models.FakeRepository;
using TestCore.Models.SqlRepository;
using System.Collections;

namespace TestCore.Models
{
    public class PurchaseMovement : StockMovement
    {
        ILocationRepository _locRep;
        IProductRepository _prodRep;

        public PurchaseMovement()
        {
            _locRep = new SqlLocationRepository();
            _prodRep = new SqlProductRepository();
            this.Initialize();
        }

        public PurchaseMovement(ILocationRepository locationRepository, 
                                IProductRepository productRepository)
        {
            _locRep = locationRepository;
            _prodRep = productRepository;
            this.Initialize();
        }

        [Required]
        [DisplayName("Location")]
        public long ToLocationId
        {
            get; set;
        }

        [Required]
        [DisplayName("Purchase Price")]
        public decimal PurchasePrice { get; set; }

        public Location ToLocation
        {
            get
            {
                return _locRep.Find(ToLocationId);
            }
        }

        private void Initialize()
        {
            this.MovementType = StockMovementType.Purchase;
        }

        public void Copy(PurchaseMovement from)
        {
            this.ToLocationId = from.ToLocationId;
            this.ProductId = from.ProductId;
            this.Quantity = from.Quantity;
            this.MovementType = from.MovementType;
        }


    }
}
