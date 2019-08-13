using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;

namespace AutoSymSwitch
{
    public class AutoSymSwitchRestWCF : IAutoSymSwitchRestWCF
    {
        SymetrixControl Symetrix;
        string fromConnectedIP;

        public AutoSymSwitchRestWCF ()
        {
            OperationContext context = OperationContext.Current;
            MessageProperties messageProperties = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpointProperty =
              messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;

            fromConnectedIP = endpointProperty.Address;

            new Logger().WriteToFile(AccessLog());
        }

        public ResponseStatusWithoutInfo getPreset (string token)
        {
            try
            {
                Symetrix = new SymetrixControl(fromConnectedIP, token);
            }
            catch (Exception ex)
            {
                return new ResponseStatusWithoutInfo(new ResponseStatus("401", ex.Message));
            }

            return Symetrix.controlSymetrix("getPreset");
        }

        public ResponseStatusWithoutInfo setPreset (string token, string newPreset)
        {
            try
            {
                Symetrix = new SymetrixControl(fromConnectedIP, token, newPreset);
            }
            catch (Exception ex)
            {
                return new ResponseStatusWithoutInfo(new ResponseStatus("401", ex.Message));
            }

            // return mySimetrix.controlSymetrix("setPreset", token, newPreset);
            // return new ResponseStatusWithoutInfo(mySimetrix.controlSymetrix("setPreset", token, newPreset));
            return Symetrix.controlSymetrix("setPreset", newPreset);
        }

        public ResponseStatusWithInfo getInfo (string token)
        {
            try
            {
                Symetrix = new SymetrixControl(fromConnectedIP, token);
            }
            catch (Exception ex)
            {
                return new ResponseStatusWithInfo(new ResponseStatus("401", ex.Message), null);
            }

            return Symetrix.getInfo();
        }

        public ResponseStatusWithoutInfo notFound()
        {
            return new ResponseStatusWithoutInfo(new ResponseStatus("404", "Resource not found"));
        }

        private string AccessLog()
        {
            WebOperationContext context = WebOperationContext.Current;
            string RequestUrl = context.IncomingRequest.UriTemplateMatch.RequestUri.OriginalString;
            string Method = context.IncomingRequest.Method;

            return fromConnectedIP + " - " + Method + " " + RequestUrl;
        }
   }
}
