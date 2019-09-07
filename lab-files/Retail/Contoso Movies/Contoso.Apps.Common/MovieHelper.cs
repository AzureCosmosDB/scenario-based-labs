using Newtonsoft.Json;
using System.Threading;

namespace Contoso.Apps.Common
{
    public class MovieHelper
    {
        static public string ApiKey { get; set; }

        static public dynamic GetMovieDataByImdb(string movieId)
        {
            if (!string.IsNullOrEmpty(movieId))
            {
                string url = $"https://api.themoviedb.org/3/find/tt{movieId}?external_source=imdb_id&api_key={ApiKey}";

                //do some caching...
                string html = null;

                if (System.IO.File.Exists("./Data/TheMovieDbCache/" + movieId))
                    html = System.IO.File.ReadAllText("./Data/TheMovieDbCache/" + movieId);

                if (string.IsNullOrEmpty(html))
                {
                    html = HttpHelper.Get(url);
                    System.IO.File.WriteAllText("./Data/TheMovieDbCache/" + movieId, html);

                    Thread.Sleep(150);
                }

                if (string.IsNullOrEmpty(html))
                    return null;
                else
                    return JsonConvert.DeserializeObject(html);
            }

            return null;
        }

        static public dynamic GetMovieGenres()
        {
            string url = $"https://api.themoviedb.org/3/genre/movie/list?api_key={ApiKey}&language=en-US";

            //do some caching...
            string html = null;

            if (System.IO.File.Exists("./Data/TheMovieDbCache/genres.json"))
                html = System.IO.File.ReadAllText("./Data/TheMovieDbCache/genres.json");

            if (string.IsNullOrEmpty(html))
            {
                html = HttpHelper.Get(url);
                System.IO.File.WriteAllText("./Data/TheMovieDbCache/genres.json", html);
            }

            if (string.IsNullOrEmpty(html))
                return null;
            else
                return JsonConvert.DeserializeObject(html);
        }
    }
}
