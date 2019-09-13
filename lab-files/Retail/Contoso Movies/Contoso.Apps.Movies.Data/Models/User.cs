using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Contoso.Apps.Movies.Data.Models
{
    [Serializable]
    public class User : DbObject, IEntity
    {
        public int UserId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public int CategoryId { get; set; }

        public string Personality { get; set; }

        public string EntityType { get { return "User"; } }

        new public string ObjectId { get { return this.EntityType + "_" + this.UserId; } }

        static public List<User> GetUsers()
        {
            List<User> users = new List<User>();

            users.Add(new Contoso.Apps.Movies.Data.Models.User { UserId = 1, Email = "horror@contosomovies.com", Name = "Horror Fan", CategoryId = 27 });
            users.Add(new Contoso.Apps.Movies.Data.Models.User { UserId = 2, Email = "family@contosomovies.com", Name = "Family Fan", CategoryId = 10751 });
            users.Add(new Contoso.Apps.Movies.Data.Models.User { UserId = 3, Email = "comedy@contosomovies.com", Name = "Comedy Fan", CategoryId = 35 });
            users.Add(new Contoso.Apps.Movies.Data.Models.User { UserId = 4, Email = "romance@contosomovies.com", Name = "Romance Fan", CategoryId = 10749 });
            users.Add(new Contoso.Apps.Movies.Data.Models.User { UserId = 5, Email = "thriller@contosomovies.com", Name = "Thriller Fan", CategoryId = 53 });

            return users;
        }
    }
}