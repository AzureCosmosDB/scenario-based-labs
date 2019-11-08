using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using Newtonsoft.Json;

namespace Contoso.Apps.Movies.Data.Models
{
    [Serializable]
    public class User : DbObject, IEntity
    {
        [JsonProperty(PropertyName = "partitionKey")]
        public string PartitionKey => EntityType;
        
        public int UserId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public int CategoryId { get; set; }

        public string Personality { get; set; }

        override public string EntityType { get { return "User"; } }

        override public string ObjectId { get { return this.EntityType + "_" + this.UserId; } }

        static public List<User> GetUsers()
        {
            List<User> users = new List<User>();

            users.Add(new Contoso.Apps.Movies.Data.Models.User { UserId = 400001, Email = "mixcomedy@contosomovies.com", Name = "Mixed Comedy Fan 1", CategoryId = 35 });
            users.Add(new Contoso.Apps.Movies.Data.Models.User { UserId = 400002, Email = "mixaction@contosomovies.com", Name = "Mixed Action Fan", CategoryId = 28 });
            users.Add(new Contoso.Apps.Movies.Data.Models.User { UserId = 400003, Email = "mixcomedy@contosomovies.com", Name = "Mixed Comedy Fan 2", CategoryId = 35 });
            users.Add(new Contoso.Apps.Movies.Data.Models.User { UserId = 400004, Email = "action@contosomovies.com", Name = "Action Fan", CategoryId = 28 });
            users.Add(new Contoso.Apps.Movies.Data.Models.User { UserId = 400005, Email = "drama@contosomovies.com", Name = "Drama Fan", CategoryId = 18 });
            users.Add(new Contoso.Apps.Movies.Data.Models.User { UserId = 400006, Email = "comedy@contosomovies.com", Name = "Comedy Fan", CategoryId = 35 });

            return users;
        }
    }
}