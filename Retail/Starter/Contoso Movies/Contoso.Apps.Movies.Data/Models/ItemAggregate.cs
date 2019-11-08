using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using Newtonsoft.Json;

namespace Contoso.Apps.Movies.Data.Models
{
    [Serializable]
    public class ItemAggregate : Item
    {
        [JsonProperty(PropertyName = "partitionKey")]
        new public string PartitionKey => ObjectId;

        new public string ObjectId { get { return this.EntityType + "_" + this.ItemId.ToString(); } }

        new public string EntityType { get { return "ItemAggregate"; } }
    }

    
}