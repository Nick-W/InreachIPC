using System;
using System.Collections.Generic;
using InreachIPC = Ayva.InreachIPC;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InreachIPC_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //Set your credentials
            InreachIPC.Config.Username = "user@name";
            InreachIPC.Config.Password = "correct horse battery staple!";

            var API = new InreachIPC.Services();
            var DeviceIMEI = 55555555555555;
            var Sender = "5165559817";

            //Binary Example
            var binaryMessage = new InreachIPC.Services.Messaging.BinaryMessageModel()
            {
                Messages = new List<InreachIPC.Services.Messaging.BinaryMessageModel.Message>()
                {
                    new InreachIPC.Services.Messaging.BinaryMessageModel.Message()
                    {
                        //Max payload is 268 bytes, recipient is the IMEI
                        Payload = new byte[3]{0x00, 0x01, 0x02}, Recipients = {DeviceIMEI},
                        Type = InreachIPC.Services.Messaging.BinaryMessageModel.Message.BinaryTypeModel.Generic
                    }
                }
            };

            //Text Example
            var textMessage = new InreachIPC.Services.Messaging.TextMessageModel()
            {
                Messages = new List<InreachIPC.Services.Messaging.TextMessageModel.Message>()
                {
                    new InreachIPC.Services.Messaging.TextMessageModel.Message()
                    {
                        MessageText = "API TextMessage Test", Recipients = {DeviceIMEI}, Sender = Sender, Timestamp = DateTime.UtcNow
                    }
                }
            };

            //Send a version query and print it to the Console
            var APIVersion = API.Send(new InreachIPC.Services.Messaging.VersionModel()).Result.Content.ReadAsStringAsync().Result;
            Console.WriteLine(JToken.Parse(APIVersion).ToString(Formatting.Indented));

            //Send the Binary Example
            //API.Send(binaryMessage);

            //Send the Text Example
            //API.Send(textMessage);
        }
    }
}
