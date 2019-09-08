using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieDataImport.Models
{
    class Product
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string ImdbId { get; set; }

        public string OriginalLanguage { get; set; }

        public decimal Popularity { get; set; }

        public decimal VoteAverage { get; set; }

        public int VoteCount { get; set; }

        public string ProductName { get; set; }

        public string Description { get; set; }

        public string ImagePath { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string ThumbnailPath { get; set; }

        public double? UnitPrice { get; set; }

        public virtual List<Category> Category { get; set; }
    }
}
