using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCar.Domain.Models
{
    public class SaleDetailTmp
    {
        [Key]
        public int SaleDetailTmpId { get; set; }

        [Display(Name = "Cliente")]
        public int CustomerId { get; set; }

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

        [Display(Name = "Valor")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Value
        {
            get
            {
                return this.Price * (decimal)this.Quantity * (1 - (decimal)this.PercentDiscount);
            }
        }

        public virtual Customer Customer { get; set; }

        public virtual Product Product { get; set; }
    }
}
