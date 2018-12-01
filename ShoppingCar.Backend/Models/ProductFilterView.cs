namespace ShoppingCar.Backend.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Domain.Models;

    public class ProductFilterView
    {
        [Display(Name = "Filtro")]
        public string Filter { get; set; }

        public List<Product> Products { get; set; }
    }
}