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
            InreachIPC.Config.Password = "correct horse battery staple!";

            var API = new InreachIPC.services();

            //Binary Example
            var binaryMessage = new services.Messaging.BinaryMessageModel()
            {
                Messages = new List<services.Messaging.BinaryMessageModel.Message>()
                {
                    new services.Messaging.BinaryMessageModel.Message()
                    {
                        //Max payload is 268 bytes, recipient is the IMEI
                        Payload = new byte[3]{0x00, 0x01, 0x02}, Recipients = {55555555555555},
                        Type = services.Messaging.BinaryMessageModel.Message.BinaryTypeModel.Generic
                    }
                }
            };

            //Text Example
            var textMessage = new services.Messaging.TextMessageModel()
            {
                Messages = new List<services.Messaging.TextMessageModel.Message>()
                {
                    new services.Messaging.TextMessageModel.Message()
                    {
                        MessageText = "API TextMessage Test", Recipients = {55555555555555}, Sender = "1234567890", Timestamp = DateTime.UtcNow
                    }
                }
            };

            //Send a version query
            Console.WriteLine(API.Send(new services.Messaging.VersionModel()).Result.Content.ReadAsStringAsync().Result);
            
            //Send the Binary Example
            API.Send(binaryMessage).Start();

            //Send the Text Example
            API.Send(textMessage).Start();
```
