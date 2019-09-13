using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Contoso.Apps.Movies.Data.Models
{
    [Serializable]
    public class Recommendation : DbObject, IEntity
    {
        public int id { get; set; }
        
        public double confidence { get; set; }

        public string EntityType { get { return "Recommendation"; } }
    }
}