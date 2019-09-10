using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Apps.Movies.Data.Models
{
    interface IEntity
    {
        string EntityType { get; }
    }
}
