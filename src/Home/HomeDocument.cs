using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tavis.Home
{
    /// <summary>
    /// Document Object Model for the media type defined here http://tools.ietf.org/html/draft-nottingham-json-home-03
    /// </summary>
    /// <remarks>
    /// Currently this class only supports serializing the JSON variant of this media type.
    /// </remarks>
    public class HomeDocument
    {
        private readonly Dictionary<string, Link> _Resources = new Dictionary<string, Link>();

        /// <summary>
        /// List of links to available resources
        /// </summary>
        public IEnumerable<Link> Resources
        {
            get { return _Resources.Values; }
        }


        /// <summary>
        /// Add a typed link to the Home Document to identify an available resource
        /// </summary>
        /// <param name="resource"></param>
        public void AddResource(Link resource)
        {
            _Resources.Add(resource.Relation, resource);
        }

        /// <summary>
        /// Retrieve a typed link based on the link relation name.
        /// </summary>
        /// <remarks>
        /// Currently the specification only supports one resource per link relation type.
        /// </remarks>
        /// <param name="name"></param>
        /// <returns></returns>
        public Link GetResource(string name)
        {
            return _Resources[name];
        }


        /// <summary>
        /// Serialize the HomeDocument model as text to a Stream
        /// </summary>
        /// <remarks>
        /// The stream will be closed when the method returns.
        /// </remarks>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            JsonWriter jsonWriter = new JsonTextWriter(sw);

            jsonWriter.Formatting = Formatting.Indented;
            jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("resources");

            jsonWriter.WriteStartObject();
            foreach (var resource in _Resources.Values)
            {
                    jsonWriter.WritePropertyName(resource.Relation);
                    jsonWriter.WriteStartObject();
                    var parameters = resource.GetParameterNames().ToList();
                    var setParameters = resource.GetParameters().ToDictionary(k=> k.Name,v=> v); // These are params that actually have values set
                    if (parameters.Any())
                    {
                        jsonWriter.WritePropertyName("href-template");
                        jsonWriter.WriteValue(resource.Target);
                        
                        jsonWriter.WritePropertyName("href-values");
                        jsonWriter.WriteStartObject();
                        foreach (var linkParameterName in parameters)
                        {
                            
                            jsonWriter.WritePropertyName(linkParameterName);
                            LinkParameter linkParameter;
                            if (setParameters.TryGetValue(linkParameterName,out linkParameter))
                            {
                                jsonWriter.WriteValue(linkParameter.Identifier);
                            }
                        }
                        jsonWriter.WriteEndObject();
                    }
                    else
                    {
                        jsonWriter.WritePropertyName("href");
                        jsonWriter.WriteValue(resource.Target);
                    }

                var hints = resource.GetHints();
                if (hints != null && hints.Any())
                {
                    jsonWriter.WritePropertyName("hints");
                    jsonWriter.WriteStartObject();

                    foreach (var hint in resource.GetHints())
                    {
                        jsonWriter.WritePropertyName(hint.Name);
                        if (hint.Content != null)
                        {
                            hint.Content.WriteTo(jsonWriter);
                        }
                    }
                    jsonWriter.WriteEndObject();
                }
                jsonWriter.WriteEndObject();
            

            }
            jsonWriter.WriteEndObject();    
            jsonWriter.WriteEndObject();

            var stw = new StreamWriter(stream);
            stw.Write(sb.ToString());
            stw.Flush();
            jsonWriter.Close();

        }

        /// <summary>
        /// Create a HomeDocument instance from a stream of JSON text that is formatted as per the json-home specification
        /// </summary>
        /// <param name="jsonStream"></param>
        /// <param name="linkFactory">Optionally specifiy a linkFactory for creating strongly typed links.  If one is not provided the default LinkFactory will be created and only IANA registered links will be available as strong types.  All unrecognized link relations will be deserialized as the base Link class.</param>
        /// <returns></returns>
        public static HomeDocument Parse(Stream jsonStream, LinkFactory linkFactory = null)
        {
            var sr = new StreamReader(jsonStream);
            return Parse(sr.ReadToEnd(), linkFactory);
        }

        /// <summary>
        /// Create a HomeDocument instance from a string of JSON text that is formatted as per the json-home specification
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="linkFactory">Optionally specifiy a linkFactory for creating strongly typed links.  If one is not provided the default LinkFactory will be created and only IANA registered links will be available as strong types.  All unrecognized link relations will be deserialized as the base Link class.</param>
        /// <returns></returns>
        public static HomeDocument Parse(string jsonString, LinkFactory linkFactory = null)
        {
            var jDoc = JObject.Parse(jsonString);
            return Parse(jDoc, linkFactory);
        }


        private static HomeDocument Parse(JObject jObject, LinkFactory linkFactory = null)
        {
            if (linkFactory == null) linkFactory = new LinkFactory();
            

            var doc = new HomeDocument();
            var resources = jObject["resources"] as JObject;
            if (resources != null)
            {
                foreach (var resourceProp in resources.Properties())
                {
                    
                    var link = linkFactory.CreateLink(resourceProp.Name);
                    
                    var resource = resourceProp.Value as JObject;

                    var hrefProp = resource.Property("href");
                    if (hrefProp != null)
                    {
                        
                        link.Target = new Uri(hrefProp.Value.Value<string>(), UriKind.RelativeOrAbsolute);
                    }
                    else
                    {
                        link.Target = new Uri(resource["href-template"].Value<string>(), UriKind.RelativeOrAbsolute);
                        var hrefvars = resource.Value<JObject>("href-vars");
                        foreach (var hrefvar in hrefvars.Properties())
                        {
                            link.SetParameter(hrefvar.Name, null, new Uri(hrefvar.Value<string>(), UriKind.RelativeOrAbsolute));  
                        }
                    }

                    var hintsProp = resource.Property("hints");
                    if (hintsProp != null)
                    {
                        var hintsObject = hintsProp.Value as JObject;
                        foreach (var hintProp in hintsObject.Properties())
                        {
                            var hint = linkFactory.HintFactory.CreateHint(hintProp.Name);
                            hint.Content = hintProp.Value;
                            link.AddHint(hint);
                        }
                        
                    }

                    doc.AddResource(link);
                }
            }

            return doc;
        }
    }


  
   

   
}
