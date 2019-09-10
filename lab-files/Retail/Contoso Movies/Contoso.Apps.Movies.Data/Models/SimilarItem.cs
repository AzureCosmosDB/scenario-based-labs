using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Contoso.Apps.Movies.Data.Models
{
    [Serializable]
    public class SimilarItem : Item
    {
        public decimal Similarity { get; set; }

        public int Target { get; set; }

        new public string EntityType { get { return "Item"; } }
    }

    
}