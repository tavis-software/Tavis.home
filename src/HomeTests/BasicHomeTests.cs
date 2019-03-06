﻿using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Tavis;
using Tavis.IANA;
using Tavis.UriTemplates;
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
            Assert.Equal(resource.Target, new Uri("/test", UriKind.Relative));
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

            Assert.Equal(2, doc.Resources.Count());
        }

        [Fact]
        public void RoundTripHomeDocument()
        {
            var doc = new HomeDocument();
            doc.AddResource(new AboutLink() {Target = new Uri("about", UriKind.Relative)});

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
            var aboutLink = new AboutLink() {Target = new Uri("about", UriKind.Relative)};

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

        [Fact]
        public void CreateResourceWithPathParameter()
        {
            var doc = new HomeDocument();

            doc.AddResource<AboutLink>(l => { l.Target = new Uri("http://example.org:1001/about/{id}"); });

            doc.AddResource<HelpLink>(l => { l.Target = new Uri("http://example.org:1001/help/{id}"); });

            var ms = new MemoryStream();
            doc.Save(ms);
            ms.Position = 0;

            var st = new StreamReader(ms);
            var s = st.ReadToEnd();
            Assert.True(s.Contains("href-template"));
        }

        [Fact]
        public void CreateResourceWithPathParameterAndRelWithDots()
        {
            var doc = new HomeDocument();

            doc.AddResource<Link>(l =>
            {
                l.Relation = "vnd.foo.about";
                l.Target = new Uri("http://example.org:1001/about/{id}");
            });

            doc.AddResource<HelpLink>(l => { l.Target = new Uri("http://example.org:1001/help/{id}"); });

            var ms = new MemoryStream();
            doc.Save(ms);
            ms.Position = 0;

            var st = new StreamReader(ms);
            var s = st.ReadToEnd();
            Assert.True(s.Contains("href-template"));
        }


        [Fact]
        public void CreateResource_with_extension_rel_and_template()
        {
            var doc = new HomeDocument();

            doc.AddResource<Link>(l =>
            {
                l.Relation = "http://webapibook.net/rels#issue-processor";
                l.Target = new Uri("/issueprocessor/{id}{?action}", UriKind.Relative);
            });


            var ms = new MemoryStream();
            doc.Save(ms);
            ms.Position = 0;

            var st = new StreamReader(ms);
            var s = st.ReadToEnd();
            Assert.True(s.Contains("href-template"));
        }

        [Fact]
        public void CreateResourceWithMultipleQueryParameters()
        {
            var doc = new HomeDocument();

            doc.AddResource<Link>(l =>
            {
                l.Relation = "vnd.foo.about";
                l.Template = new UriTemplate("http://example.org:1001/about{?id,name}");
            });


            var ms = new MemoryStream();
            doc.Save(ms);
            ms.Position = 0;

            var st = new StreamReader(ms);
            var s = st.ReadToEnd();
            Assert.True(s.Contains("href-template"));
        }
        //[Fact]
        //public async Task LiveParse()
        //{
        //    var httpClient = new HttpClient();

        //    var response = await httpClient.GetAsync("http://birch:1001");

        //    var homedocument = HomeDocument.Parse(await response.Content.ReadAsStreamAsync());


        //}
    }

    //[Fact]
    //public async Task LiveParse()
    //{
    //    var httpClient = new HttpClient();

    //    var response = await httpClient.GetAsync("http://birch:1001");

    //    var homedocument = HomeDocument.Parse(await response.Content.ReadAsStreamAsync());


    //}
}