using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace AutoSymSwitch
{
    [ServiceContract]
    public interface IAutoSymSwitchRestWCF
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedResponse,
            UriTemplate = "getInfo?token={token}")]
        [return: MessageParameter(Name = "SymetrixResult")]
        ResponseStatusWithInfo getInfo(string token);

        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "getPreset?token={token}")]
        [return: MessageParameter(Name = "SymetrixResult")]
        ResponseStatusWithoutInfo getPreset(string token);

        [OperationContract]
        [WebInvoke(Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "setPreset?token={token}&newPreset={newPreset}")]
        [return: MessageParameter(Name = "SymetrixResult")]
        ResponseStatusWithoutInfo setPreset(string token, string newPreset);

        [OperationContract]
        [WebInvoke(Method = "*",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedResponse,
            UriTemplate = "*")]
        [return: MessageParameter(Name = "SymetrixResult")]
        ResponseStatusWithoutInfo notFound();
    }

    [DataContract]
    public class ResponseStatus
    {
        public ResponseStatus() { }

        public ResponseStatus(string status, string desc)
        {
            code = status;
            description = desc;

            WebOperationContext context = WebOperationContext.Current;
            HttpStatusCode HttpStatus;

            bool isError = true;

            switch (status)
            {
                case "200":
                    HttpStatus = HttpStatusCode.OK;
                    isError = false;
                    break;
                case "400":
                    HttpStatus = HttpStatusCode.BadRequest;
                    break;
                case "401":
                    HttpStatus = HttpStatusCode.Unauthorized;
                    break;
                case "404":
                    HttpStatus = HttpStatusCode.NotFound;
                    break;
                case "500":
                    HttpStatus = HttpStatusCode.InternalServerError;
                    break;
                default:
                    HttpStatus = HttpStatusCode.NotImplemented;
                    break;
            }

            context.OutgoingResponse.StatusCode = HttpStatus;

            new Logger().WriteToFile(status + " - " + desc, isError);
        }
        [DataMember]
        public string code { get; set; }
        [DataMember]
        public string description { get; set; }
        // public List<ResponseStatus> Items { get; set; }
    }

    [DataContract]
    public class ResponseStatusWithInfo
    {
        public ResponseStatusWithInfo(ResponseStatus st, SymetrixInfo si)
        {
            status = st;
            information = si;
        }
        [DataMember]
        public ResponseStatus status { get; set; }
        [DataMember]
        public SymetrixInfo information { get; set; }
    }

    [DataContract]
    public class ResponseStatusWithoutInfo
    {
        public ResponseStatusWithoutInfo(ResponseStatus st)
        {
            status = st;
        }
        [DataMember]
        public ResponseStatus status { get; set; }

    }
}