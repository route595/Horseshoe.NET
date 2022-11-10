using System;
using System.Collections.Generic;
using System.Linq;

using Horseshoe.NET.Collections;

namespace Horseshoe.NET.Http.ReportingServices
{
    public class ReportException : Exception
    {
        private IDictionary<string, object> _parameters;

        public bool BulkDismissable { get; set; }

        public IDictionary<string, object> Parameters
        {
            get { return _parameters; }
            set { _parameters = value != null ? new Dictionary<string, object>(value) : null; }
        }

        public bool HasParameters => Parameters != null && Parameters.Any();

        public override string Message => base.Message + (HasParameters ? " - Parameters: " + Parameters.StringDump(equals: "=", separator: ";") : "");

        public ReportException(string message) : base(message)
        {
        }

        public ReportException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
