using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShopGeocoder.Models
{
    public class Shop
    {
        [Key]
        public Guid Guid { get; set; }
        [Display(Name = "Наименование")]
        public string Name { get; set; }
        [Display(Name = "Адрес")]
        public string Address { get; set; }
        [Display(Name = "Доп. Адрес")]
        public string AddressExt { get; set; }
        [Display(Name = "Долгота")]
        public double Longitude { get; set; }
        [Display(Name = "Широта")]
        public double Latitude { get; set; }
    }
}
