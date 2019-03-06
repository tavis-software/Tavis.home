using System;

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