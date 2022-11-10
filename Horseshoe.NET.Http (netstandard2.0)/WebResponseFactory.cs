using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Horseshoe.NET.IO;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Http
{
    internal static class WebResponseFactory
    {
        internal static string ProcessResponse
        (
            HttpWebRequest request, 
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse, 
            TraceJournal journal
        )
        {
            journal.WriteEntry("WebResponseFactory.ProcessResponse()");
            journal.Level++;
            WebResponse response;
            try
            {
                journal.WriteEntry("WebResponse.GetResponse()");
                response = request.GetResponse();
                journal.WriteEntry(" -> success");
            }
            catch (WebException wex)
            {
                journal.WriteEntry(" -> error");
                if (wex.Response is HttpWebResponse errorResponse)
                {
                    journal.WriteEntry("    HTTP " + (int)errorResponse.StatusCode + " (" + errorResponse.StatusDescription + ") - " + wex.Message);
                }
                else
                {
                    journal.WriteEntry("    " + wex.RenderMessage());
                }
                journal.Level--;
                throw;
            }
            catch (Exception ex)
            {
                journal.WriteEntry(" -> error");
                journal.WriteEntry("    " + ex.RenderMessage());
                journal.Level--;
                throw;
            }

            // give client code the first stab at the response
            if (handleResponse != null)
            {
                journal.WriteEntry("handleResponse.Invoke()");
                var consumerResponse = new ConsumerResponseEnvelope();
                handleResponse.Invoke(response as HttpWebResponse, consumerResponse);
                journal.WriteEntry(" -> { Flag = " + consumerResponse.Flag + " }");
                if ((consumerResponse.Flag & ConsumerResponse.SuppressFurtherResponseHandling) == ConsumerResponse.SuppressFurtherResponseHandling)
                {
                    journal.Level--;
                    return "";
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
                        journal.Level--;
                        throw new Exception("Cannot seek non-seek stream");
                    }
                }
            }

            // process response stream
            journal.WriteEntry("WebResponse.GetContentAsString()");
            try
            {
                var stringResult = response.GetContentAsString();
                journal.WriteEntry(" -> size = " + FileUtil.GetDisplayFileSize(stringResult.Length));
                return stringResult;
            }
            catch (Exception ex)
            {
                journal.WriteEntry(" -> error");
                journal.WriteEntry("    " + ex.RenderMessage());
                throw;
            }
            finally
            {
                journal.Level--;
            }
        }

        public static async Task<string> ProcessResponseAsync
        (
            HttpWebRequest request, 
            Action<HttpWebResponse, ConsumerResponseEnvelope> handleResponse,
            TraceJournal journal
        )
        {
            journal.WriteEntry("WebResponseFactory.ProcessResponseAsync()");
            journal.Level++;
            WebResponse response;
            try
            {
                journal.WriteEntry("WebResponse.GetResponse()");
                response = request.GetResponse();
                journal.WriteEntry(" -> success");
            }
            catch (WebException wex)
            {
                journal.WriteEntry(" -> error");
                if (wex.Response is HttpWebResponse errorResponse)
                {
                    journal.WriteEntry("    HTTP " + (int)errorResponse.StatusCode + " (" + errorResponse.StatusDescription + ") - " + wex.Message);
                }
                else
                {
                    journal.WriteEntry("    " + wex.RenderMessage());
                }
                journal.Level--;
                throw;
            }
            catch (Exception ex)
            {
                journal.WriteEntry(" -> error");
                journal.WriteEntry("    " + ex.RenderMessage());
                journal.Level--;
                throw;
            }

            // give client code the first stab at the response
            if (handleResponse != null)
            {
                journal.WriteEntry("handleResponse.Invoke()");
                var consumerResponse = new ConsumerResponseEnvelope();
                handleResponse.Invoke(response as HttpWebResponse, consumerResponse);
                journal.WriteEntry(" -> { Flag = " + consumerResponse.Flag + " }");
                if ((consumerResponse.Flag & ConsumerResponse.SuppressFurtherResponseHandling) == ConsumerResponse.SuppressFurtherResponseHandling)
                {
                    journal.Level--;
                    return "";
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
                        journal.Level--;
                        throw new Exception("Cannot seek non-seek stream");
                    }
                }
            }

            // process response stream
            journal.WriteEntry("WebResponse.GetContentAsString()");
            try
            {
                var stringResult = await response.GetContentAsStringAsync();
                journal.WriteEntry(" -> size = " + FileUtil.GetDisplayFileSize(stringResult.Length));
                return stringResult;
            }
            catch (Exception ex)
            {
                journal.WriteEntry(" -> error");
                journal.WriteEntry("    " + ex.RenderMessage());
                throw;
            }
            finally
            {
                journal.Level--;
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
