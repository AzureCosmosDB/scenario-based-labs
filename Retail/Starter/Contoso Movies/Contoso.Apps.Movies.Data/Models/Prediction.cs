using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Contoso.Apps.Movies.Data.Models
{
    [Serializable]
    public class PredictionModel
    {
        public double Prediction { get; set; }

        public List<SimilarItem> Items { get; set; }

        new public string EntityType { get { return "PredictionModel"; } }
    }

    
}