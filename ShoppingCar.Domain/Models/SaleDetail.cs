namespace ShoppingCar.Domain.Models
{
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;

    public class SaleDetail
    {
        [Key]
        public int SaleDatailId { get; set; }

        [Display(Name = "Venta")]
        public int SaleId { get; set; }

        [Display(Name = "ID Producto")]
        public int ProductId { get; set; }

        [Display(Name = "Producto")]
        public string Name { get; set; }

        [Display(Name = "Precio")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Price { get; set; }

        [Display(Name = "Cantidad")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Quantity { get; set; }

        [Display(Name = "% Descuento")]
        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double PercentDiscount { get; set; }

        [JsonIgnore]
        public virtual Sale Sale { get; set; }

        [JsonIgnore]
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
