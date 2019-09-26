using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Contoso.Apps.Movies.Data.Models
{
    [Serializable]
    public class SimilarItem : Item
    {
        public double similarity { get; set; }

        public string sourceItemId { get; set; }
        public string targetItemId { get; set; }

        public int Target { get; set; }

        new public string EntityType { get { return "Item"; } }
    }

    
}