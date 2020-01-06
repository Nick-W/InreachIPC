using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ayva.InreachIPC
{
    public class Services
    {
        public async Task<HttpResponseMessage> Send(APIMessage model)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            using (var httpClient = new HttpClient(new HttpClientHandler() {Credentials = new NetworkCredential(Config.Username, Config.Password)}))
            {
                var modelAttribute = (APIMessage.ServicePath) model.GetType().GetCustomAttributes(typeof(APIMessage.ServicePath), true).SingleOrDefault();
                switch (modelAttribute.method)
                {
                    case APIMessage.ServicePath.HttpMethods.POST:
                        var payload = JsonConvert.SerializeObject(model,
                            new JsonSerializerSettings()
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
                            });
                        var postContent = new StringContent(payload, Encoding.UTF8, "application/json");
                        response = await httpClient.PostAsync($"{Config.GetEndpointUri}/{modelAttribute.path}", postContent);
                        break;
                    case APIMessage.ServicePath.HttpMethods.GET:
                        response = await httpClient.GetAsync($"{Config.GetEndpointUri}/{modelAttribute.path}");
                        break;
                }
            }

            if (response.StatusCode != HttpStatusCode.OK)
                throw new HttpRequestException(
                    $"API request failed - code {response.StatusCode}: {response.Content.ReadAsStringAsync()}");

            return response;
        }

        public class APIMessage
        {
            public class ServicePath : Attribute
            {
                public string path;
                public HttpMethods method;

                public enum HttpMethods
                {
                    POST,
                    GET
                }
            }
        }

        public static class Messaging
        {
            /// <summary>
            /// Model to send a binary (encrypted?) message
            /// </summary>
            [ServicePath(path = "Messaging.svc/Binary", method = ServicePath.HttpMethods.POST)]
            public class BinaryMessageModel : APIMessage
            {
                [JsonProperty(PropertyName = "Messages")]
                public List<Message> Messages = new List<Message>();

                public class Message
                {
                    [JsonProperty(PropertyName = "Payload")]
                    public byte[] Payload
                    {
                        get => _payload;
                        set
                        {
                            if (value.Length > 268)
                                throw new FormatException("Payload length is limited to 268 bytes");
                            _payload = value;
                        }
                    }

                    public byte[] _payload;

                    [JsonProperty(PropertyName = "Recipients")]
                    public List<long> Recipients = new List<long>();

                    [JsonProperty(PropertyName = "Type")] public BinaryTypeModel Type;

                    public enum BinaryTypeModel
                    {
                        Encrypted = 0,
                        Generic = 1,
                        EncryptedPinpoint = 2
                    }
                }
            }

            /// <summary>
            /// Model to send a text message, with optional reference data
            /// </summary>
            [ServicePath(path = "Messaging.svc/Message", method = ServicePath.HttpMethods.POST)]
            public class TextMessageModel : APIMessage
            {
                [JsonProperty(PropertyName = "Messages")]
                public List<Message> Messages = new List<Message>();

                public class Message
                {
                    [JsonProperty(PropertyName = "Message")]
                    public string MessageText
                    {
                        get => _messageText;
                        set
                        {
                            if (value.Length > 160)
                                throw new FormatException("Text messages are limited to 160 characters");
                            _messageText = value;
                        }
                    }

                    private string _messageText;

                    [JsonProperty(PropertyName = "Recipients")]
                    public List<long> Recipients = new List<long>();

                    [JsonProperty(PropertyName = "Sender")]
                    public string Sender;

                    [JsonProperty(PropertyName = "Timestamp")]
                    public DateTime Timestamp;

                    [JsonProperty(PropertyName = "ReferencePoint")]
                    public ReferencePointModel ReferencePoint;


                    public class ReferencePointModel
                    {
                        [JsonProperty(PropertyName = "Altitude")]
                        public long Altitude;

                        [JsonProperty(PropertyName = "Coordinate")]
                        public CoordinateModel Coordinate = new CoordinateModel();

                        [JsonProperty(PropertyName = "Course")]
                        public long Course;

                        [JsonProperty(PropertyName = "Label")] public string Label;

                        [JsonProperty(PropertyName = "LocationType")]
                        public LocationTypes LocationType;

                        [JsonProperty(PropertyName = "Speed")] public long Speed;

                        public class CoordinateModel
                        {
                            public long Latitude;
                            public long Longitude;
                        }

                        public enum LocationTypes
                        {
                            ReferencePoint = 0,
                            GPSLocation = 1
                        }
                    }
                }
            }

            /// <summary>
            /// Model to retrieve the API Version
            /// </summary>
            [ServicePath(path = "Messaging.svc/Version", method = ServicePath.HttpMethods.GET)]
            public class VersionModel : APIMessage
            {
            }
        }
    }
}
