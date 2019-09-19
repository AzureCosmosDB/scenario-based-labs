using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Contoso.Apps.Movies.Data.Models
{
    [Serializable]
    public class Report : DbObject, IEntity
    {
        public int ReportId { get; set; }

        public string EntityType { get { return "Report"; } }
    }
}