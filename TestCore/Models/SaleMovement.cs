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
    public class SaleMovement : StockMovement
    {
        ILocationRepository _locRep;
        IProductRepository _prodRep;

        public SaleMovement()
        {
            _locRep = new SqlLocationRepository();
            _prodRep = new SqlProductRepository();
            Initialize();
        }

        private void Initialize()
        {
            this.MovementType = StockMovementType.Sale;
        }
        
        [Required]
        [DisplayName("Transfering To")]
        public long ToLocationId { get; set; }

        [Required]
        [DisplayName("Transfering From")]
        public long FromLocationId { get; set; }


        public Location ToLocation
        {
            get
            {
                return this.FindLocation(ToLocationId);
            }
        }

        public Location FromLocation
        {
            get
            {
                return this.FindLocation(FromLocationId);
            }
        }

        private Location FindLocation(long id)
        {
            return _locRep.Find(id);
        }
        
    }
}
