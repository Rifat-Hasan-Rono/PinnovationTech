using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinnovationTech.Models
{
    public class City
    {
        [Key]
        public int CityId { set; get; }
        public string CityName { set; get; }

        [ForeignKey("CountryId")]
        public Country Country { get; set; }
        public int CountryId { set; get; }
    }
}
