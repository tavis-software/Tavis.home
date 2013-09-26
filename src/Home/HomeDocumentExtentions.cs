using System;

namespace Tavis.Home
{
    public static class HomeDocumentExtentions
    {
        public static void AddHint<T>(this Link link, Action<T> configureHint) where T:Hint, new()
        {
            var hint = new T();
            configureHint(hint);
            link.AddHint(hint);
        }

        public static void AddResource<T>(this HomeDocument homeDocument, Action<T> configureLink) where T:Link, new()
        {
            var link = new T();
            configureLink(link);
            homeDocument.AddResource(link);
        }
    }
}