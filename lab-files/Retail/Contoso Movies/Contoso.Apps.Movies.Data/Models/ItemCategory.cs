using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Contoso.Apps.Movies.Data.Models
{
    [Serializable]
    public class ItemCategory : IEntity
  {
    public int CategoryId { get; set; }

        public int ItemId { get; set; }


        public string EntityType { get { return "ProductCategory"; } }
    }
}