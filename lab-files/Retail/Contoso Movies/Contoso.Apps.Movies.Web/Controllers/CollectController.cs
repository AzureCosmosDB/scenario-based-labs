using Contoso.Apps.Movies.Data.Models;
using Contoso.Apps.Movies.Web.Controllers;
using Contoso.Apps.Movies.Web.Models;
using Microsoft.Azure.Documents.Client;
using System;
using System.Web.Mvc;


namespace Contoso.Apps.Movies.Controllers
{
    public class CollectController : BaseController
    {
        [HttpPost]
        public bool Log(string user_id, string content_id, string event_type, string session_id)
        {
            CollectorLog log = new CollectorLog();

            log.UserId = user_id;
            log.ContentId = content_id;
            log.Event = event_type;
            log.SessionId = session_id;
            log.Created = DateTime.Now;

            //add to cosmos db
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "collector_log");
            var item = client.UpsertDocumentAsync(collectionUri, log);

            return true;
        }

        
    }
}