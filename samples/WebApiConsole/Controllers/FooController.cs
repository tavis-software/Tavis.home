using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApiConsole.Controllers
{
    public class FooController : ApiController
    {

        [EntryPointRelation("http://example.org/rels/foo")]
        public HttpResponseMessage Get(int id)
        {
            return null;
        }

        [EntryPointRelation("http://example.org/rels/foo")]
        public HttpResponseMessage Post(int id)
        {
            return null;
        } 
    }

    public class BarController : ApiController
    {

        [EntryPointRelation("http://example.org/rels/bar")]
        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            return null;
        }

        //public HttpResponseMessage Post(HttpRequestMessage request)
        //{
        //    return null;
        //}
    }
}
