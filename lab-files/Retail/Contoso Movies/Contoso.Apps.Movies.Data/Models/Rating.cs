using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Contoso.Apps.Movies.Data.Models
{
    [Serializable]
    public class ItemRating : DbObject, IEntity
    {
        public int ItemId { get; set; }

        public decimal Rating { get; set; }

        public string EntityType { get { return "ItemRating"; } }
    }
}