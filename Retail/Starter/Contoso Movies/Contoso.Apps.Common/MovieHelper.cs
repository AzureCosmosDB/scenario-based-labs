using Newtonsoft.Json;
using System;
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

                string dir = AppDomain.CurrentDomain.BaseDirectory;
                string path = $"{dir}/Data/TheMovieDbCache/" + movieId;

                if (System.IO.File.Exists(path))
                    html = System.IO.File.ReadAllText(path);

                if (string.IsNullOrEmpty(html) || html.Contains("Invalid API key") || html.Contains("over the allowed limit"))
                {
                    HttpHelper hh = new HttpHelper();
                    html = hh.DoGet(url, "");

                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);

                    System.IO.File.WriteAllText(path, html);

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

            string dir = AppDomain.CurrentDomain.BaseDirectory;
            string path = $"{dir}/Data/TheMovieDbCache/genres.json";

            if (System.IO.File.Exists(path))
                html = System.IO.File.ReadAllText(path);

            if (string.IsNullOrEmpty(html) || html.Contains("Invalid API key"))
            {
                HttpHelper hh = new HttpHelper();
                html = hh.DoGet(url, "");

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                System.IO.File.WriteAllText(path, html);
            }

            if (string.IsNullOrEmpty(html))
                return null;
            else
                return JsonConvert.DeserializeObject(html);
        }
    }
}
