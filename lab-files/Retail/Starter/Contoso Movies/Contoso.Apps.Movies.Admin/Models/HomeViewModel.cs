using Contoso.Apps.Movies.Data.Models;
using System.Collections.Generic;

namespace Contoso.Apps.Movies.Admin.Models
{
    public class HomeModel : BaseModel
    {
        public List<Order> Orders { get; set; }
    }
}