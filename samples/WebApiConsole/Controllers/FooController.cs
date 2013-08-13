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

        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            return null;
        }

        public HttpResponseMessage Post(HttpRequestMessage request)
        {
            return null;
        } 
    }

    public class BarController : ApiController
    {

        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            return null;
        }

        public HttpResponseMessage Post(HttpRequestMessage request)
        {
            return null;
        }
    }
}
