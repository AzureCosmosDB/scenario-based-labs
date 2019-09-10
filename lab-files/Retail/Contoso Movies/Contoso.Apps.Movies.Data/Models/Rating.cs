using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Contoso.Apps.Movies.Data.Models
{
    [Serializable]
    public class ItemRating : IEntity
    {
        public int ProductId { get; set; }

        public decimal Rating { get; set; }

        public string EntityType { get { return "ItemRating"; } }
    }
}