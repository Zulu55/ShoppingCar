namespace ShoppingCar.Domain.Models
{
    using System.Data.Entity;

    public class DataContext : DbContext
    {
        #region Constructors
        public DbSet<Product> Products { get; set; }

        public DataContext() : base("DefaultConnection")
        {
        }
        #endregion
    }
}