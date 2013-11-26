using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Tavis.IANA;
using Xunit;
using Tavis.Home;

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

            Assert.Equal(2,doc.Resources.Count());

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

        [Fact]
        public void CreateHomeDocumentWithFormatsHints()
        {
            var doc = new HomeDocument();
            var aboutLink = new AboutLink() { Target = new Uri("about", UriKind.Relative) };

            aboutLink.AddHint<AllowHint>(h => h.AddMethod(HttpMethod.Get));
            aboutLink.AddHint<FormatsHint>(h => h.AddMediaType("application/json"));

            doc.AddResource(aboutLink);

            var ms = new MemoryStream();
            doc.Save(ms);
            ms.Position = 0;

            var outDoc = HomeDocument.Parse(ms);

            var link = outDoc.GetResource("about");
            Assert.IsType<AboutLink>(link);
            Assert.IsType<AllowHint>(link.GetHints().First());
            Assert.IsType<FormatsHint>(link.GetHints().Last());
            Assert.IsType<AboutLink>(outDoc.Resources.First());
        }

        [Fact]
        public void CreateHomeDocumentWithLotsOfHints()
        {
            var doc = new HomeDocument();

            doc.AddResource<AboutLink>(l =>
            {
                l.Target = new Uri("about", UriKind.Relative);
                l.AddHint<AllowHint>(h => h.AddMethod(HttpMethod.Get));
                l.AddHint<FormatsHint>(h => h.AddMediaType("application/json"));
                l.AddHint<AcceptPostHint>(h => h.AddMediaType("application/vnd.tavis.foo+json"));
                l.AddHint<AcceptPreferHint>(h => h.AddPreference("handling"));
            });

            var ms = new MemoryStream();
            doc.Save(ms);
            ms.Position = 0;

            var st = new StreamReader(ms);
            var s = st.ReadToEnd();
           
            Assert.NotNull(s);
           
        }
    }
}
