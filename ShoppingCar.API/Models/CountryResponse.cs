namespace ShoppingCar.API.Models
{
    using System.Collections.Generic;
    using Domain.Models;

    public class CountryResponse
    {
        public int CountryId { get; set; }

        public string Name { get; set; }

        public ICollection<City> Cities { get; set; }
    }
}