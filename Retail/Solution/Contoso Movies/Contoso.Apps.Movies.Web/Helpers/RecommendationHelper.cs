using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Contoso.Apps.Common;
using Contoso.Apps.Movies.Data.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace Contoso.Apps.Movies.Logic
{
    public class RecommendationHelper
    {   
        public static List<Item> GetViaFunction(string algo, int userId, int contentId)
        {
            try
            {
                return GetViaFunction(algo, userId, contentId, 10);
            }
            catch (Exception ex)
            {
                return new List<Item>();
            }
        }

        public static List<Item> GetViaFunction(string algo, int userId, int contentId, int take)
        {
            List<Item> items = new List<Item>();

            string funcUrl = ConfigurationManager.AppSettings["funcAPIUrl"];
            //string funcKey = ConfigurationManager.AppSettings["funcAPIKey"];

            dynamic request = new System.Dynamic.ExpandoObject();
            request.Algo = algo;
            request.UserId = userId;
            request.ContentId = contentId;
            request.Take = take;

            string json = JsonConvert.SerializeObject(request);

            HttpHelper hh = new HttpHelper();

            hh.ContentType = "text/plain";
            hh.Accept = "*/*";

            //string finalUrl = $"{funcUrl}/api/Recommend?code={funcKey}";
            string finalUrl = $"{funcUrl}/api/Recommend";
            string res = hh.DoPost(finalUrl, json, "");
            items = JsonConvert.DeserializeObject<List<Item>>(res);

            if (items == null)
                return new List<Item>();

            if (items.Count > take)
                items = items.Take<Item>(take).ToList();

            return items;
        }

        public static List<Data.Models.User> GetViaFunction(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
