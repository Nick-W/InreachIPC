# InreachIPC
[![License: Unlicense](https://img.shields.io/badge/license-Unlicense-blue.svg)](http://unlicense.org/)
[![Build Status](https://dev.azure.com/AyvaOps/InreachIPC/_apis/build/status/Nick-W.InreachIPC?branchName=master)](https://dev.azure.com/AyvaOps/InreachIPC/_build/latest?definitionId=1&branchName=master)
[![NuGet Badge](https://buildstats.info/nuget/Ayva.InreachIPC)](https://www.nuget.org/packages/Ayva.InreachIPC/)

C# Library to communicate with the InReach Satellite API
## Updates
##### [1/24/2020] v0.1.3 Fix erroneous serialization on subclasses
- Prevents serialization of private attributes on subclasses where OptIn behavior was not applied.
##### [1/24/2020] v0.1.2 Proper response deserialization, exception handling, and support for non-attribute uri parameters
- Breaking change: Services.Send(APIModel) has been renamed to Services.Process(APIModel)
- Breaking change: APIMessage has been renamed to APIModel
- Breaking change: Configuration & credentials are now set when instantiating InreachIPC.Services(...)
##### [1/5/2020] v0.1.1 Added a length-check for outbound messages, throws FormatException when overrun.
- Binary messages are limited to 268 bytes
- Text messages are limited to 160 chars

## Installation
Install-Package Ayva.InreachIPC

## Features
### Implemented
* Binary Messages
* Text Messages
* API Version Query
* API Region Selection
* Exception Handling
* Response Model Deserialization
### Planned
* Outbound Message Callback
* Location History
* Last Known Location
* Location Request
* Tracking
* Emergency Status

## Supported Regions
* InreachIPC.Config.APIEndpoint: [US, Europe, Australia, Custom]

## Example
#### Setup the API
```C#
            //Create the API with credentials and optional endpoint
            InreachIPC.Services API = new InreachIPC.Services(username:"user@name", password:"correct horse battery staple!", region: InreachIPC.Config.RegionalEndpoints.US);
```
#### Send a version request
```C#
            //First create the API model - we'll use VersionModel for this example
            var APIVersion = new InreachIPC.Messaging.VersionModel();

            //Then send it to InreachIPC.Services to process the request & response.  The model will be populated with the result information, diagnostic information, and the response.
            Console.WriteLine($"Unprocessed APIVersion: {APIVersion}");
            Console.WriteLine($"Processed APIVersion: {await API.Process(APIVersion)}");

            /** Result:
                  Unprocessed APIVersion: {Model=VersionModel, Path=Messaging.svc/Version, Status=NEW, ID=87207750-1ac1-4459-b4b5-d0641140687e | JSON: "{}"}
                  Processed APIVersion: {Model=VersionModel, Path=Messaging.svc/Version, Status=PROCESSED, ID=87207750-1ac1-4459-b4b5-d0641140687e | JSON: "{"Build":"1.0.37.8399","Service":"Messaging","URL":"https://airdroptracker.com/IPCInbound/V1/Messaging.svc","Version":"V1"}"}
             */

            //There are a few other informational methods implemented on the APIModel type:
            //  Get the JSON-parsed & formatted result
            Console.WriteLine($"Model.GetJsonResult(): {await APIVersion.GetJsonResult()}");
            /** Result:
                  Model.GetJsonResult(): {
                    "Build": "1.0.37.8399",
                    "Service": "Messaging",
                    "URL": "https://airdroptracker.com/IPCInbound/V1/Messaging.svc",
                    "Version": "V1"
                  }
             */

            //  Get the raw result text
            Console.WriteLine($"Model.GetRawResult(): {await APIVersion.GetRawResult()}");
            /** Result:
                  Model.GetRawResult(): {"Build":"1.0.37.8399","Service":"Messaging","URL":"https:\/\/airdroptracker.com\/IPCInbound\/V1\/Messaging.svc","Version":"V1"}
             */
```
#### Send a binary/text message
```C#
            var DeviceIMEI = 555555555555555;
            var Sender = "5165559817";

            //Binary Message Example
            var binaryMessage = new InreachIPC.Messaging.BinaryMessageModel()
            {
                Messages = new List<InreachIPC.Messaging.BinaryMessageModel.Message>()
                {
                    new InreachIPC.Messaging.BinaryMessageModel.Message()
                    {
                        //Max payload is 268 bytes, recipient is the IMEI
                        Payload = new byte[3]{0x00, 0x01, 0x02},
                        Recipients = {DeviceIMEI},
                        Type = InreachIPC.Messaging.BinaryMessageModel.Message.BinaryTypeModel.Generic
                    }
                }
            };

            //Text Message Example
            var textMessage = new InreachIPC.Messaging.TextMessageModel()
            {
                Messages = new List<InreachIPC.Messaging.TextMessageModel.Message>()
                {
                    new InreachIPC.Messaging.TextMessageModel.Message()
                    {
                        MessageText = "API TextMessage Test",
                        Recipients = {DeviceIMEI},
                        Sender = Sender,
                        Timestamp = DateTime.UtcNow
                    }
                }
            };

            try
            {
                /** Final warning: These cost money/credits on plans without unlimited messaging, and require valid credentials **/
                Console.WriteLine(await API.Process(binaryMessage));
                Console.WriteLine(await API.Process(textMessage));
            }

            catch (InreachIPC.Services.InreachIPCException e)
            {
                Console.WriteLine(e);

                /** Example exception (formatted):
                      Ayva.InreachIPC.Services+InreachIPCException:
                        {
                            Message="API request failed",
                            Code=Forbidden,
                            Response={"Code":3,"Description":"","IMEI":null,"Message":"Invalid username or password","URL":"https:\/\/airdroptracker.com\/IPCInbound\/V1\/Messaging.svc\/Binary"},
                            Model={
                                Type=BinaryMessageModel,
                                Path=Messaging.svc/Binary,
                                Status=ERROR,
                                ID=652a8ad8-ee26-440f-8720-7523b751be86
                                JSON={"Messages":[{"_payload":"AAEC","Recipients":[555555555555555],"Type":1,"Payload":"AAEC"}]}
                              },
                            ServicesEndpoint=US
                        }
                 */
            }
```

## License
Unlicense (Public Domain)
