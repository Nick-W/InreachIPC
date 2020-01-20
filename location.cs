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

            [JsonProperty(PropertyName = "Altitude")]
            public long Altitude;

            /*[JsonProperty(PropertyName = "Coordinate")]
            public ;*/

            [JsonProperty(PropertyName = "Course")]
            public long Course;
/*
            [JsonProperty(PropertyName = "GPSFixStatus")]
            //public*/

            [JsonProperty(PropertyName = "IMEI")]
            public long IMEI;

            [JsonProperty(PropertyName = "Speed")]
            public long Speed;

            [JsonProperty(PropertyName = "Timestamp")]
            //public 

            /*{
	"Locations":[{
		"Coordinate":{
			"Latitude":1.26743233E+15,
			"Longitude":1.26743233E+15
		},
		"GPSFixStatus":0,

		"Timestamp":"\/Date(928149600000+0000)\/"
	}]
}
*/

        }


    }

}
