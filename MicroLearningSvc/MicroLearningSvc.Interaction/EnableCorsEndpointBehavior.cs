using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace MicroLearningSvc.Interaction
{


    public class EnableCorsEndpointBehavior : BehaviorExtensionElement, IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters) { }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime) { }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
            var inspector = new CustomHeaderMessageInspector();
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(inspector);
        }

        public void Validate(ServiceEndpoint endpoint) { }

        public override Type BehaviorType
        {
            get { return typeof(EnableCorsEndpointBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new EnableCorsEndpointBehavior();
        }
    }

    internal class CustomHeaderMessageInspector : IDispatchMessageInspector
    {
        public CustomHeaderMessageInspector() { }

        public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel, System.ServiceModel.InstanceContext instanceContext)
        {

            var httpRequest = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
            return new
            {
                origin = httpRequest.Headers["Origin"],
                handlePreflight = httpRequest.Method.Equals("OPTIONS", StringComparison.InvariantCultureIgnoreCase)
            };
        }

        public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            var state = (dynamic)correlationState;
            if (state.handlePreflight)
            {
                reply = Message.CreateMessage(MessageVersion.None, "PreflightReturn");

                var httpResponse = new HttpResponseMessageProperty();
                reply.Properties.Add(HttpResponseMessageProperty.Name, httpResponse);

                httpResponse.SuppressEntityBody = true;
                httpResponse.StatusCode = HttpStatusCode.OK;
            }

            if (reply != null && reply.Properties.TryGetValue("httpResponse", out var resp) && resp is HttpResponseMessageProperty responseMessage)
            {
                responseMessage.Headers.Add("Access-Control-Allow-Credentials", "true");
                responseMessage.Headers.Add("Access-Control-Allow-Origin", state.origin);
                responseMessage.Headers.Add("Access-Control-Request-Method", "POST,GET,PUT,DELETE,OPTIONS");
                responseMessage.Headers.Add("Access-Control-Allow-Methods", "POST,GET,PUT,DELETE,OPTIONS");
                responseMessage.Headers.Add("Access-Control-Allow-Headers", "X-Requested-With,Content-Type,Accept,Cookie,Set-Cookie,Content-Length");
            }
            else
            {
                var headers = WebOperationContext.Current.OutgoingResponse.Headers;
                headers.Add("Access-Control-Allow-Credentials", "true");
                headers.Add("Access-Control-Allow-Origin", state.origin);
                headers.Add("Access-Control-Request-Method", "POST,GET,PUT,DELETE,OPTIONS");
                headers.Add("Access-Control-Allow-Methods", "POST,GET,PUT,DELETE,OPTIONS");
                headers.Add("Access-Control-Allow-Headers", "X-Requested-With,Content-Type,Accept,Cookie,Set-Cookie,Content-Length");
            }
        }
    }
}
