using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestCore.Models
{
    public class Location
    {
        [HiddenInput(DisplayValue =false)]
        [Key]
        public long? LocationId { get; set; }

        [Required]
        [DisplayName("Location Code")]
        public Int16 LocationCode { get; set; }

        [Required]
        [DisplayName("Location Name")]
        [MaxLength(20, ErrorMessage ="Location name can contain 20 characters.")]
        public string LocationName { get; set; }
    }
}
