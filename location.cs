using System;
using System.Linq;
using Newtonsoft.Json;


namespace Ayva.InreachIPC
{
    public class Location    
    {
        /// <summary>
        /// Model to retrieve the Last Known Location
        /// </summary>
        [Services.APIMessage.ServicePath(path = "", method = Services.APIMessage.ServicePath.HttpMethods.GET)]
        public class LastKnownLocationModel: Services.APIMessage
        {
/*            private long _IMEI;
            public LastKnownLocationModel(long IMEI)
            {
                ServicePath attribute = (ServicePath)this.GetType().GetCustomAttributes(typeof(Services.APIMessage.ServicePath), true).SingleOrDefault();
                attribute.path = $"Location.svc/LastKnownLocation?IMEI={_IMEI}";

            }*/

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
