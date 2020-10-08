using MicroLearningSvc.Interaction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MicroLearningCli
{
    public class WcfRestSvcClient<TSvc> : ClientBase<TSvc>
        where TSvc : class
    {
        public class MyCookieInspectionBehavior : BehaviorExtensionElement, IEndpointBehavior
        {
            public override Type BehaviorType { get { return this.GetType(); } }

            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }

            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
                clientRuntime.CallbackDispatchRuntime.CallbackClientRuntime.ClientMessageInspectors.Add(new MyCookieInspector());
            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { }

            public void Validate(ServiceEndpoint endpoint) { }

            protected override object CreateBehavior()
            {
                return new MyCookieInspector();
            }
        }

        public class MyCookieInspector : IClientMessageInspector
        {
            private readonly Dictionary<string, string> _cookies = new Dictionary<string, string>();

            public object BeforeSendRequest(ref Message request, IClientChannel channel)
            {
                if (_cookies.Count > 0 && request.Properties.ContainsKey("httpRequest"))
                {
                    HttpRequestMessageProperty httpRequest = request.Properties["httpRequest"] as HttpRequestMessageProperty;

                    if (httpRequest != null)
                    {
                        httpRequest.Headers[HttpRequestHeader.Cookie] = string.Join("; ", _cookies.Select(kv => kv.Key + "; " + kv.Value));
                    }
                }

                return null;
            }

            public void AfterReceiveReply(ref Message reply, object correlationState)
            {
                if (reply.Properties.ContainsKey("httpResponse"))
                {
                    HttpResponseMessageProperty httpResponse = reply.Properties["httpResponse"] as HttpResponseMessageProperty;

                    if (httpResponse != null)
                    {
                        string cookie = httpResponse.Headers[HttpResponseHeader.SetCookie];

                        if (cookie != null)
                        {
                            var parts = cookie.Split(new[] { ';' }, 2, StringSplitOptions.RemoveEmptyEntries);
                            var kv = parts[0].Split(new[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
                            _cookies.Add(kv[0], kv[1]);
                        }
                    }

                    if ((int)httpResponse.StatusCode >= 400)
                    {
                        var buffer = reply.CreateBufferedCopy(int.MaxValue);
                        var msg = buffer.CreateMessage();

                        var ms2 = new MemoryStream();
                        var xw = XmlWriter.Create(ms2);
                        msg.WriteMessage(xw);
                        xw.Flush();
                        ms2.Flush();

                        var text = Encoding.UTF8.GetString(ms2.ToArray());

                        ms2.Position = 0;
                        var xs = new XmlSerializer(typeof(ExtendedErrorInfoType), new[] { typeof(ErrorInfoType) });
                        var o = xs.Deserialize(ms2);

                        ErrorInfoType errorInfo;
                        Exception ex;
                        if (o is ExtendedErrorInfoType extendedErrorInfo)
                        {
                            errorInfo = extendedErrorInfo;
                            var formatter = new BinaryFormatter();
                            formatter.Context = new StreamingContext(StreamingContextStates.All);
                            ex = (Exception)formatter.Deserialize(new MemoryStream(Convert.FromBase64String(extendedErrorInfo.RawErrorInfo)));
                        }
                        else if (o is ErrorInfoType e)
                        {
                            errorInfo = e;
                            ex = null;
                        }
                        else
                        {
                            throw new NotImplementedException("wtf");
                        }

                        if (errorInfo != null)
                        {
                            throw new System.ServiceModel.Web.WebFaultException<WcfRestErrorDetails>(
                                  new WcfRestErrorDetails(errorInfo, ex), httpResponse.StatusCode
                            );
                        }
                    }
                }
            }
        }

        public WcfRestSvcClient(string svcRootUrl)
            : base(new WebHttpBinding(), new EndpointAddress(svcRootUrl))
        {
            this.Endpoint.Binding = MyWebHttp.CreateBinding();
            this.Endpoint.Behaviors.Clear();
            this.Endpoint.Behaviors.Add(new MyCookieInspectionBehavior());
            this.Endpoint.Behaviors.Add(MyWebHttp.CreateBehavior());
        }

        public TSvc Proxy { get { return this.Channel; } }
    }

    public class LerningSvcWcfClient : WcfRestSvcClient<ILearningSvc>
    {
        public LerningSvcWcfClient(string svcRootUrl) : base(svcRootUrl) { }
    }

    public class WcfRestErrorDetails
    {
        public ErrorInfoType Info { get; }
        public Exception Exception { get; }

        public WcfRestErrorDetails(ErrorInfoType info, Exception exception)
        {
            this.Info = info;
            this.Exception = exception;
        }
    }
}
