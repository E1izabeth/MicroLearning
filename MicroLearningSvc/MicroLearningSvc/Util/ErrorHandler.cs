using MicroLearningSvc.Interaction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MicroLearningSvc.Util
{
    class ErrorServiceBehavior : BehaviorExtensionElement, IServiceBehavior
    {
        public override Type BehaviorType { get { return this.GetType(); } }

        /// <summary>
        ///     Provides the ability to inspect the service host and the service description to confirm that the service can run
        ///     successfully.
        /// </summary>
        /// <param name="serviceDescription">The service description.</param>
        /// <param name="serviceHostBase">The service host that is currently being constructed.</param>
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) { }

        /// <summary>
        ///     Provides the ability to pass custom data to binding elements to support the contract implementation.
        /// </summary>
        /// <param name="serviceDescription">The service description of the service.</param>
        /// <param name="serviceHostBase">The host of the service.</param>
        /// <param name="endpoints">The service endpoints.</param>
        /// <param name="bindingParameters">Custom objects to which binding elements have access.</param>
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters) { }

        /// <summary>
        ///     Provides the ability to change run-time property values or insert custom extension objects such as error handlers,
        ///     message or parameter interceptors, security extensions, and other custom extension objects.
        /// </summary>
        /// <param name="serviceDescription">The service description.</param>
        /// <param name="serviceHostBase">The host that is currently being built.</param>
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            //Enumerate all channels and add the error handler to the collection
            var handler = new ErrorHandler();
            foreach (ChannelDispatcher dispatcher in serviceHostBase.ChannelDispatchers)
            {
                dispatcher.ErrorHandlers.Add(handler);
            }
        }

        protected override object CreateBehavior()
        {
            return new ErrorServiceBehavior();
        }
    }

    public class ErrorHandler : IErrorHandler
    {
        /// <summary>
        ///     This method will be called whenever an exception occurs. Therefore,
        ///     we log it and then return false so the error can continue to propagate up the chain.
        /// </summary>
        /// <param name="error">Exception being raised.</param>
        /// <returns>False to let the error propagate up the chain, or True to stop the error here.</returns>
        public bool HandleError(Exception ex)
        {
            return true;
        }

        public void ProvideFault(Exception ex, MessageVersion version, ref Message reply)
        {
            System.Diagnostics.Debug.Print(ex.FormatExceptionOutputInfo());

            var resp = WebOperationContext.Current.OutgoingResponse;

            var statusCode = resp.StatusCode = ex is WebFaultException webFault ? webFault.StatusCode : HttpStatusCode.InternalServerError;
            var statusDescr = resp.StatusDescription = HttpWorkerRequest.GetStatusDescription((int)resp.StatusCode);

            var exType = ex.GetType();

            if (WebOperationContext.Current.IncomingRequest.GetAcceptHeaderElements().Any(e => e.MediaType == "text/html"))
            {
                var title = (int)statusCode + " " + statusDescr;
                var text = @"<!DOCTYPE html> 
                        <HTML><HEAD>
                        <TITLE>" + title + @"</TITLE>
                        </HEAD><BODY>
                        <H1>" + title + @"</H1>
                        <p>Error handling request to " + WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri + @"</p>
                        <PRE>
                        " + ex.ToString() + @"
                        </PRE>
                        <p></p>
                        <HR>
                        <ADDRESS>" + typeof(Program).AssemblyQualifiedName + " at " + DateTime.Now.ToString(WcfUtils.DateFormat, CultureInfo.InvariantCulture) + @"</ADDRESS>
                        </BODY></HTML>";

                reply = Message.CreateMessage(version, null, new TextBodyWriter(text));
                reply.Properties[WebBodyFormatMessageProperty.Name] = new WebBodyFormatMessageProperty(WebContentFormat.Raw);

                var prop = new HttpResponseMessageProperty();
                prop.Headers[HttpResponseHeader.ContentType] = "text/html";
                prop.StatusCode = statusCode;
                prop.StatusDescription = statusDescr;
                reply.Properties[HttpResponseMessageProperty.Name] = prop;

                resp.ContentType = "text/html";
            }
            else
            {
                ErrorInfoType MakeErrorInfo(Exception ex2, ErrorInfoType infoToFill = null)
                {
                    if (ex2 == null)
                        return null;

                    infoToFill = infoToFill ?? new ErrorInfoType();
                    infoToFill.Message = ex2.Message;
                    infoToFill.StackTrace = ex2.StackTrace;
                    infoToFill.TypeName = ex2.GetType().FullName;
                    infoToFill.InnerError = MakeErrorInfo(ex2.InnerException);
                    return infoToFill;
                }

                ErrorInfoType MakeErrorInfoEx(Exception ex2)
                {
                    if (ex2 == null)
                        return null;

                    try
                    {
                        var buffer = new MemoryStream();
                        var formatter = new BinaryFormatter();
                        formatter.Context = new StreamingContext(StreamingContextStates.All);
                        formatter.Serialize(buffer, ex2);
                        buffer.Flush();

                        var errorInfoEx = new ExtendedErrorInfoType();
                        MakeErrorInfo(ex2, errorInfoEx);
                        errorInfoEx.RawErrorInfo = Convert.ToBase64String(buffer.ToArray());
                        return errorInfoEx;
                    }
                    catch
                    {
                        return MakeErrorInfo(ex2);
                    }
                }

                var exInfo = ExceptionDispatchInfo.Capture(ex);

                //var errorInfo = MakeErrorInfoEx(ex);
                //reply = Message.CreateMessage(version, new FaultCode(errorInfo.GetType().FullName), null, errorInfo, null);
                //reply = Message.CreateMessage(version, null, MakeErrorInfoEx(ex));
                //reply.Properties[WebBodyFormatMessageProperty.Name] = new WebBodyFormatMessageProperty(WebContentFormat.Xml);

                byte[] body;
                string contentType;

                var accept = WebOperationContext.Current.IncomingRequest.GetAcceptHeaderElements();

                if (accept.Any(ct => ct.MediaType == "application/json"))
                {
                    var errorInfo = MakeErrorInfo(ex);
                    var converter = new XmlVsJsonObjectConverter(errorInfo.GetType());
                    var formatter = new JavaScriptSerializer();
                    var tree = converter.ToTree(errorInfo, true);
                    body = Encoding.UTF8.GetBytes(formatter.Serialize(tree));
                    contentType = "application/json";
                }
                else
                {
                    var errorInfo = MakeErrorInfoEx(ex);
                    var xs = new XmlSerializer(errorInfo.GetType());
                    var ms = new MemoryStream();
                    var sw = new StreamWriter(ms);
                    xs.Serialize(sw, errorInfo);
                    sw.Flush();
                    ms.Flush();
                    body = ms.ToArray();
                    contentType = "application/xml";
                }

                reply = Message.CreateMessage(version, null, new TextBodyWriter(body));
                reply.Properties[WebBodyFormatMessageProperty.Name] = new WebBodyFormatMessageProperty(WebContentFormat.Raw);

                var prop = new HttpResponseMessageProperty();
                prop.Headers[HttpResponseHeader.ContentType] = contentType;
                prop.StatusCode = statusCode;
                prop.StatusDescription = statusDescr;
                reply.Properties[HttpResponseMessageProperty.Name] = prop;

                resp.ContentType = contentType;
            }
        }
    }

}
