using Contoso.Apps.Common;
using Contoso.Apps.Movies.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //fire off many threads of different personalities...

            DoWork(1);
            DoWork(2);
            DoWork(3);
        }

        static void DoWork(int personalityType)
        {
            //execute actions of a user...
            Guid sessionId = Guid.NewGuid();

            List<Product> movies = DbHelper.GetMoviesByType(personalityType); ;

            //loop...
            while (true)
            {
                //randomly get a movie
                Product p = GetRandomMovie(movies);

                DbHelper.GenerateAction(1, p.ProductId.ToString(), "details", sessionId.ToString().Replace("-", ""));
                DbHelper.GenerateAction(1, p.ProductId.ToString(), "buy", sessionId.ToString().Replace("-", ""));
                DbHelper.GenerateAction(1, p.ProductId.ToString(), "order", sessionId.ToString().Replace("-", ""));
            }
        }

        static Product GetRandomMovie(List<Product> movieSet)
        {
            Random r = new Random();
            return movieSet.Skip(r.Next(movieSet.Count)).Take(1).FirstOrDefault();
        }
    }
}
