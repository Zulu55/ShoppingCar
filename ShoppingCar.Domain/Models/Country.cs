namespace ShoppingCar.Domain.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Country
    {
        [Key]
        public int CountryId { get; set; }

        [Display(Name = "País")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(50, ErrorMessage = "El campo {0} no debe tener más de {1} caracteres.")]
        public string Name { get; set; }
        
        [JsonIgnore]
        public virtual ICollection<City> Cities { get; set; }
    }
}
