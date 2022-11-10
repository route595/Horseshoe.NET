using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Horseshoe.NET.Http
{
    /// <summary>
    /// Basic Web API response status
    /// </summary>
    public enum WebServiceResponseStatus
    {
        /// <summary>
        /// Indicates an exception has occurred
        /// </summary>
        Error, 

        /// <summary>
        /// Indicates a normal response
        /// </summary>
        Ok
    }
}