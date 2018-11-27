namespace ShoppingCar.Domain.Models
{
    using System.ComponentModel.DataAnnotations;

    public class SaleDetail
    {
        [Key]
        public int SaleDatailId { get; set; }

        public int SaleId { get; set; }

        public int ProductId { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public double Quantity { get; set; }

        public double PercentDiscount { get; set; }

        public virtual Sale Sale { get; set; }

        public virtual Product Product { get; set; }

        [Display(Name = "Valor")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Value
        {
            get
            {
                return this.Price * (decimal)this.Quantity * (1 - (decimal)this.PercentDiscount);
            }
        }

    }
}
