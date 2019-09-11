using Contoso.Apps.Movies.Data.Models;
using System.Collections.Generic;

namespace Contoso.Apps.Movies.Web.Models
{
    public class UserAnalyticsModel
    {
        public UserAnalyticsModel()
        {
            this.RecommendProductsAssoc = new List<Item>();
            this.RecommendProductsContentBased = new List<Item>();
            this.RecommendProductsCollabBased = new List<Item>();
            this.RecommendProductsMatrixFactor = new List<Item>();
            this.RecommendProductsHybrid = new List<Item>();
            this.RecommendProductsRanking = new List<Item>();

            this.UsersJaccard = new List<User>();
            this.UsersPearson = new List<User>();

            this.Events = new List<CollectorLog>();

        }
        public List<Item> RecommendProductsAssoc { get; set; }

        public List<Item> RecommendProductsContentBased { get; set; }

        public List<Item> RecommendProductsCollabBased { get; set; }

        public List<Item> RecommendProductsMatrixFactor { get; set; }

        public List<Item> RecommendProductsHybrid { get; set; }

        public List<Item> RecommendProductsRanking { get; set; }

        public List<CollectorLog> Events { get; set; }

        public List<User> UsersJaccard { get; set; }

        public List<User> UsersPearson { get; set; }
    }
}