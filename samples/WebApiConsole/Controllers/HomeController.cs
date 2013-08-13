using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
            


            IApiExplorer apiExplorer = Configuration.Services.GetApiExplorer();

            var resources = from ad in apiExplorer.ApiDescriptions
                            let id = ad.RelativePath + MakeQueryString(ad)
                            group ad by id
                            into res
                            select res;

            foreach (var res in resources)  // group by path
            {
                var link = new Link();
                link.Target = new Uri(res.Key, UriKind.Relative);

                foreach (var api in res)
                {
                    var allowHint = new AllowHint();
                    allowHint.AddMethod(api.HttpMethod);

                    link.AddHint(allowHint);
                    

                }

                foreach (ApiParameterDescription parameter in res.First().ParameterDescriptions)
                {
                    if (parameter.Source == ApiParameterSource.FromUri)
                    {
                        link.SetParameter(parameter.Name, null);
                    }

                }
               
  
                
                
                homeDocument.AddResource(link);

            }

            var stream = new MemoryStream();
            homeDocument.Save(stream);
            stream.Position = 0;

            var streamContent = new StreamContent(stream);

            return new HttpResponseMessage() { Content = streamContent };
        }

        private string MakeQueryString(ApiDescription ad)
        {
            return String.Empty;
        }
    }


}
