using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace MovieDataImport.Models
{
    [Serializable]
    public class Category
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string Description { get; set; }
    }
}