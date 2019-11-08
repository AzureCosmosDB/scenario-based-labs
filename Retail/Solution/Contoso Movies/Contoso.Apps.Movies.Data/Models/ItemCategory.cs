using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using Newtonsoft.Json;

namespace Contoso.Apps.Movies.Data.Models
{
    [Serializable]
    public class ItemCategory : DbObject, IEntity
    {
        [JsonProperty(PropertyName = "partitionKey")]
        public string PartitionKey => EntityType;
        
        public int CategoryId { get; set; }

        public int ItemId { get; set; }

        new public string ObjectId { get { return this.EntityType + "_" + this.CategoryId + "_" + this.ItemId; } }

        public string EntityType { get { return "ItemCategory"; } }
    }
}