using System;
using System.IO;
using InreachIPC = Ayva.InreachIPC;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace LocationTest
{
    class LocationTest
    {
        static void Main(string[] args)
        {
            //get credentials from JSON file
            JObject credentials = JObject.Parse(File.ReadAllText("creds.json"));

            //Create the API with credentials and optional endpoint
            InreachIPC.Services API = new InreachIPC.Services(username: credentials["username"].ToString(), password: credentials["password"].ToString(), region: InreachIPC.Config.RegionalEndpoints.US);
            var DeviceIMEI = 5555555555;
            //var Sender = "5165559817";
            #region
            //LastKnownLocation sample
            var locationInfo = new InreachIPC.Location.VersionModel();

            /*
                        var locationResult = API.Process(locationInfo).Result.Content.ReadAsStringAsync().Result;
                        Console.WriteLine(locationResult);
            */
            
            API.Process(locationInfo);
            Console.WriteLine(locationInfo);

            #endregion
        }
    }
}
