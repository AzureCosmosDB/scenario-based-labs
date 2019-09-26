using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections;

namespace Contoso.Apps.Movies.Data.Models
{
    [Serializable]
    public class TopItemReport : Report
    {
        public int ItemId { get; set; }

        public Hashtable Aggregates { get; set; }

        public TopItemReport ()
        {
            this.Aggregates = new Hashtable();
        }
    }
}