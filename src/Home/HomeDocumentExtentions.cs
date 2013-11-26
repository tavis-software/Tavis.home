using System;

namespace Tavis.Home
{
    /// <summary>
    /// Helper methods for building a HomeDocument
    /// </summary>
    public static class HomeDocumentExtentions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="link"></param>
        /// <param name="configureHint"></param>
        public static void AddHint<T>(this Link link, Action<T> configureHint) where T:Hint, new()
        {
            var hint = new T();
            configureHint(hint);
            link.AddHint(hint);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="homeDocument"></param>
        /// <param name="configureLink"></param>
        public static void AddResource<T>(this HomeDocument homeDocument, Action<T> configureLink) where T:Link, new()
        {
            var link = new T();
            configureLink(link);
            homeDocument.AddResource(link);
        }
    }
}