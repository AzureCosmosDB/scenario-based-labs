using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using Newtonsoft.Json;

namespace Contoso.Apps.Movies.Data.Models
{
    [Serializable]
    public class Item : DbObject, IEntity
    {
        [JsonProperty(PropertyName = "partitionKey")]
        public string PartitionKey => ObjectId;

        [ScaffoldColumn(false)]
        public int ItemId { get; set; }

        new public string ObjectId { get { return this.EntityType + "_" + this.ItemId.ToString(); } }

        public int BuyCount { get; set; }
        public int ViewDetailsCount { get; set; }
        public int AddToCartCount { get; set; }
        public int VoteCount { get; set; }

        [Required, StringLength(100), Display(Name = "Name")]
        public string ProductName { get; set; }

        public string ImdbId { get; set; }

        [Required, StringLength(10000), Display(Name = "Product Description"), DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public string ImagePath { get; set; }

        public string ThumbnailPath { get; set; }

        [Display(Name = "Price")]
        public double? UnitPrice { get; set; }

        public int? CategoryId { get; set; }

        public virtual Category Category { get; set; }

        public string EntityType { get { return "Item"; } }

        public dynamic Popularity { get; set; }
        public dynamic OriginalLanguage { get; set; }
        public dynamic ReleaseDate { get; set; }
        public dynamic VoteAverage { get; set; }
    }

    // This class is used to compare two objects of type Product to remove 
    // all objects that are duplicates, as determined by the ProductID field.
    public class ProductsComparer : IEqualityComparer<Item>
    {
        public bool Equals(Item x, Item y)
        {
            if (x.ItemId == y.ItemId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(Item obj)
        {
            return obj.ItemId.GetHashCode();
        }
    }
}