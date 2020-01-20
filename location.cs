using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;


namespace Ayva.InreachIPC
{
    public class Location
    {
        [Services.APIMessage.ServicePath(path = "Location.svc/LastKnownLocation?IMEI={PARAM}", method = Services.APIMessage.ServicePath.HttpMethods.POST)]
        public class LastKnownLocation: Services.APIMessage
        {
            public class CoordinateModel
            {
                public long Latitude;
                public long Longitude;
            }
            
            [JsonProperty(PropertyName = "Altitude")]
            public long Altitude;

            [JsonProperty(PropertyName = "Coordinate")]
            public CoordinateModel Coordinate = new CoordinateModel();

            [JsonProperty(PropertyName = "Course")]
            public long Course;

            [JsonProperty(PropertyName = "GPSFixStatus")]
            public int GPSFix;
            //No Fix = 0, 2D Fix = 1, 3D Fix = 2, 3D Fix+ = 3

            [JsonProperty(PropertyName = "IMEI")]
            public string IMEI;

            [JsonProperty(PropertyName = "Speed")]
            public long Speed;

            [JsonProperty(PropertyName = "Timestamp")]
            public DateTime Timestamp;
        }
    }

}
