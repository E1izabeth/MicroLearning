using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mime;
using SD = System.Web.Services.Description;
using System.Reflection;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Web.Script.Serialization;
using System.Net;
using System.Collections.Specialized;

namespace MicroLearningSvc.Interaction
{
    public static class MyWebHttp
    {
        public static Binding CreateBinding()
        {
            var httpBinding = new WebHttpBinding(WebHttpSecurityMode.None)
            {
                AllowCookies = true
            };

            //var httpBinding = new BasicHttpBinding(BasicHttpSecurityMode.None);

            return httpBinding;
        }

        public static WebHttpBehavior CreateBehavior(bool service = false)
        {
            var httpBehavior = new MyWebHttpBehavior(service)
            {
                DefaultBodyStyle = WebMessageBodyStyle.Bare,
                AutomaticFormatSelectionEnabled = true,
                DefaultOutgoingRequestFormat = WebMessageFormat.Xml,
                DefaultOutgoingResponseFormat = WebMessageFormat.Xml,
            };

            return httpBehavior;
        }
    }

    class MyWebHttpBehavior : WebHttpBehavior
    {
        private bool _service;

        public MyWebHttpBehavior(bool service)
        {
            _service = service;
        }

        protected override IDispatchMessageFormatter GetRequestDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
        {
            return _service ? new MyDispatchMessageFormatter(operationDescription, true, endpoint) : base.GetRequestDispatchFormatter(operationDescription, endpoint);
        }

        protected override IDispatchMessageFormatter GetReplyDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
        {
            return _service ? new MyDispatchMessageFormatter(operationDescription, true, endpoint) : base.GetReplyDispatchFormatter(operationDescription, endpoint);
        }
    }

    class MyDispatchMessageFormatter : IDispatchMessageFormatter
    {
        readonly OperationDescription _operation;
        readonly ServiceEndpoint _endpoint;

        readonly int _bodyParameterIndex;
        readonly Type _bodyType;

        readonly RestRequestUrlMapping _mapping;

        public MyDispatchMessageFormatter(OperationDescription operation, bool isRequest, ServiceEndpoint endpoint)
        {
            _operation = operation;
            _endpoint = endpoint;

            if (isRequest)
            {
                var getInfo = operation.Behaviors.OfType<WebGetAttribute>().FirstOrDefault();
                var invokeInfo = operation.Behaviors.OfType<WebInvokeAttribute>().FirstOrDefault();
                var urlTemplate = getInfo?.UriTemplate ?? invokeInfo?.UriTemplate ?? null;

                if (urlTemplate != null)
                {
                    _mapping = new RestRequestUrlMapping(urlTemplate, operation.SyncMethod);
                    if (_mapping.BodyParameterInfo != null)
                    {
                        _bodyType = _mapping.BodyParameterInfo.Type;
                        _bodyParameterIndex = _mapping.BodyParameterInfo.Index;
                    }
                }
            }
        }

        private object GetPropertyValue(object obj, string propName)
        {
            var prop = obj.GetType().GetProperty(propName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return prop.GetValue(obj);
        }

        public void DeserializeRequest(Message message, object[] parameters)
        {
            var tail = message.Headers.To.PathAndQuery.Substring(_endpoint.ListenUri.PathAndQuery.Length - 1);
            var match = _mapping.UriFragmentRegex.Match(tail);
            if (match.Success)
            {
                var values = _mapping.UrlParametersInfo.Select(p => new { p.Name, value = this.TryParse(match.Groups[p.Name].Value, p.Type) }).ToArray();
                if (values.Any(v => !v.value.Item1))
                    throw new WebFaultException(HttpStatusCode.BadRequest);

                _mapping.FillParameters(values.Select(v => new KeyValuePair<string, object>(v.Name, v.value.Item2)), parameters);
            }

            if (_bodyType != null)
            {
                var data = this.GetPropertyValue(message, "MessageData"); // TODO: check if body presetned
                var dataBuffer = (ArraySegment<byte>)this.GetPropertyValue(data, "Buffer");
                // System.ServiceModel.Channels.BufferManager

                //WebOperationContext.Current.OutgoingResponse.Headers["Access-Control-Allow-Origin"] = "*";
                //WebOperationContext.Current.OutgoingResponse.Headers["Access-Control-Request-Method"] = "POST,GET,DELETE";
                //WebOperationContext.Current.OutgoingResponse.Headers["Access-Control-Allow-Headers"] = "Content-Type,Accept";

                var contentType = WebOperationContext.Current.IncomingRequest.ContentType;

                //XmlDictionaryReader bodyReader = message.GetReaderAtBodyContents();
                //bodyReader.ReadStartElement("Binary");
                //byte[] rawBody = bodyReader.ReadContentAsBase64();

                var requestText = Encoding.UTF8.GetString(dataBuffer.Array, dataBuffer.Offset, dataBuffer.Count);
                MemoryStream ms = new MemoryStream(dataBuffer.Array, dataBuffer.Offset, dataBuffer.Count);
                StreamReader sr = new StreamReader(ms);

                if (_bodyType != null)
                {
                    if (contentType == "application/xml")
                    {
                        var xs = new XmlSerializer(_bodyType);
                        var sw = new StringWriter();
                        var obj = xs.Deserialize(sr);
                        parameters[_bodyParameterIndex] = obj;
                    }
                    else
                    {
                        var converter = new XmlVsJsonObjectConverter(_bodyType);
                        var formatter = new JavaScriptSerializer();
                        var tree = formatter.DeserializeObject(sr.ReadToEnd());
                        var obj = converter.FromTree(tree, _bodyType);
                        parameters[_bodyParameterIndex] = obj;
                    }
                }
            }
        }

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            Message replyMessage;

            if (result != null)
            {
                var accept = WebOperationContext.Current.IncomingRequest.GetAcceptHeaderElements();
                byte[] body;
                string contentType;

                if (accept.Any(ct => ct.MediaType == "application/json"))
                {
                    var converter = new XmlVsJsonObjectConverter(result.GetType());
                    var formatter = new JavaScriptSerializer();
                    var tree = converter.ToTree(result, true);
                    body = Encoding.UTF8.GetBytes(formatter.Serialize(tree));
                    contentType = "application/json";
                }
                else
                {
                    var xs = new XmlSerializer(result.GetType());
                    var ms = new MemoryStream();
                    var sw = new StreamWriter(ms);
                    xs.Serialize(sw, result);
                    sw.Flush();
                    ms.Flush();
                    body = ms.ToArray();
                    contentType = "application/xml";
                }

                replyMessage = Message.CreateMessage(messageVersion, _operation.Messages[1].Action, new TextBodyWriter(body));
                replyMessage.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Raw));
                
                HttpResponseMessageProperty respProp = new HttpResponseMessageProperty();
                respProp.Headers[HttpResponseHeader.ContentType] = contentType;
                replyMessage.Properties.Add(HttpResponseMessageProperty.Name, respProp);
            }
            else
            {
                replyMessage = Message.CreateMessage(messageVersion, _operation.Messages[1].Action);

                HttpResponseMessageProperty respProp = new HttpResponseMessageProperty();
                replyMessage.Properties.Add(HttpResponseMessageProperty.Name, respProp);
            }

            return replyMessage;
        }


        private Tuple<bool, object> TryParse(string str, Type type)
        {
            bool ok;
            object result;

            if (type == typeof(Guid))
            {
                ok = Guid.TryParse(str, out var value);
                result = value;
            }
            else if (type == typeof(string))
            {
                ok = true;
                result = str;
            }
            else if (type == typeof(bool))
            {
                ok = true;
                switch (str.ToLower())
                {
                    case "true":
                    case "yes":
                    case "1":
                        {
                            ok = true;
                            result = true;
                        }
                        break;
                    case "false":
                    case "0":
                    case "no":
                        {
                            ok = true;
                            result = false;
                        }
                        break;
                    default:
                        {
                            ok = false;
                            result = null;
                        }
                        break;
                }
            }
            else
            {
                var delegateType = typeof(TryParseDelegate<>).MakeGenericType(type);
                var parseParameterTypes = delegateType.GetMethod("Invoke").GetParameters().Select(p => p.ParameterType).ToArray();

                var parseMethod = type.GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, parseParameterTypes, null);
                var parseDelegate = Delegate.CreateDelegate(delegateType, parseMethod);

                if (parseMethod != null)
                {
                    var makeWrapperMethod = this.GetType().GetMethod("MakeConvMethod", BindingFlags.Static | BindingFlags.NonPublic);
                    var wrapperMethod = (Func<string, Tuple<bool, object>>)makeWrapperMethod.Invoke(null, new[] { parseDelegate });

                    return wrapperMethod(str);
                }
                else
                {
                    throw new NotImplementedException("");
                }
            }


            return Tuple.Create(ok, result);
        }

        private delegate bool TryParseDelegate<T>(string str, out T result);

        private static Func<string, Tuple<bool, object>> MakeConvMethod<T>(TryParseDelegate<T> m)
        {
            return s =>
            {
                T x;
                return m(s, out x) ? Tuple.Create(true, (object)x) : Tuple.Create(false, (object)null);
            };
        }
    }

    public class TextBodyWriter : BodyWriter
    {
        readonly byte[] _messageBytes;

        public TextBodyWriter(byte[] data)
            : base(true)
        {
            _messageBytes = data;
        }

        public TextBodyWriter(string message)
            : base(true)
        {
            _messageBytes = Encoding.UTF8.GetBytes(message);
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            writer.WriteStartElement("Binary");
            writer.WriteBase64(_messageBytes, 0, _messageBytes.Length);
            writer.WriteEndElement();
        }
    }

}
