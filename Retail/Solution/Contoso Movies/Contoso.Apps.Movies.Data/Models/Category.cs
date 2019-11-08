using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using Newtonsoft.Json;

namespace Contoso.Apps.Movies.Data.Models
{
    [Serializable]
    public class Category : DbObject, IEntity
    {
        [JsonProperty(PropertyName = "partitionKey")]
        public string PartitionKey => EntityType;

        [ScaffoldColumn(false)]
        public int CategoryId { get; set; }

        [Required, StringLength(100), Display(Name = "Name")]
        public string CategoryName { get; set; }

        [Display(Name = "Product Description")]
        public string Description { get; set; }

        new public string ObjectId { get { return this.EntityType + "_" + this.CategoryId; } }

        public virtual ICollection<Item> Products { get; set; }

        public string EntityType { get { return "Category"; } }
    }
}