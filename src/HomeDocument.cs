using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tavis.Home
{
    public class HomeDocument
    {
        private readonly Dictionary<string, Link> _Resources = new Dictionary<string, Link>();

        

        public void AddResource(Link resource)
        {
            _Resources.Add(resource.Relation, resource);
        }

        public Link GetResource(string name)
        {
            return _Resources[name];
        }


        public void Save(System.IO.MemoryStream stream)
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
                    var parameters = resource.GetParameters(); 
                    if (parameters != null && parameters.Count() > 0)
                    {
                        jsonWriter.WritePropertyName("href-template");
                        jsonWriter.WriteValue(resource.Target);
                        jsonWriter.WritePropertyName("href-values");
                        foreach (var linkParameter in parameters)
                        {
                            jsonWriter.WritePropertyName(linkParameter.Name);
                            jsonWriter.WriteValue(linkParameter.Identifier);
                        }

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

        public static HomeDocument Parse(Stream jsonStream, LinkFactory linkFactory = null)
        {
            var sr = new StreamReader(jsonStream);
            return Parse(sr.ReadToEnd(), linkFactory);
        }

        public static HomeDocument Parse(string jsonString, LinkFactory linkFactory = null)
        {
            var jDoc = JObject.Parse(jsonString);
            return Parse(jDoc, linkFactory);
        }


        public static HomeDocument Parse(JObject jObject, LinkFactory linkFactory = null)
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
