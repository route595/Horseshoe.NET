using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Horseshoe.NET.IO;
using Horseshoe.NET.RelayMessages;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Http
{
    internal static class WebResponseFactory
    {
        public static string MessageRelayGroup => HttpConstants.MessageRelayGroup;

        internal static string ProcessResponse
        (
            HttpWebRequest request, 
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            WebResponse response;
            try
            {
                SystemMessageRelay.RelayMessage("request.GetResponse()", group: MessageRelayGroup);
                response = request.GetResponse();
                SystemMessageRelay.RelayMessage("success", group: MessageRelayGroup);
            }
            catch (WebException wex)
            {
                if (wex.Response is HttpWebResponse errorResponse)
                {
                    SystemMessageRelay.RelayMessage("error: HTTP " + (int)errorResponse.StatusCode + " (" + errorResponse.StatusDescription + ")", group: MessageRelayGroup);
                }
                else
                {
                    SystemMessageRelay.RelayMessage("error: " + wex.RenderMessage(), group: MessageRelayGroup);
                }
                SystemMessageRelay.RelayException(wex, group: MessageRelayGroup);
                throw;
            }
            catch (Exception ex)
            {
                SystemMessageRelay.RelayException(ex, group: MessageRelayGroup);
                throw;
            }

            // give client code the first stab at the response
            if (handleResponse != null)
            {
                SystemMessageRelay.RelayMessage("client is handling response...", group: MessageRelayGroup);
                var consumerResponse = new ConsumerResponseEnvelope();
                handleResponse.Invoke(response as HttpWebResponse, consumerResponse);
                SystemMessageRelay.RelayMessage("consumerResponse.Flag = " + consumerResponse.Flag, group: MessageRelayGroup);
                if ((consumerResponse.Flag & ConsumerResponse.SuppressFurtherResponseHandling) == ConsumerResponse.SuppressFurtherResponseHandling)
                {
                    SystemMessageRelay.RelayMethodReturnValue(string.Empty, group: MessageRelayGroup);
                    return string.Empty;
                }
                if ((consumerResponse.Flag & ConsumerResponse.ResetResponseStream) == ConsumerResponse.ResetResponseStream)
                {
                    var stream = response.GetResponseStream();
                    if (stream.CanSeek)
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                    }
                    else
                    {
                        var ex = new Exception("Cannot seek non-seek stream");
                        SystemMessageRelay.RelayException(ex, group: MessageRelayGroup);
                        throw ex;
                    }
                }
            }

            // process response stream
            try
            {
                SystemMessageRelay.RelayMessage("response.GetContentAsString()", group: MessageRelayGroup);
                var stringResult = response.GetContentAsString();
                SystemMessageRelay.RelayMethodReturn(returnDescription: FileUtil.GetDisplayFileSize(stringResult.Length), group: MessageRelayGroup);
                return stringResult;
            }
            catch (Exception ex)
            {
                SystemMessageRelay.RelayException(ex, group: MessageRelayGroup);
                throw;
            }
        }

        public static async Task<string> ProcessResponseAsync
        (
            HttpWebRequest request, 
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse
        )
        {
            SystemMessageRelay.RelayMethodInfo(group: MessageRelayGroup);

            WebResponse response;
            try
            {
                SystemMessageRelay.RelayMessage("request.GetResponse()", group: MessageRelayGroup);
                response = request.GetResponse();
                SystemMessageRelay.RelayMessage("success", group: MessageRelayGroup);
            }
            catch (WebException wex)
            {
                if (wex.Response is HttpWebResponse errorResponse)
                {
                    SystemMessageRelay.RelayMessage("error: HTTP " + (int)errorResponse.StatusCode + " (" + errorResponse.StatusDescription + ")", group: MessageRelayGroup);
                }
                else
                {
                    SystemMessageRelay.RelayMessage("error: " + wex.RenderMessage(), group: MessageRelayGroup);
                }
                SystemMessageRelay.RelayException(wex, group: MessageRelayGroup);
                throw;
            }
            catch (Exception ex)
            {
                SystemMessageRelay.RelayException(ex, group: MessageRelayGroup);
                throw;
            }

            // give client code the first stab at the response
            if (handleResponse != null)
            {
                SystemMessageRelay.RelayMessage("client is handling response...", group: MessageRelayGroup);
                var consumerResponse = new ConsumerResponseEnvelope();
                handleResponse.Invoke(response as HttpWebResponse, consumerResponse);
                SystemMessageRelay.RelayMessage("consumerResponse.Flag = " + consumerResponse.Flag, group: MessageRelayGroup);
                if ((consumerResponse.Flag & ConsumerResponse.SuppressFurtherResponseHandling) == ConsumerResponse.SuppressFurtherResponseHandling)
                {
                    SystemMessageRelay.RelayMethodReturnValue(string.Empty, group: MessageRelayGroup);
                    return string.Empty;
                }
                if ((consumerResponse.Flag & ConsumerResponse.ResetResponseStream) == ConsumerResponse.ResetResponseStream)
                {
                    var stream = response.GetResponseStream();
                    if (stream.CanSeek)
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                    }
                    else
                    {
                        var ex = new Exception("Cannot seek non-seek stream");
                        SystemMessageRelay.RelayException(ex, group: MessageRelayGroup);
                        throw ex;
                    }
                }
            }

            // process response stream
            try
            {
                SystemMessageRelay.RelayMessage("response.GetContentAsStringAsync()", group: MessageRelayGroup);
                var stringResult = await response.GetContentAsStringAsync();
                SystemMessageRelay.RelayMethodReturn(returnDescription: FileUtil.GetDisplayFileSize(stringResult.Length), group: MessageRelayGroup);
                return stringResult;
            }
            catch (Exception ex)
            {
                SystemMessageRelay.RelayException(ex, group: MessageRelayGroup);
                throw;
            }
        }

        internal static Func<string, E> GetJsonDeserializer<E>(bool zapBackingFields)
        {
            if (zapBackingFields)
            {
                return (json) => Deserialize.Json<E>(json, onBeforeDeserialize: WebServiceUtil.ZapBackingFields);
            }
            return (json) => Deserialize.Json<E>(json);
        }
    }
}
