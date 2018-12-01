namespace ShoppingCar.Domain.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Product
    {
        #region Properties
        [Key]
        public int ProductId { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} carácteres")]
        public string Name { get; set; }

        [Display(Name = "Descripción")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Precio")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }

        [Display(Name = "Imagen")]
        public string ImagePath { get; set; }

        [Display(Name = "Inventario")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public double Stock { get; set; }

        [Display(Name = "Cantidad para descuento")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public double QuantityDiscount { get; set; }

        [Display(Name = "% Descuento por cantidad")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = false)]
        [Range(0,100, ErrorMessage = "En el campo {0}, debes ingresar valores entre 0 y 100.")]
        public int PercentDiscountInt { get; set; }

        [Display(Name = "Última compra")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime LastPurchase { get; set; }

        [Display(Name = "Está disponible?")]
        public bool IsAvailable { get; set; }

        [JsonIgnore]
        public virtual ICollection<SaleDetail> SaleDetails { get; set; }

        [JsonIgnore]
        public virtual ICollection<SaleDetailTmp> SaleDetailTmps { get; set; }

        [Display(Name = "% Descuento por cantidad")]
        [DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = false)]
        public double PercentDiscount
        {
            get
            {
                return (double)this.PercentDiscountInt / 100;
            }
        }
        #endregion
    }
}