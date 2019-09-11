using Contoso.Apps.Common;
using Contoso.Apps.Movies.Data.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieDataImport
{
    class Program
    {
        static protected DocumentClient client;
        static protected Database database;
        static protected string databaseId;
        
        static void Main(string[] args)
        {
            MovieHelper.ApiKey = ConfigurationManager.AppSettings["movieApiKey"];

            //import genre/category
            string endpointUrl = ConfigurationManager.AppSettings["dbConnectionUrl"];
            string authorizationKey = ConfigurationManager.AppSettings["dbConnectionKey"];
            databaseId = ConfigurationManager.AppSettings["databaseId"];

            client = new DocumentClient(new Uri(endpointUrl), authorizationKey, new ConnectionPolicy { ConnectionMode = ConnectionMode.Gateway, ConnectionProtocol = Protocol.Https });
            database = client.CreateDatabaseIfNotExistsAsync(new Database { Id = databaseId }).Result;

            ImportUsers();

            ImportGenre();

            ImportMovies();
        }

        async static void ImportUsers()
        {
            List<Contoso.Apps.Movies.Data.Models.User> users = new List<Contoso.Apps.Movies.Data.Models.User>();
            users.Add(new Contoso.Apps.Movies.Data.Models.User { UserId = 1, Email = "horror@contosomovies.com", Name="Horror Fan" });
            users.Add(new Contoso.Apps.Movies.Data.Models.User { UserId = 2, Email = "family@contosomovies.com", Name = "Family Fan" });
            users.Add(new Contoso.Apps.Movies.Data.Models.User { UserId = 3, Email = "comedy@contosomovies.com", Name = "Comedy Fan" });
            users.Add(new Contoso.Apps.Movies.Data.Models.User { UserId = 4, Email = "romance@contosomovies.com", Name = "Romance Fan" });
            users.Add(new Contoso.Apps.Movies.Data.Models.User { UserId = 5, Email = "thriller@contosomovies.com", Name = "Thriller Fan" });

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "user");

            foreach (Contoso.Apps.Movies.Data.Models.User u in users)
            {
                var blah = client.UpsertDocumentAsync(collectionUri, u).Result;
            }
        }

        async static void ImportGenre()
        {
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "category");

            dynamic data = MovieHelper.GetMovieGenres();

            foreach(dynamic item in data.genres)
            {
                Category c = new Category();
                c.CategoryId = item.id;
                c.CategoryName = item.name.ToString();

                try
                {
                    var blah = client.UpsertDocumentAsync(collectionUri, c).Result;
                }
                catch (Exception ex)
                {

                }
            }

            /*
            string[] lines = System.IO.File.ReadAllLines("./Data/genre.csv");

            int count = 0;
            foreach(string line in lines)
            {
                count++;

                if (count == 1)
                    continue;

                Category c = new Category();
                string[] vals = line.Split(',');
                c.CategoryId = int.Parse(vals[0]);
                c.CategoryName = vals[1];
                var item = client.UpsertDocumentAsync(collectionUri, c).Result;
            }
            */
        }

        async static void ImportMovies()
        {
            Uri productCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "item");
            Uri productCatCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "item_category");

            string[] lines = System.IO.File.ReadAllLines("./Data/movies.csv");

            int count = 0;
            foreach (string line in lines)
            {
                count++;

                if (count == 1)
                    continue;

                try
                {

                    Item p = new Item();
                    string[] vals = line.Split('|');
                    p.ItemId = int.Parse(vals[0]);
                    p.ImdbId = vals[2];
                    p.ProductName = vals[3];
                    p.Description = vals[4];

                    if (vals.Length > 4)
                    {
                        string[] cats = vals[5].Replace("\"", "").Replace("[", "").Replace("]", "").Split(',');

                        foreach (string category in cats)
                        {
                            if (!string.IsNullOrEmpty(category))
                            {
                                //add a product_category...
                                ItemCategory pc = new ItemCategory();
                                pc.ItemId = p.ItemId;
                                pc.CategoryId = int.Parse(category);
                                var blah = client.UpsertDocumentAsync(productCatCollectionUri, pc);

                                p.CategoryId = pc.CategoryId;
                            }
                        }
                    }

                    //do the final lookup for other data...
                    dynamic data = MovieHelper.GetMovieDataByImdb(p.ImdbId);

                    if (data == null || data.movie_results.Count == 0)
                        continue;

                    p.Popularity = data.movie_results[0].popularity;
                    p.OriginalLanguage = data.movie_results[0].original_language;
                    p.ImagePath = data.movie_results[0].poster_path;

                    if (data.movie_results[0].release_date == null)
                        continue;

                    p.ReleaseDate = data.movie_results[0].release_date;
                    p.VoteAverage = data.movie_results[0].vote_average;
                    p.VoteCount = data.movie_results[0].vote_count;

                    p.BuyCount = 0;
                    p.AddToCartCount = 0;
                    p.ViewDetailsCount = 0;

                    TimeSpan ts = DateTime.Now - DateTime.Parse(p.ReleaseDate.ToString());

                    if (ts.TotalDays / 365 < 1)
                        p.UnitPrice = 14.99;

                    if (ts.TotalDays / 365 < 2)
                        p.UnitPrice = 12.99;

                    if (ts.TotalDays / 365 < 5)
                        p.UnitPrice = 10.99;

                    if (ts.TotalDays / 365 < 10)
                        p.UnitPrice = 7.99;

                    if (!p.UnitPrice.HasValue)
                        p.UnitPrice = 5.99;

                    var item = client.UpsertDocumentAsync(productCollectionUri, p);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}