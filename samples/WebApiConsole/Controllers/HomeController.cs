using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Tavis;
using Tavis.Home;
using Tavis.IANA;

namespace WebApiConsole.Controllers
{
    public class HomeController : ApiController
    {

        public HttpResponseMessage Get(HttpRequestMessage requestMessage)
        {
            var homeDocument = new HomeDocument();


            var fooLink = new Link()
                {
                    Relation = "http://example.org/rels/foo", 
                    Target = new Uri("foo", UriKind.Relative)
                };
            var allowHint = new AllowHint();
            allowHint.AddMethod(HttpMethod.Get);
            allowHint.AddMethod(HttpMethod.Post);
            fooLink.AddHint(allowHint);
            homeDocument.AddResource(fooLink);


            var barLink = new Link()
            {
                Relation = "http://example.org/rels/bar",
                Target = new Uri("bar", UriKind.Relative)
            };
            var allowHint2 = new AllowHint();
            allowHint2.AddMethod(HttpMethod.Get);
            barLink.AddHint(allowHint2);
            homeDocument.AddResource(barLink);


            var bar2Link = new Link()
            {
                Relation = "http://example.org/rels/bar2",
                Target = new Uri("bar/{id}", UriKind.Relative)
            };
         //   bar2Link.SetParameter("id","",new Uri("template/params/id", UriKind.Relative));
            homeDocument.AddResource(bar2Link);


            var ms = new MemoryStream();
            homeDocument.Save(ms);
            ms.Position = 0;
            var streamContent = new StreamContent(ms);
            return new HttpResponseMessage()
                {
                    Content = streamContent
                };
        }


        //public HttpResponseMessage Get2(HttpRequestMessage requestMessage)
        //{

           
        //    var homeDocument = new HomeDocument();

            
        //    IApiExplorer apiExplorer = Configuration.Services.GetApiExplorer();

        //    var resources = from ad in apiExplorer.ApiDescriptions
        //                    let id = ad.RelativePath + MakeQueryString(ad)
        //                    group ad by id
        //                        into res
        //                        select res;

        //    foreach (var res in resources)  // group by path
        //    {
        //        var apiDescription = res.First();
        //        var atts = apiDescription.ActionDescriptor.GetCustomAttributes<EntryPointRelationAttribute>();
                
        //        // Get EntryPoint Link relation name
        //        var epr = atts.FirstOrDefault();
        //        if (epr != null)
        //        {

        //            var link = new Link();
        //            link.Target = new Uri(res.Key, UriKind.Relative);
        //            link.Relation = epr.Name;

        //            var allowHint = new AllowHint();

        //            foreach (var api in res)
        //            {
        //                allowHint.AddMethod(api.HttpMethod);
        //            }
        //            link.AddHint(allowHint);


        //            foreach (ApiParameterDescription parameter in apiDescription.ParameterDescriptions)
        //            {
        //                if (parameter.Source == ApiParameterSource.FromUri)
        //                {
        //                    link.SetParameter(parameter.Name, null);
        //                }

        //            }


        //            homeDocument.AddResource(link);
        //        }
        //    }

        //    var stream = new MemoryStream();
        //    homeDocument.Save(stream);
        //    stream.Position = 0;

        //    var streamContent = new StreamContent(stream);
        //    streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        //    var responseMessage = new HttpResponseMessage() {Content = streamContent};
            
        //    return responseMessage;
            
        //}

        private string MakeQueryString(ApiDescription ad)
        {
            return String.Empty;
        }
    }


}
