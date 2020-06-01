using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShopGeocoder.Models
{
    public class ApiKey
    {
        [Key]
        public Guid Guid { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Display(Name = "API ключ")]
        public string Key { get; set; }
        [Required]
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [Display(Name = "Использовать")]
        public bool Default { get; set; }
    }
}
