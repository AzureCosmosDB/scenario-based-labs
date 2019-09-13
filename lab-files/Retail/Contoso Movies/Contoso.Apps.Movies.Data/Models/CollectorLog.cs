using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel;
using System;

namespace Contoso.Apps.Movies.Data.Models
{
    [Serializable]
    public class CollectorLog : DbObject, IEntity
    {
        public int UserId { get; set; }

        public string ItemId { get; set; }

        public string Event { get; set; }

        public string SessionId { get; set; }

        public System.DateTime Created { get; set; }

        public string EntityType { get { return "CollectorLog"; } }
    }
}