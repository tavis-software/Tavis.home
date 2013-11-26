using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Tavis.Home
{
    /// <summary>
    /// This class is an adapter between an instance of a HomeDocument and the HttpContent class needed by HttpResponseMessage 
    /// </summary>
    /// <remarks>
    /// Current this class sets the content type header to application/home+json, however as yet, the media type identifier has not been decided.  This will change.
    /// </remarks>
    /// <example>
    /// return new HttpResponseMessage() {
    ///     Content = new HomeContent(homeDocument)
    /// };
    /// </example>
    public class HomeContent : HttpContent
    {
        private readonly MemoryStream _memoryStream = new MemoryStream();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        public HomeContent(HomeDocument document)
        {
            document.Save(_memoryStream);
            _memoryStream.Position = 0;
            Headers.ContentType = new MediaTypeHeaderValue("application/home+json");
        }

        /// <summary>
        /// Copies serialized document to network stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            _memoryStream.CopyTo(stream);
            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(null);
            return tcs.Task;
        }

        /// <summary>
        /// Returns length serialized document
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        protected override bool TryComputeLength(out long length)
        {
            length = _memoryStream.Length;
            return true;
        }
    }
}
