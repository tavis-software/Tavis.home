using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiConsole
{
    public class EntryPointRelationAttribute : Attribute
    {
        private readonly string _name;

        public EntryPointRelationAttribute(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
    }
}
