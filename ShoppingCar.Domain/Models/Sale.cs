namespace ShoppingCar.Domain.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class Sale
    {
        [Key]
        public int SaleId { get; set; }

        public int CustomerId { get; set; }

        [Display(Name = "Fecha Venta")]
        public DateTime DateSale { get; set; }

        [Display(Name = "Comentarios")]
        [DataType(DataType.MultilineText)]
        public string Remarks { get; set; }

        [Display(Name = "¿Fue Despachado?")]
        public bool IsDeliveried { get; set; }

        [Display(Name = "Fecha Despacho")]
        public DateTime? DateDeliveried { get; set; }

        [JsonIgnore]
        public virtual Customer Customer { get; set; }

        [JsonIgnore]
        public virtual ICollection<SaleDetail> SaleDetails { get; set; }

        [Display(Name = "Artículos")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double TotalQuantity
        {
            get
            {
                return this.SaleDetails == null? 0 : this.SaleDetails.Sum(s => s.Quantity);
            }
        }

        [Display(Name = "Valor")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotalValue
        {
            get
            {
                return this.SaleDetails == null ? 0 : this.SaleDetails.Sum(s => s.Value);
            }
        }
    }
}
