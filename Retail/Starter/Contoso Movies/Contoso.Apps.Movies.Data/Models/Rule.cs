using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Contoso.Apps.Movies.Data.Models
{
    [Serializable]
    public class Rule : DbObject, IEntity
    {
        public string source { get; set; }

        public double support { get; set; }

        public string target { get; set; }

        public double confidence { get; set; }

        public string EntityType { get { return "Rule"; } }
    }
}