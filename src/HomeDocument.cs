using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace Tavis.Home
{
    public class HomeDocument
    {
        private readonly Dictionary<string, Link> _Resources = new Dictionary<string, Link>();

      

        public void AddResource(Link resource)
        {
            _Resources.Add(resource.Relation, resource);
        }

        public Link GetResource(string name)
        {
            return _Resources[name];
        }

    }


    public class HomeDocumentReader
    {
        

    }


    public class HomeDocumentWriter
    {

    }

   

   
}
