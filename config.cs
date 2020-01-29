using System;
using System.Linq;

namespace Ayva.InreachIPC
{
    /// <summary>
    /// InReach Portal Credentials & Endpoint Settings
    /// </summary>
    public class Config
    {
        public string Username;
        public string Password;
        internal string customUri;

        /// <summary>
        /// Default region set to US
        /// </summary>
        public RegionalEndpoints APIEndpoint = RegionalEndpoints.US;

        /// <summary>
        /// Regional endpoints as defined in the Delorme/Garmin IPC documentation
        /// </summary>
        /// <see cref="https://files.delorme.com/support/inreachwebdocs/IPC_Inbound.pdf"/>
        /// <remarks>Custom defaults to "http://localhost", setting SetCustomApiUri will override this and automatically set the endpoint to Custom</remarks>
        public enum RegionalEndpoints
        {
            [Region(Uri = "https://explore.garmin.com/IPCInbound/V1/")]
            US,

            [Region(Uri = "https://eur-enterprise.garmin.com/IPCInbound/V1/")]
            Europe,

            [Region(Uri = "https://explore.garmin.com/IPCInbound/V1/")]
            Australia,

            [Region(Uri = "http://localhost")]
            Custom
        }

        /// <summary>
        /// (Read Only) Uri of the InReach Portal API
        /// </summary>
        public string GetEndpointUri => APIEndpoint == RegionalEndpoints.Custom ? customUri : ((Region)APIEndpoint.GetType().GetField(Enum.GetName(APIEndpoint.GetType(), APIEndpoint)).GetCustomAttributes(typeof(Region), true).SingleOrDefault()).Uri;

        public string SetCustomApiUri
        {
            set
            {
                APIEndpoint = RegionalEndpoints.Custom;
                customUri = value;
            }
        }

        public class Region : Attribute
        {
            public string Uri;
        }
    }
}
