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

            DbHelper.client = client;
            DbHelper.databaseId = databaseId;

            PreCalculate();

            ImportUsers();

            ImportGenre();

            ImportMovies(true);
        }

        private static void PreCalculate()
        {
            //get all the buy events, create the buy aggregates...
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true };
            
            //get the product
            Uri objCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "events");

            var query = client.CreateDocumentQuery<CollectorLog>(objCollectionUri, $"SELECT * FROM events f WHERE (f.event = 'buy')", options);

            //do the aggregate for each product...
            foreach (var group in query.ToList().GroupBy(singleEvent => singleEvent.ContentId))
            {
                int itemId = int.Parse(group.FirstOrDefault().ContentId);

                //get the item aggregate record
                Document doc = DbHelper.GetObject(itemId, "ItemAggregate");

                ItemAggregate agg = new ItemAggregate();

                if (doc != null)
                {
                    agg = (dynamic)doc;
                    doc.SetPropertyValue("BuyCount", group.Count<CollectorLog>());
                }
                else
                {
                    agg.ItemId = itemId;
                    agg.BuyCount = group.Count<CollectorLog>();
                }

                DbHelper.SaveObject(doc, agg);
            }

            query = client.CreateDocumentQuery<CollectorLog>(objCollectionUri, $"SELECT * FROM events f WHERE (f.event = 'details')", options);

            //do the aggregate for each product...
            foreach (var group in query.ToList().GroupBy(singleEvent => singleEvent.ContentId))
            {
                int itemId = int.Parse(group.FirstOrDefault().ContentId);

                //get the item aggregate record
                Document doc = DbHelper.GetObject(itemId, "ItemAggregate");

                ItemAggregate agg = new ItemAggregate();

                if (doc != null)
                {
                    agg = (dynamic)doc;
                    doc.SetPropertyValue("ViewDetailsCount", group.Count<CollectorLog>());
                }
                else
                {
                    agg.ItemId = itemId;
                    agg.ViewDetailsCount=  group.Count<CollectorLog>();
                }

                DbHelper.SaveObject(doc, agg);
            }

            query = client.CreateDocumentQuery<CollectorLog>(objCollectionUri, $"SELECT * FROM events f WHERE (f.event = 'addToCart')", options);

            //do the aggregate for each product...
            foreach (var group in query.ToList().GroupBy(singleEvent => singleEvent.ContentId))
            {
                int itemId = int.Parse(group.FirstOrDefault().ContentId);

                //get the item aggregate record
                Document doc = DbHelper.GetObject(itemId, "ItemAggregate");

                ItemAggregate agg = new ItemAggregate();

                if (doc != null)
                {
                    agg = (dynamic)doc;
                    doc.SetPropertyValue("AddToCartCount", group.Count<CollectorLog>());
                }
                else
                {
                    agg.ItemId = itemId;
                    agg.AddToCartCount = group.Count<CollectorLog>();
                }

                DbHelper.SaveObject(doc, agg);
            }

        }

        async static void ImportUsers()
        {
            List<Contoso.Apps.Movies.Data.Models.User> users = new List<Contoso.Apps.Movies.Data.Models.User>();

            users = Contoso.Apps.Movies.Data.Models.User.GetUsers();

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "user");

            foreach (Contoso.Apps.Movies.Data.Models.User u in users)
            {
                try
                {
                    var blah = client.UpsertDocumentAsync(collectionUri, u).Result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                DbHelper.SaveObject(null, u);
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

                    DbHelper.SaveObject(null, c);
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

        async static void ImportMovies(bool usedOnly)
        {
            Uri productCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "item");
            Uri productCatCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "item_category");

            string[] lines = System.IO.File.ReadAllLines("./Data/movies.csv");

            string[] usedOnlyLines = System.IO.File.ReadAllLines("./Data/UsedMovies.txt");

            List<string> itemIds = new List<string>();
            foreach(string line in usedOnlyLines)
            {
                itemIds.Add(line.Trim());
            }

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

                    if (usedOnly && !itemIds.Contains(vals[2]))
                        continue;

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

                                DbHelper.SaveObject(null, pc);

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

                    DbHelper.SaveObject(null, p);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            //fallback catch for anything not in the starting set
            foreach(string id in itemIds)
            {
                dynamic data = MovieHelper.GetMovieDataByImdb(id);

                if (data == null || data.movie_results.Count == 0)
                    continue;

                Item p = new Item();
                p.ItemId = data.movie_results[0].id;
                p.ImdbId = data.movie_results[0].id;
                p.ProductName = data.movie_results[0].title;
                p.Description = data.movie_results[0].overview;
                p.Popularity = data.movie_results[0].popularity;
                p.OriginalLanguage = data.movie_results[0].original_language;
                p.ImagePath = data.movie_results[0].poster_path;

                p.CategoryId = data.movie_results[0].genre_ids[0];

                //add a product_category...
                ItemCategory pc = new ItemCategory();
                pc.ItemId = p.ItemId;
                pc.CategoryId = p.CategoryId.Value;
                var blah = client.UpsertDocumentAsync(productCatCollectionUri, pc);

                DbHelper.SaveObject(null, pc);

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

                DbHelper.SaveObject(null, p);
            }
        }
    }
}