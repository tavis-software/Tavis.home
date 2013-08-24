using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using Tavis.Home;
using Tavis.IANA;
using Xunit;

namespace HomeTests
{
    public class BasicHomeTests
    {
        
        [Fact]
        public void ParseEmptyHomeDoc()
        {
            // Doc needs to be at least an object
            var doc = HomeDocument.Parse("{}");

            Assert.NotNull(doc);
        }

        [Fact]
        public void ParseHomeDocWithNoResources()
        {
            // Doc needs to be at least an object
            var doc = HomeDocument.Parse("{ \"resources\" : {} }");

            Assert.NotNull(doc);
        }

        [Fact]
        public void ParseHomeDocWithOneResource()
        {
            // Doc needs to be at least an object
            var doc = HomeDocument.Parse("{ \"resources\" :{ \"http://example.org/rels/test\" : {\"href\" : \"/test\"}  }}");

            var resource = doc.GetResource("http://example.org/rels/test");

            Assert.NotNull(resource);
            Assert.Equal(resource.Target, new Uri("/test",UriKind.Relative));
        }

        [Fact]
        public void ParseHomeDocWithMultipleResources()
        {
            // Doc needs to be at least an object
            var doc = HomeDocument.Parse("{ \"resources\" :{ \"http://example.org/rels/test\" : {\"href\" : \"/test\"}, \"http://example.org/rels/test2\" : {\"href\" : \"/test2\"}   }}");

            var resource = doc.GetResource("http://example.org/rels/test");
            var resource2 = doc.GetResource("http://example.org/rels/test2");

            Assert.NotNull(resource);
            Assert.Equal(resource.Target, new Uri("/test", UriKind.Relative));
            Assert.NotNull(resource2);
            Assert.Equal(resource2.Target, new Uri("/test2", UriKind.Relative));
        }

        [Fact]
        public void RoundTripHomeDocument()
        {
            var doc = new HomeDocument();
            doc.AddResource(new AboutLink() {Target = new Uri("about",UriKind.Relative)});

            var ms = new MemoryStream();
            doc.Save(ms);
            ms.Position = 0;

            var outDoc = HomeDocument.Parse(ms);

            Assert.NotNull(outDoc.GetResource("about"));
        }


        [Fact]
        public void RoundTripHomeDocumentWithHint()
        {
            var doc = new HomeDocument();
            var aboutLink = new AboutLink() {Target = new Uri("about", UriKind.Relative)};
            var allowHint = new AllowHint();
            allowHint.AddMethod(HttpMethod.Get);
            aboutLink.AddHint(allowHint);
            aboutLink.AddHint(new FormatsHint());
            doc.AddResource(aboutLink);

            var ms = new MemoryStream();
            doc.Save(ms);
            ms.Position = 0;

            var outDoc = HomeDocument.Parse(ms);

            var link = outDoc.GetResource("about");
            Assert.IsType<AboutLink>(link);
            Assert.IsType<AllowHint>(link.GetHints().First());
            Assert.IsType<FormatsHint>(link.GetHints().Last());
        }

    }
}
