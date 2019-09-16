using Contoso.Apps.Common;
using Contoso.Apps.Movies.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            //28 = action
            //12 = adventure
            //35 = comedy
            //18 = drama
            //27 = horror

            //int[] types =  new int[] { 27, 28, 12, 35, 18 };

            int[] types = new int[] { 28 };

            //fire off many threads of different personalities...
            foreach (int type in types)
            {
                DoWork(type);
            }
        }

        static void DoWork(int personalityType)
        {
            List<User> users = User.GetUsers();

            User u = users.Where(u1 => u1.CategoryId == personalityType).FirstOrDefault();

            UserActor ua = new UserActor(u.UserId, personalityType);

            Thread t = new Thread(new ThreadStart(ua.DoWork));
            t.Start();
        }
    }
}
