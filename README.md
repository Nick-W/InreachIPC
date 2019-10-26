# InreachIPC
C# Library to communicate with the InReach Satellite API

## Features
### Implemented
* Binary Messages
* Text Messages
* API Version Query
* API Region Selection
### Planned
* Outbound Message Callback
* Location History
* Last Known Location
* Location Request
* Tracking
* Emergency Status

## Usage
* (Required) **InreachIPC.Config.Username**
* (Required) **InreachIPC.Config.Password**
* (Optional) **InreachIPC.Config.APIEndpoint** [US, Europe, Australia, Custom]

## Example
```C#
			//Set your credentials
            InreachIPC.Config.Username = "user@name";
            InreachIPC.Config.Password = "Correct horse battery staple!";

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
                        //Max binary payload is 268 bytes, recipient is the IMEI
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

			/** Warning: These cost money/credits on plans without unlimited messaging **/
			//Send the Binary Example
            // await API.Send(binaryMessage);

            //Send the Text Example
            // await API.Send(textMessage);
```
