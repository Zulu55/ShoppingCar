namespace ShoppingCar.Domain.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class City
    {
        [Key]
        public int CityId { get; set; }

        [Display(Name = "País")]
        public int CountryId { get; set; }

        [Display(Name = "Ciudad")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(50, ErrorMessage = "El campo {0} no debe tener más de {1} caracteres.")]
        public string Name { get; set; }

        public virtual Country Country { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }
    }
}
