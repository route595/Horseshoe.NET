using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.Bootstrap
{
    public static class Bootstrap3
    {
        public enum AlertType
        {
            Info,
            Success,
            Warning,
            Danger,
            Error
        }

        /* Ref: http://getbootstrap.com/docs/3.3/components/#alerts */
        public class Alert
        {
            bool? _closeable;
            public AlertType AlertType { get; set; }
            public string Message { get; set; }
            public string Emphasis { get; set; }
            public bool Closeable { get { return _closeable ?? BootstrapSettings.DefaultAutoCloseable; } set { _closeable = value; } }
            public bool EncodeHtml { get; set; }
            public string MessageDetails { get; set; }
            public AlertMessageDetailsRenderingPolicy MessageDetailsRendering { get; set; }
            public bool IsMessageDetailsEncodeHtml => (MessageDetailsRendering & AlertMessageDetailsRenderingPolicy.EncodeHtml) == AlertMessageDetailsRenderingPolicy.EncodeHtml;
            public bool IsMessageDetailsPreFormatted => (MessageDetailsRendering & AlertMessageDetailsRenderingPolicy.PreFormatted) == AlertMessageDetailsRenderingPolicy.PreFormatted;
            public bool IsMessageDetailsHidden => (MessageDetailsRendering & AlertMessageDetailsRenderingPolicy.Hidden) == AlertMessageDetailsRenderingPolicy.Hidden;
        }

        public static Alert CreateAlert
        (
            AlertType alertType,
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool? closeable = null,
            bool encodeHtml = false,
            string messageDetails = null,
            AlertMessageDetailsRenderingPolicy messageDetailsRendering = default
        )
        {
            var alert = new Alert
            {
                AlertType = alertType,
                Message = message,
                Emphasis = emphasis ?? (autoEmphasis ? alertType.ToString() : null),
                EncodeHtml = encodeHtml,
                MessageDetails = messageDetails,
                MessageDetailsRendering = messageDetailsRendering
            };
            if (closeable.HasValue) alert.Closeable = closeable.Value;
            return alert;
        }

        public static Alert CreateInfoAlert
        (
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool? closeable = null,
            bool encodeHtml = false,
            string messageDetails = null,
            AlertMessageDetailsRenderingPolicy messageDetailsRendering = default
        )
        {
            return CreateAlert
            (
                AlertType.Info,
                message,
                emphasis: emphasis,
                autoEmphasis: autoEmphasis,
                closeable: closeable,
                encodeHtml: encodeHtml,
                messageDetails: messageDetails,
                messageDetailsRendering: messageDetailsRendering
            );
        }

        public static Alert CreateSuccessAlert
        (
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool? closeable = null,
            bool encodeHtml = false,
            string messageDetails = null,
            AlertMessageDetailsRenderingPolicy messageDetailsRendering = default
        )
        {
            return CreateAlert
            (
                AlertType.Success,
                message,
                emphasis: emphasis,
                autoEmphasis: autoEmphasis,
                closeable: closeable,
                encodeHtml: encodeHtml,
                messageDetails: messageDetails,
                messageDetailsRendering: messageDetailsRendering
            );
        }

        public static Alert CreateWarningAlert
        (
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool? closeable = null,
            bool encodeHtml = false,
            string messageDetails = null,
            AlertMessageDetailsRenderingPolicy messageDetailsRendering = default
        )
        {
            return CreateAlert
            (
                AlertType.Warning,
                message,
                emphasis: emphasis,
                autoEmphasis: autoEmphasis,
                closeable: closeable,
                encodeHtml: encodeHtml,
                messageDetails: messageDetails,
                messageDetailsRendering: messageDetailsRendering
            );
        }

        public static Alert CreateDangerAlert
        (
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool? closeable = null,
            bool encodeHtml = false,
            string messageDetails = null,
            AlertMessageDetailsRenderingPolicy messageDetailsRendering = default
        )
        {
            return CreateAlert
            (
                AlertType.Danger,
                message,
                emphasis: emphasis,
                autoEmphasis: autoEmphasis,
                closeable: closeable,
                encodeHtml: encodeHtml,
                messageDetails: messageDetails,
                messageDetailsRendering: messageDetailsRendering
            );
        }

        public static Alert CreateErrorAlert
        (
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool? closeable = null,
            bool encodeHtml = false,
            string messageDetails = null,
            AlertMessageDetailsRenderingPolicy messageDetailsRendering = default
        )
        {
            return CreateAlert
            (
                AlertType.Error,
                message,
                emphasis: emphasis,
                autoEmphasis: autoEmphasis,
                closeable: closeable,
                encodeHtml: encodeHtml,
                messageDetails: messageDetails,
                messageDetailsRendering: messageDetailsRendering
            );
        }

        public static Alert CreateErrorAlert
        (
            Exception exception,
            string emphasis = null,
            bool autoEmphasis = true,
            bool? closeable = null,
            bool encodeHtml = true,
            AlertType? alertType = null,
            ExceptionTypeRenderingPolicy typeRendering = default,
            bool includeStackTrace = false,
            int indent = 2,
            bool recursive = false,
            ExceptionRenderingPolicy? exceptionRendering = null
        )
        {
            return CreateErrorAlert
            (
                ExceptionInfo.From(exception),
                emphasis: emphasis,
                autoEmphasis: autoEmphasis,
                closeable: closeable,
                encodeHtml: encodeHtml,
                alertType: alertType,
                typeRendering: typeRendering,
                includeStackTrace: includeStackTrace,
                indent: indent,
                recursive: recursive,
                exceptionRendering: exceptionRendering
            );
        }

        public static Alert CreateErrorAlert
        (
            ExceptionInfo exception,
            string emphasis = null,
            bool autoEmphasis = true,
            bool? closeable = null,
            bool encodeHtml = true,
            AlertType? alertType = null,
            ExceptionTypeRenderingPolicy typeRendering = default,
            bool includeStackTrace = false,
            int indent = 2,
            bool recursive = false,
            ExceptionRenderingPolicy? exceptionRendering = null
        )
        {
            var resultantErrorRendering = exceptionRendering ?? BootstrapSettings.DefaultExceptionRendering;
            return CreateAlert
            (
                alertType ?? AlertType.Error,
                exception?.Message ?? "[null]",
                emphasis: emphasis,
                autoEmphasis: autoEmphasis,
                closeable: closeable,
                encodeHtml: encodeHtml,
                messageDetails: resultantErrorRendering != default
                    ? exception?.Render(typeRendering: typeRendering, includeStackTrace: includeStackTrace, indent: indent, recursive: recursive)
                    : null,
                messageDetailsRendering: resultantErrorRendering.ToAlertMessageDetailsRendering()
            );
        }

    }
}
