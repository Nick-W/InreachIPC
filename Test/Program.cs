using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using InreachIPC = Ayva.InreachIPC;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InreachIPC_Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            #region API Setup
            //Create the API with credentials and optional endpoint
            InreachIPC.Services API = new InreachIPC.Services(username:"user@name", password:"correct horse battery staple!", region: InreachIPC.Config.RegionalEndpoints.US);
            var DeviceIMEI = 555555555555555;
            var Sender = "5165559817";
            #endregion

            #region Send API Version Request (does not require valid credentials)
            //First create the API model - we'll use VersionModel here
            var APIVersion = new InreachIPC.Messaging.VersionModel();

            //Then send it to InreachIPC.Services to process the request & response.  The model will be populated with the result information, diagnostics, and the response.
            Console.WriteLine($"Unprocessed APIVersion: {APIVersion}");
            Console.WriteLine($"Processed APIVersion: {await API.Process(APIVersion)}");

            /** Result:
                  New APIVersion: {Model=VersionModel, Path=Messaging.svc/Version, Status=NEW, ID=87207750-1ac1-4459-b4b5-d0641140687e | JSON: "{}"}
                  Processed APIVersion: {Model=VersionModel, Path=Messaging.svc/Version, Status=PROCESSED, ID=87207750-1ac1-4459-b4b5-d0641140687e | JSON: "{"Build":"1.0.37.8399","Service":"Messaging","URL":"https://airdroptracker.com/IPCInbound/V1/Messaging.svc","Version":"V1"}"}
             */

            //There are a few other informational methods implemented on the APIModel type to aid with development:
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
            #endregion

            #region Send Binary/Text Message
#if I_AM_AWARE_THAT_THESE_CAN_COST_MONEY
            
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
#endif
            #endregion
        }
    }
}
