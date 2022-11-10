using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.Bootstrap
{
    public static class Extensions
    {
        public static string ToCssClass(this Bootstrap3.AlertType alertType)
        {
            switch (alertType)
            {
                case Bootstrap3.AlertType.Error:
                    return "alert-danger";
                default:
                    return "alert-" + alertType.ToString().ToLower();
            }
        }

        public static string ToCssClass(this Bootstrap4.AlertType alertType)
        {
            switch (alertType)
            {
                case Bootstrap4.AlertType.Error:
                    return "alert-danger";
                default:
                    return "alert-" + alertType.ToString().ToLower();
            }
        }

        internal static AlertMessageDetailsRenderingPolicy ToAlertMessageDetailsRendering(this ExceptionRenderingPolicy exceptionRendering)
        {
            switch (exceptionRendering)
            {
                case ExceptionRenderingPolicy.Visible:
                    return AlertMessageDetailsRenderingPolicy.EncodeHtml | AlertMessageDetailsRenderingPolicy.PreFormatted;
                case ExceptionRenderingPolicy.Hidden:
                default:
                    return AlertMessageDetailsRenderingPolicy.Default;
            }
        }
    }
}
