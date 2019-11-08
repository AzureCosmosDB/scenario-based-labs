using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel;
using System;
using Newtonsoft.Json;

namespace Contoso.Apps.Movies.Data.Models
{
    [Serializable]
    public class CollectorLog : DbObject, IEntity
    {
        [JsonProperty(PropertyName = "partitionKey")]
        public string PartitionKey => UserId;

        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "itemId")]
        public string ItemId { get; set; }

        [JsonProperty(PropertyName = "contentId")]
        public string ContentId { get; set; }

        [JsonProperty(PropertyName = "event")]
        public string Event { get; set; }

        [JsonProperty(PropertyName = "orderid")]
        public string OrderId { get; set; }

        [JsonProperty(PropertyName = "sessionId")]
        public string SessionId { get; set; }

        [JsonProperty(PropertyName = "created")]
        public System.DateTime Created { get; set; }

        override public string EntityType { get { return "Event"; } }
    }
}