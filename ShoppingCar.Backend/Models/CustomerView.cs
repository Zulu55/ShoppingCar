namespace ShoppingCar.Backend.Models
{
    using ShoppingCar.Domain.Models;
    using System.ComponentModel.DataAnnotations;
    using System.Web;

    public class CustomerView : Customer
    {
        [Display(Name = "Imagen")]
        public HttpPostedFileBase ImageFile { get; set; }

        [Display(Name = "País")]
        [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar un país.")]
        public int CountryId { get; set; }
    }
}