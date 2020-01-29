using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ayva.InreachIPC
{
    /// <summary>
    /// Version and Messaging services, and Serializer
    /// </summary>
    public class Services
    {
        private readonly Config _config = new Config();
        public Services(string username, string password, Config.RegionalEndpoints region = Config.RegionalEndpoints.US)
        {
            _config.Username = username;
            _config.Password = password;
        }
        public async Task<APIModel> Process(APIModel model)
        {
            model.Response = new HttpResponseMessage();
            model.ModelStatus = APIModel.ModelStatuses.SENDING;
            using (var httpClient = new HttpClient(new HttpClientHandler() { Credentials = new NetworkCredential(_config.Username, _config.Password) }))
            {
                var modelAttribute = (APIModel.ServicePath)model.GetType().GetCustomAttributes(typeof(APIModel.ServicePath), true).Single();
                switch (modelAttribute.method)
                {
                    case APIModel.ServicePath.HttpMethods.POST:
                        var payload = JsonConvert.SerializeObject(model,
                            new JsonSerializerSettings()
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
                            });
                        var postContent = new StringContent(payload, Encoding.UTF8, "application/json");
                        model.Response = await httpClient.PostAsync($"{_config.GetEndpointUri}/{model.BuildUri()}", postContent);
                        break;
                    case APIModel.ServicePath.HttpMethods.GET:
                        model.Response = await httpClient.GetAsync($"{_config.GetEndpointUri}/{model.BuildUri()}");
                        break;
                }
            }

            if (model.Response.StatusCode != HttpStatusCode.OK)
            {
                model.ModelStatus = APIModel.ModelStatuses.ERROR;
                throw new InreachIPCException($"API request failed", model, this);
            }

            JsonConvert.PopulateObject(await model.Response.Content.ReadAsStringAsync(), model);
            model.ModelStatus = APIModel.ModelStatuses.PROCESSED;
            return model;
        }

        public class InreachIPCException : Exception
        {
            public InreachIPCException(string message, APIModel model, Services services)
                : base($"{{Message=\"{message}\"," +
                       $"{(model.Response != null ? $"Code={Enum.GetName(typeof(HttpStatusCode), model.Response.StatusCode)}, Response={model.Response.Content.ReadAsStringAsync().Result}, " : string.Empty)}" +
                       $"Model={model}, " +
                       $"ServicesEndpoint={Enum.GetName(typeof(Config.RegionalEndpoints), services._config.APIEndpoint)}}}")
            {
            }
        }

        [JsonObject(MemberSerialization.OptIn)]
        public abstract class APIComponent
        { }

        public abstract class APIModel : APIComponent
        {
            protected string pathParameters = string.Empty;
            public HttpResponseMessage Response;
            public ModelStatuses ModelStatus = ModelStatuses.NEW;
            public Guid MessageID = Guid.NewGuid();

            public enum ModelStatuses
            {
                NEW,
                SENDING,
                PROCESSED,
                ERROR
            }

            [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
            internal class ServicePath : Attribute
            {
                [Required(AllowEmptyStrings = false, ErrorMessage = "Path is required on types that implement APIMessage")]
                public string path;
                [Required(ErrorMessage = "Method is required on types that implement APIMessage")]
                public HttpMethods method;

                public enum HttpMethods
                {
                    POST,
                    GET
                }
            }

            internal virtual Uri BuildUri()
            {
                var modelAttribute = (ServicePath)GetType().GetCustomAttributes(typeof(ServicePath), true).SingleOrDefault();
                if (modelAttribute == null || String.IsNullOrEmpty(modelAttribute.path))
                    throw new ArgumentException("Model is missing required attribute data [ServicePath path=value method=value]");
                if (!String.IsNullOrEmpty(pathParameters) && modelAttribute.method != ServicePath.HttpMethods.GET)
                    throw new ArgumentException("Model has path parameters with an incompatible method type");
                return new Uri($"{modelAttribute.path}{pathParameters}", UriKind.Relative);
            }

            public async Task<string> GetJsonResult()
            {
                if (ModelStatus != ModelStatuses.PROCESSED)
                    throw new MethodAccessException("Result JSON is only available after the Model has been successfully processed");
                return JToken.Parse(await Response.Content.ReadAsStringAsync()).ToString(Formatting.Indented);
            }

            public async Task<string> GetRawResult()
            {
                if (Response == null)
                    throw new MethodAccessException("Result text is only available after the Model has been processed");
                return await Response.Content.ReadAsStringAsync();
            }

            public override string ToString()
            {
                var modelContent = JsonConvert.SerializeObject(this,
                    new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                        Formatting = Formatting.None
                    });
                return $"{{Type={GetType().Name}, Path={BuildUri()}, Status={Enum.GetName(typeof(ModelStatuses), ModelStatus)}, ID={MessageID}, JSON={modelContent}}}";}
        }
    }
}