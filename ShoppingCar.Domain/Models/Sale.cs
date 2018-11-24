namespace ShoppingCar.Domain.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Sale
    {
        [Key]
        public int SaleId { get; set; }

        public int CustomerId { get; set; }

        public DateTime DateSale { get; set; }

        [Display(Name = "Comentarios")]
        [DataType(DataType.MultilineText)]
        public string Remarks { get; set; }

        public bool IsDeliveried { get; set; }

        public DateTime DateDeliveried { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual ICollection<SaleDetail> SaleDetails { get; set; }
    }
}
