using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinnovationTech.Models
{
    public class Country
    {
        [Key]
        public int CountryId { set; get; }
        public string CountryName { set; get; }

        [ForeignKey("CountryId")]
        public ICollection<City> Cities { get; set; }
    }
}
