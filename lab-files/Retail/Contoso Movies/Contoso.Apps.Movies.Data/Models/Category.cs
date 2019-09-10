using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Contoso.Apps.Movies.Data.Models
{
    [Serializable]
    public class Category : IEntity
  {
    [ScaffoldColumn(false)]
    public int CategoryId { get; set; }

    [Required, StringLength(100), Display(Name = "Name")]
    public string CategoryName { get; set; }

    [Display(Name = "Product Description")]
    public string Description { get; set; }

    public virtual ICollection<Item> Products { get; set; }

        public string EntityType { get { return "Category"; } }
    }
}