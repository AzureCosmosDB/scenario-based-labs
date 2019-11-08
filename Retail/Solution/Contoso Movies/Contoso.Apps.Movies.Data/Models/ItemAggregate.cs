using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Contoso.Apps.Movies.Data.Models
{
    [Serializable]
    public class ItemAggregate : Item
    {
        public string PartitionKey => ObjectId;

        new public string ObjectId { get { return this.EntityType + "_" + this.ItemId.ToString(); } }

        new public string EntityType { get { return "ItemAggregate"; } }
    }

    
}