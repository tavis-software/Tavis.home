# Tavis.Home

This library provides support for the media type `application/json+home` http://tools.ietf.org/html/draft-nottingham-json-home-00

Resources can be added to HomeDocuments like this:

			
			var doc = new HomeDocument();

            doc.AddResource<AboutLink>(l =>
            {
                l.Target = new Uri("about", UriKind.Relative);
                l.AddHint<AllowHint>(h => h.AddMethod(HttpMethod.Get));
                l.AddHint<FormatsHint>(h => h.AddMediaType("application/json"));
                l.AddHint<AcceptPostHint>(h => h.AddMediaType("application/vnd.tavis.foo+json"));
                l.AddHint<AcceptPreferHint>(h => h.AddPreference("handling"));
            });