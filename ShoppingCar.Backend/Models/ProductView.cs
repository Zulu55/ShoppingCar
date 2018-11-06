namespace ShoppingCar.Backend.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    using Domain.Models;

    public class ProductView : Product
    {
        [Display(Name = "Imagen")]
        public HttpPostedFileBase ImageFile { get; set; }
    }
}