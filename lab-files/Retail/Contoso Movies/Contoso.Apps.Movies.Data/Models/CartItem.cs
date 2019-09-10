using System.ComponentModel.DataAnnotations;

namespace Contoso.Apps.Movies.Data.Models
{
    public class CartItem : IEntity
    {
        [Key]
        public string ItemId { get; set; }

        public string CartId { get; set; }

        public int Quantity { get; set; }

        public System.DateTime DateCreated { get; set; }

        public int ProductId { get; set; }

        public virtual Item Product { get; set; }

        public string EntityType { get { return "CartItem"; } }
    }
}