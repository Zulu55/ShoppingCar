namespace ShoppingCar.Domain.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Display(Name = "Nombres")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(50, ErrorMessage = "El campo {0} no debe tener más de {1} caracteres.")]
        public string FirstNames { get; set; }

        [Display(Name = "Apellidos")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(50, ErrorMessage = "El campo {0} no debe tener más de {1} caracteres.")]
        public string LastNames { get; set; }

        [Display(Name = "Correo")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(50, ErrorMessage = "El campo {0} no debe tener más de {1} caracteres.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Ciudad")]
        [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar una ciudad.")]
        public int CityId { get; set; }

        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(50, ErrorMessage = "El campo {0} no debe tener más de {1} caracteres.")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Display(Name = "Dirección")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [MaxLength(50, ErrorMessage = "El campo {0} no debe tener más de {1} caracteres.")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        [Display(Name = "Imagen")]
        public string ImagePath { get; set; }

        [NotMapped]
        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [StringLength(20, ErrorMessage = "El campo {0} debe entre {2} y {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [NotMapped]
        [Display(Name = "Confirmación Contraseña")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Compare("Password", ErrorMessage = "La contraseña y la confirmación no concuerdan.")]
        [DataType(DataType.Password)]
        public string Confirm { get; set; }

        public virtual City City { get; set; }

        public virtual ICollection<Sale> Sales { get; set; }

        public virtual ICollection<SaleDetailTmp> SaleDetailTmps { get; set; }

        [Display(Name = "Cliente")]
        public string FullName { get { return $"{this.FirstNames} {this.LastNames}"; }  }
    }
}