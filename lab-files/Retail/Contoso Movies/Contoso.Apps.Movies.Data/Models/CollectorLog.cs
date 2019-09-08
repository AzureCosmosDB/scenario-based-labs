using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel;
using System;

namespace Contoso.Apps.Movies.Data.Models
{
    [Serializable]
    public class CollectorLog
    {
        public int UserId { get; set; }

        public string ContentId { get; set; }

        public string Event { get; set; }

        public string SessionId { get; set; }

        public System.DateTime Created { get; set; }

    }
}