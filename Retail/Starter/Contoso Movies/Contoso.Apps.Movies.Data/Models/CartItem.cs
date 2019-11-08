using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Contoso.Apps.Movies.Data.Models
{
    public class CartItem : DbObject, IEntity
    {
        [Key]
        public string CartItemId { get; set; }

        [JsonProperty(PropertyName = "partitionKey")]
        public string PartitionKey => CartId;

        public string CartId { get; set; }

        public int Quantity { get; set; }

        public System.DateTime DateCreated { get; set; }

        public int ItemId { get; set; }

        public virtual Item Product { get; set; }

        public override string ObjectId { get { return this.EntityType + "_" + this.CartId + "_" + this.ItemId; } }

        public override string EntityType { get { return "CartItem"; } }
    }
}