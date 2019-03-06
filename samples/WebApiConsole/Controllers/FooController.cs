using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApiConsole.Controllers
{
    public class FooController : Controller
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

    public class BarController : Controller
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