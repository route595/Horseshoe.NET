using System;
using System.Collections.Generic;
using System.Linq;
using Horseshoe.NET.Collections;
using Horseshoe.NET.Objects;

namespace Horseshoe.NET.Http
{
    /// <summary>
    /// A robust, serializable web API response that can help overcome the limitations of cross-HTTP exception handling.
    /// Use type parameters for deserialization (e.g. <c>Get.Json&lt;WebServiceResponse&lt;foo&gt;&gt;</c>).
    /// </summary>
    /// <remarks>
    /// By catching and returning exceptions in a normal (HTTP 200) resonse the exception details are guaranteed 
    /// to be preserved.  In some cases, uncaught ASP.NET exceptions produce an HTML exception page which is great unless 
    /// the receiver is expecting JSON.  In other cases, a server-side error is manifest as a no detail HTTP 500 error.  
    /// In general, API calls (e.g. AJAX) are easier to handle if the return type is consistent. 
    /// </remarks>
    public class WebServiceResponse : WebServiceResponse<object>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public WebServiceResponse()
        {
        }

        /// <summary>
        /// Constructor for a normal response
        /// </summary>
        /// <param name="data">An object or collection</param>
        public WebServiceResponse(object data, params string[] messages) : base (data, messages)
        {
        }

        /// <summary>
        /// Constructor for an error type response
        /// </summary>
        /// <param name="ex">An exception</param>
        public WebServiceResponse(Exception ex, params string[] messages) : base(ex, messages)
        {
        }

        /// <summary>
        /// Factory method for photocopying a response
        /// </summary>
        /// <param name="response">A response</param>
        public static WebServiceResponse From<E>(WebServiceResponse<E> response, params string[] messages)
        {
            var newInstance = new WebServiceResponse();
            ObjectUtil.MapProperties(response, newInstance);
            if (messages != null)
            {
                var list = new List<string>(newInstance.Messages ?? new string[] { });
                list.AddRange(messages);
                newInstance.Messages = list;
            }
            return newInstance;
        }
    }

    /// <summary>
    /// A robust, serializable web API response that can help overcome the limitations of cross-HTTP exception handling.
    /// Use type parameters for deserialization (e.g. <c>WebService.GetJson&lt;WebServiceResponse&lt;foo&gt;&gt;</c>).
    /// </summary>
    /// <remarks>
    /// By catching and returning exceptions in a normal (HTTP 200) resonse the exception details are guaranteed 
    /// to be preserved.  In some cases, uncaught ASP.NET exceptions produce an HTML exception page which is great unless 
    /// the receiver is expecting JSON.  In other cases, a server-side error is manifest as a no detail HTTP 500 error.  
    /// In general, API calls (e.g. AJAX) are easier to handle if the return type is consistent. 
    /// </remarks>
    /// <typeparam name="E">The deserialized type</typeparam>
    public class WebServiceResponse<E>
    {
        /// <summary>
        /// The data to return to the caller (will typically be JSONified)
        /// </summary>
        public E Data { get; set; }

        /// <summary>
        /// The serializable error information to return in case of a catchable server-side exception
        /// </summary>
        public ExceptionInfo Exception { get; set; }

        /// <summary>
        /// The status (i.e. Ok, Error) of this response (renders as int when serialized)
        /// </summary>
        public WebServiceResponseStatus Status { get; set; } = WebServiceResponseStatus.Ok;

        /// <summary>
        /// The status (i.e. Ok, Error) of this response (renders as text when serialized)
        /// </summary>
        public string StatusText => Status.ToString();

        /// <summary>
        /// Use this to easily pass informational messages for troubleshooting purposes e.g. comments, warnings, collection counts, etc.
        /// </summary>
        /// <remarks>
        /// <para>
        /// By convention, prepend each message with an uppercase tag.
        /// </para>
        /// <para>
        /// For instance...
        /// <code>
        /// return new WebServiceResponse(data) { Messages = new[] { "INFO: This in an excerpt from 'Lorem Ipsum'") } };
        /// </code>
        /// ... or ...
        /// <code>
        /// return new WebServiceResponse(data, "INFO: This in an excerpt from 'Lorem Ipsum'");
        /// </code>
        /// ... or ...
        /// <code>
        /// return new WebServiceResponse(list) { Messages = new[] { "COUNT: " + list.Count(), "INFO: Remember kids, C# collections never use drugs!" } };
        /// </code>
        /// ... or ...
        /// <code>
        /// return new WebServiceResponse(list, "COUNT: " + list.Count(), "INFO: Remember kids, C# collections never use drugs!");
        /// </code>
        /// </para>
        /// </remarks>
        public IEnumerable<string> Messages { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public WebServiceResponse()
        {
        }

        /// <summary>
        /// Constructor for a normal response (prefer the constructor without type parameters)
        /// </summary>
        /// <param name="data">An object or collection</param>
        /// <param name="messages">Messages (e.g. debug info, metadata, list count, etc.)</param>
        public WebServiceResponse(E data, params string[] messages)
        {
            Data = data;
            Messages = messages;
        }

        /// <summary>
        /// Constructor for an error type response (prefer the constructor without type parameters)
        /// </summary>
        /// <param name="ex">An exception</param>
        /// <param name="messages">Messages (e.g. debug info, metadata, list count, etc.)</param>
        public WebServiceResponse(Exception ex, params string[] messages)
        {
            Exception = ExceptionInfo.From(ex);
            Status = WebServiceResponseStatus.Error;
            Messages = messages;
        }

        /// <summary>
        /// Constructor for creating a duplicate response
        /// </summary>
        /// <param name="response">A response</param>
        /// <param name="messages">Messages (e.g. debug info, metadata, list count, etc.)</param>
        public WebServiceResponse(WebServiceResponse<E> response, params string[] messages)
        {
            Data = response.Data;
            Exception = response.Exception;
            Status = response.Status;
            Messages = messages == null || !messages.Any()
                ? response.Messages
                : CollectionUtil.Combine(response.Messages, messages);
        }

        /// <summary>
        /// Creates a <c>WebServiceException</c> from the exception info in a <c>WebServiceResponse</c> if and only if the status is <c>Error</c>
        /// </summary>
        /// <param name="typeRendering">If <c>Fqn</c> (default) displays the fully qualified type of the exception</param>
        /// <param name="includeStackTrace">If <c>true</c> renders the stack trace (default is <c>true</c>)</param>
        /// <param name="indent">Number of spaces to indent detail lines in the output</param>
        /// <param name="recursive">If <c>true</c> renders inner exceptions (default is <c>false</c>)</param>
        /// <returns>A <c>WebServiceException</c></returns>
        /// <seealso cref="WebServiceException"/>
        public Exception GetReconstitutedException(ExceptionTypeRenderingPolicy typeRendering = default, bool includeStackTrace = false, int indent = 2, bool recursive = false)
        {
            if (Status != WebServiceResponseStatus.Error) 
                return null;
            return Exception != null
                ? WebServiceException.From(Exception, typeRendering: typeRendering, includeStackTrace: includeStackTrace, indent: indent, recursive: recursive)
                : new Exception("The last call resulted in an error status but no exception info was found");
        }
    }
}