using Contoso.Apps.Movies.Data.Models;
using System.Collections.Generic;

namespace Contoso.Apps.Movies.Web.Models
{
    public class UserAnalyticsModel
    {
        public List<Product> RecommendProductsAssoc { get; set; }

        public List<Product> RecommendProductsContentBased { get; set; }

        public List<Product> RecommendProductsCollabBased { get; set; }

        public List<Product> RecommendProductsMatrixFactor { get; set; }

        public List<Product> RecommendProductsHybrid { get; set; }

        public List<Product> RecommendProductsRanking { get; set; }

        public List<CollectorLog> Events { get; set; }

        public List<User> UsersJaccard { get; set; }

        public List<User> UsersPearson { get; set; }
    }
}