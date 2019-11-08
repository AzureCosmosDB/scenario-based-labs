using Contoso.Apps.Common;
using Contoso.Apps.Movies.Data.Models;
using Microsoft.Azure.Cosmos;
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
        static protected CosmosClient client;
        static protected string databaseId;
        
        static async Task Main(string[] args)
        {
            try
            {
                MovieHelper.ApiKey = ConfigurationManager.AppSettings["movieApiKey"];

                //import genre/category
                string endpointUrl = ConfigurationManager.AppSettings["dbConnectionUrl"];
                string authorizationKey = ConfigurationManager.AppSettings["dbConnectionKey"];
                databaseId = ConfigurationManager.AppSettings["databaseId"];

                client = new CosmosClient(endpointUrl, authorizationKey);

                DbHelper.client = client;
                DbHelper.databaseId = databaseId;

                await PreCalculate();

                await ImportUsers();

                await ImportGenre();

                await ImportMovies(true);
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex.Message);
            }

            Console.WriteLine("\nData import completed!");
            Console.WriteLine("You may now close this window.");
            Console.ReadLine();
        }

        private async static Task PreCalculate()
        {
            //get all the buy events, create the buy aggregates...
            var container = client.GetContainer(databaseId, "events");
            var query = container.GetItemLinqQueryable<CollectorLog>(true).Where(c=>c.Event == "buy");

            Console.WriteLine($"Saving buy aggregates");

            //do the aggregate for each product...
            foreach (var group in query.ToList().GroupBy(singleEvent => singleEvent.ContentId))
            {
                int itemId = int.Parse(group.FirstOrDefault().ContentId);

                //get the item aggregate record
                var objectId = $"ItemAggregate_{itemId}";
                ItemAggregate doc = await DbHelper.GetObject<ItemAggregate>(objectId, "ItemAggregate", objectId);

                ItemAggregate agg = new ItemAggregate();

                if (doc != null)
                {
                    doc.BuyCount = group.Count<CollectorLog>();
                }
                else
                {
                    agg.ItemId = itemId;
                    agg.BuyCount = group.Count<CollectorLog>();
                }

                await DbHelper.SaveObject(agg);
            }

        }

        async static Task ImportUsers()
        {
            List<Contoso.Apps.Movies.Data.Models.User> users = new List<Contoso.Apps.Movies.Data.Models.User>();

            users = Contoso.Apps.Movies.Data.Models.User.GetUsers();

            foreach (Contoso.Apps.Movies.Data.Models.User u in users)
            {
                try
                {
                    Console.WriteLine($"Saving user {u.UserId}");

                    await DbHelper.SaveObject(u);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                
            }
        }

        async static Task ImportGenre()
        {
            dynamic data = MovieHelper.GetMovieGenres();

            foreach(dynamic item in data.genres)
            {
                Category c = new Category();
                c.CategoryId = item.id;
                c.CategoryName = item.name.ToString();

                try
                {
                    Console.WriteLine($"Saving genre {c.CategoryName}");

                    await DbHelper.SaveObject(c);
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

        async static Task ImportMovies(bool usedOnly)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            string path = $"{dir}/Data/";

            string[] lines = System.IO.File.ReadAllLines($"{path}/movies.csv");

            string[] usedOnlyLines = System.IO.File.ReadAllLines($"{path}/UsedMovies.txt");

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
                                
                                await DbHelper.SaveObject(pc);

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

                    await DbHelper.SaveObject(p);
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
                p.ImdbId = id;
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

                await DbHelper.SaveObject(pc);

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

                Console.WriteLine($"Saving movie {p.ImdbId} : {p.ProductName}");

                await DbHelper.SaveObject(p);
            }
        }
    }
}