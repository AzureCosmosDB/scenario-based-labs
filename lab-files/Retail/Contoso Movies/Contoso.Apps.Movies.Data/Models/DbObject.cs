using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Apps.Movies.Data.Models
{
    public class DbObject
    {
        public string ObjectId { get; set; }        

        public string _rid { get; set; }

        public string _self { get; set; }

        public string _etag { get; set; }
        public string _attachments { get; set; }

        public string _ts { get; set; }
    }
}
