using StankinsInterfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http.Headers;
using System.Xml;
using StanskinsImplementation;

namespace SenderHttp
{
    public class SenderHttp : ISend
    {
        public string Uri { get; set; }
        public HttpMethod Verb { get; set; }
        public string MediaType { get; set; }
        public string AuthenticationType { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public IRow[] valuesToBeSent { get; set; }
        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public SenderHttp(string uri, HttpMethod verb, string mediaType, string authenticationType, string username, string password)
        {
            this.Uri = uri;
            this.Verb = verb;
            this.MediaType = mediaType;
            this.AuthenticationType = authenticationType;
            this.Username = username;
            this.Password = password;
        }
        public async Task Send()
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = new HttpResponseMessage();
                if (AuthenticationType.ToLower() == "basic")
                {
                    var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", Username, Password));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationType, Convert.ToBase64String(byteArray));
                }
                if (Verb == HttpMethod.Delete)
                {

                    response = await client.DeleteAsync(new Uri(Uri));
                }
                else
                {
                    string data = string.Empty;
                    if (valuesToBeSent != null)
                    {
                        var dictionary = valuesToBeSent.Select(it => it.Values).ToArray();
                        var settings = new JsonSerializerSettings()
                        {
                            TypeNameHandling = TypeNameHandling.Objects,
                            Formatting = Newtonsoft.Json.Formatting.Indented,
                        };
                        settings.Converters.Add(new JsonEncodingConverter());
                        data = JsonConvert.SerializeObject(dictionary, settings);
                    }
                    var content = new StringContent(data.ToString(), Encoding.UTF8, MediaType);
                    if (Verb == HttpMethod.Post)
                        response = await client.PostAsync(Uri, content);
                    else if (Verb == HttpMethod.Put)
                        response = await client.PutAsync(Uri, content);
                }
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(response.StatusCode.ToString());
                }

            }
        }
    }
}
