using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Ayva.InreachIPC
{
    /// <summary>
    /// Implements:
    ///     Messaging.svc/Binary
    ///     Messaging.svc/Message
    ///     Messaging.svc/Version
    /// </summary>
    public class Messaging
    {
        /// <summary>
        /// Model to send a binary (encrypted?) message
        /// </summary>
        [ServicePath(path = "Messaging.svc/Binary", method = ServicePath.HttpMethods.POST)]
        public class BinaryMessageModel : Services.APIModel
        {
            [JsonProperty] public List<Message> Messages = new List<Message>();

            public class Message : Services.APIComponent
            {
                public byte[] _payload;
                [JsonProperty] public byte[] Payload
                {
                    get => _payload;
                    set
                    {
                        if (value.Length > 268)
                            throw new FormatException("Payload length is limited to 268 bytes");
                        _payload = value;
                    }
                }

                [JsonProperty] public List<long> Recipients = new List<long>();
                [JsonProperty] public BinaryTypeModel Type;

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
        public class TextMessageModel : Services.APIModel
        {
            [JsonProperty] public List<Message> Messages = new List<Message>();

            public class Message
            {
                private string _messageText;
                [JsonProperty(PropertyName = "Message")] public string MessageText
                {
                    get => _messageText;
                    set
                    {
                        if (value.Length > 160)
                            throw new FormatException("Text messages are limited to 160 characters");
                        _messageText = value;
                    }
                }

                [JsonProperty] public List<long> Recipients = new List<long>();
                [JsonProperty] public string Sender;
                [JsonProperty] public DateTime Timestamp;
                [JsonProperty] public ReferencePointModel ReferencePoint;
                
                public class ReferencePointModel : Services.APIComponent
                {
                    [JsonProperty] public long Altitude;
                    [JsonProperty] public CoordinateModel Coordinate = new CoordinateModel();
                    [JsonProperty] public long Course;
                    [JsonProperty] public string Label;
                    [JsonProperty] public LocationTypes LocationType;
                    [JsonProperty] public long Speed;

                    public class CoordinateModel : Services.APIComponent
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
        [ServicePath(path = "Messaging.svc/Version",  method = ServicePath.HttpMethods.GET)]
        public class VersionModel : Services.APIModel
        {
            [JsonProperty] public string Build { get; protected set; }
            [JsonProperty] public string Service { get; protected set; }
            [JsonProperty] public Uri URL { get; protected set; }
            [JsonProperty] public string Version { get; protected set; }
        }
    }
}
