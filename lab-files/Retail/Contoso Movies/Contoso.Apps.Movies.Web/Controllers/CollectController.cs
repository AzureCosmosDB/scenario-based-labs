using Contoso.Apps.Common.Controllers;
using Contoso.Apps.Movies.Data.Models;
using Contoso.Apps.Movies.Web.Controllers;
using Contoso.Apps.Movies.Web.Models;
using Microsoft.Azure.Documents.Client;
using System;
using System.Web.Mvc;


namespace Contoso.Apps.Movies.Controllers
{
    [AllowAnonymous]
    public class CollectController : BaseController
    {
        [HttpPost]
        public bool Log(string user_id, string content_id, string event_type, string session_id)
        {
            Contoso.Apps.Movies.Data.Models.User user = (Contoso.Apps.Movies.Data.Models.User)Session["User"];
            
            if (user != null)
            {
                string name = user.Email;
                int userId = user.UserId;

                CollectorLog log = new CollectorLog();

                log.UserId = userId;
                log.ContentId = content_id;
                log.Event = event_type;
                log.SessionId = session_id;
                log.Created = DateTime.Now;

                //add to cosmos db
                Uri collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "event");
                var item = client.UpsertDocumentAsync(collectionUri, log);
            }

            return true;
        }

        
    }
}