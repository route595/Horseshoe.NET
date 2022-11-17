using System;

namespace Horseshoe.NET.Bootstrap
{
    /// <summary>
    /// Classes and methods for creating Bootstrap v4 alerts
    /// </summary>
    public static class Bootstrap4
    {
        /// <summary>
        /// Alert type corresponds to CSS class used to render different styles of Bootstrap alert
        /// </summary>
        public enum AlertType
        {
            /// <summary>
            /// Informational alert
            /// </summary>
            Info,

            /// <summary>
            /// Success alert
            /// </summary>
            Success,

            /// <summary>
            /// Warning alert
            /// </summary>
            Warning,

            /// <summary>
            /// Danger alert
            /// </summary>
            Danger,

            /// <summary>
            /// Error alert
            /// </summary>
            Error,

            /// <summary>
            /// Primary alert
            /// </summary>
            Primary,

            /// <summary>
            /// Secondary alert
            /// </summary>
            Secondary,

            /// <summary>
            /// Light alert
            /// </summary>
            Light,

            /// <summary>
            /// Dark alert
            /// </summary>
            Dark
        }

        /// <summary>
        /// Encapsulates customizable aspects of Bootstrap alerts
        /// </summary>
        /// <remarks>ref: https://getbootstrap.com/docs/4.1/components/alerts/</remarks>
        public class Alert
        {
            bool? _closeable;

            /// <summary>
            /// Alert type corresponds to CSS class used in Bootstrap alert
            /// </summary>
            public AlertType AlertType { get; set; }

            /// <summary>
            /// Alert message
            /// </summary>
            public string Message { get; set; }

            /// <summary>
            /// Boldface text to prepend to the message
            /// </summary>
            public string Emphasis { get; set; }

            /// <summary>
            /// Whether to render an alert closing widget
            /// </summary>
            public bool Closeable { get { return _closeable ?? BootstrapSettings.DefaultAutoCloseableAlerts; } set { _closeable = value; } }

            /// <summary>
            /// Whether to escape HTML markup for display or let it render naturally
            /// </summary>
            public bool EncodeHtml { get; set; }

            /// <summary>
            /// Whether alert should attempt to fade in and out
            /// </summary>
            /// <remarks><see cref="MessageDetailsRendering"/></remarks>
            public bool Fade { get; set; }

            /// <summary>
            /// Whether alert should be immediately visible
            /// </summary>
            /// <remarks><see cref="MessageDetailsRendering"/></remarks>
            public bool Show { get; set; }

            /// <summary>
            /// Extra text to reinforce the message, such as the stack trace for an <c>Exception</c> error alert
            /// </summary>
            public string MessageDetails { get; set; }

            /// <summary>
            /// Hints for rendering message details in Bootstrap alerts
            /// </summary>
            public AlertMessageDetailsRenderingPolicy MessageDetailsRendering { get; set; }

            /// <summary>
            /// Whether message details should be HTML encoded
            /// </summary>
            /// <remarks><see cref="MessageDetailsRendering"/></remarks>
            public bool IsMessageDetailsEncodeHtml => (MessageDetailsRendering & AlertMessageDetailsRenderingPolicy.EncodeHtml) == AlertMessageDetailsRenderingPolicy.EncodeHtml;

            /// <summary>
            /// Whether message details should be &lt;pre&gt; formatted
            /// </summary>
            /// <remarks><see cref="MessageDetailsRendering"/></remarks>
            public bool IsMessageDetailsPreFormatted => (MessageDetailsRendering & AlertMessageDetailsRenderingPolicy.PreFormatted) == AlertMessageDetailsRenderingPolicy.PreFormatted;

            /// <summary>
            /// Whether message details should be rendered to a hidden element
            /// </summary>
            /// <remarks><see cref="MessageDetailsRendering"/></remarks>
            public bool IsMessageDetailsHidden => (MessageDetailsRendering & AlertMessageDetailsRenderingPolicy.Hidden) == AlertMessageDetailsRenderingPolicy.Hidden;
        }

        /// <summary>
        /// Creates an <c>Alert</c> object useful for rendering Bootstrap alerts
        /// </summary>
        /// <param name="alertType">alert type</param>
        /// <param name="message">message</param>
        /// <param name="emphasis">boldface text to prepend to the message</param>
        /// <param name="autoEmphasis">add alert type as <c>emphasis</c></param>
        /// <param name="closeable">where to add widget for user to close alert</param>
        /// <param name="fade">whether alert should attempt to fade in and out</param>
        /// <param name="show">whether alert should be immediately visible</param>
        /// <param name="encodeHtml">whether to escape HTML markup for display or let it render naturally</param>
        /// <param name="messageDetails">extra text to reinforce the message, such as the stack trace for an <c>Exception</c> error alert</param>
        /// <param name="messageDetailsRendering">hints for rendering message details in Bootstrap alerts</param>
        /// <returns>an <c>Alert</c> object</returns>
        public static Alert CreateAlert
        (
            AlertType alertType,
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool? closeable = null,
            bool encodeHtml = false,
            bool fade = true,
            bool show = true,
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
                Fade = fade,
                Show = show,
                MessageDetails = messageDetails,
                MessageDetailsRendering = messageDetailsRendering
            };
            if (closeable.HasValue) alert.Closeable = closeable.Value;
            return alert;
        }

        /// <summary>
        /// Creates an <c>Alert</c> object useful for rendering Bootstrap 'info' alerts
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="emphasis">boldface text to prepend to the message</param>
        /// <param name="autoEmphasis">add alert type as <c>emphasis</c></param>
        /// <param name="closeable">where to add widget for user to close alert</param>
        /// <param name="fade">whether alert should attempt to fade in and out</param>
        /// <param name="show">whether alert should be immediately visible</param>
        /// <param name="encodeHtml">whether to escape HTML markup for display or let it render naturally</param>
        /// <param name="messageDetails">extra text to reinforce the message, such as the stack trace for an <c>Exception</c> error alert</param>
        /// <param name="messageDetailsRendering">hints for rendering message details in Bootstrap alerts</param>
        /// <returns>an <c>Alert</c> object</returns>
        public static Alert CreateInfoAlert
        (
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool? closeable = null,
            bool encodeHtml = false,
            bool fade = true,
            bool show = true,
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
                fade: fade,
                show: show,
                messageDetails: messageDetails,
                messageDetailsRendering: messageDetailsRendering
            );
        }

        /// <summary>
        /// Creates an <c>Alert</c> object useful for rendering Bootstrap 'success' alerts
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="emphasis">boldface text to prepend to the message</param>
        /// <param name="autoEmphasis">add alert type as <c>emphasis</c></param>
        /// <param name="closeable">where to add widget for user to close alert</param>
        /// <param name="fade">whether alert should attempt to fade in and out</param>
        /// <param name="show">whether alert should be immediately visible</param>
        /// <param name="encodeHtml">whether to escape HTML markup for display or let it render naturally</param>
        /// <param name="messageDetails">extra text to reinforce the message, such as the stack trace for an <c>Exception</c> error alert</param>
        /// <param name="messageDetailsRendering">hints for rendering message details in Bootstrap alerts</param>
        /// <returns>an <c>Alert</c> object</returns>
        public static Alert CreateSuccessAlert
        (
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool? closeable = null,
            bool encodeHtml = false,
            bool fade = true,
            bool show = true,
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
                fade: fade,
                show: show,
                messageDetails: messageDetails,
                messageDetailsRendering: messageDetailsRendering
            );
        }

        /// <summary>
        /// Creates an <c>Alert</c> object useful for rendering Bootstrap 'warning' alerts
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="emphasis">boldface text to prepend to the message</param>
        /// <param name="autoEmphasis">add alert type as <c>emphasis</c></param>
        /// <param name="closeable">where to add widget for user to close alert</param>
        /// <param name="fade">whether alert should attempt to fade in and out</param>
        /// <param name="show">whether alert should be immediately visible</param>
        /// <param name="encodeHtml">whether to escape HTML markup for display or let it render naturally</param>
        /// <param name="messageDetails">extra text to reinforce the message, such as the stack trace for an <c>Exception</c> error alert</param>
        /// <param name="messageDetailsRendering">hints for rendering message details in Bootstrap alerts</param>
        /// <returns>an <c>Alert</c> object</returns>
        public static Alert CreateWarningAlert
        (
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool? closeable = null,
            bool encodeHtml = false,
            bool fade = true,
            bool show = true,
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
                fade: fade,
                show: show,
                encodeHtml: encodeHtml,
                messageDetails: messageDetails,
                messageDetailsRendering: messageDetailsRendering
            );
        }

        /// <summary>
        /// Creates an <c>Alert</c> object useful for rendering Bootstrap 'danger' alerts
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="emphasis">boldface text to prepend to the message</param>
        /// <param name="autoEmphasis">add alert type as <c>emphasis</c></param>
        /// <param name="closeable">where to add widget for user to close alert</param>
        /// <param name="fade">whether alert should attempt to fade in and out</param>
        /// <param name="show">whether alert should be immediately visible</param>
        /// <param name="encodeHtml">whether to escape HTML markup for display or let it render naturally</param>
        /// <param name="messageDetails">extra text to reinforce the message, such as the stack trace for an <c>Exception</c> error alert</param>
        /// <param name="messageDetailsRendering">hints for rendering message details in Bootstrap alerts</param>
        /// <returns>an <c>Alert</c> object</returns>
        public static Alert CreateDangerAlert
        (
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool? closeable = null,
            bool encodeHtml = false,
            bool fade = true,
            bool show = true,
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
                fade: fade,
                show: show,
                messageDetails: messageDetails,
                messageDetailsRendering: messageDetailsRendering
            );
        }

        /// <summary>
        /// Creates an <c>Alert</c> object useful for rendering Bootstrap 'primary' alerts
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="emphasis">boldface text to prepend to the message</param>
        /// <param name="autoEmphasis">add alert type as <c>emphasis</c></param>
        /// <param name="closeable">where to add widget for user to close alert</param>
        /// <param name="fade">whether alert should attempt to fade in and out</param>
        /// <param name="show">whether alert should be immediately visible</param>
        /// <param name="encodeHtml">whether to escape HTML markup for display or let it render naturally</param>
        /// <param name="messageDetails">extra text to reinforce the message, such as the stack trace for an <c>Exception</c> error alert</param>
        /// <param name="messageDetailsRendering">hints for rendering message details in Bootstrap alerts</param>
        /// <returns>an <c>Alert</c> object</returns>
        public static Alert CreatePrimaryAlert
        (
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool? closeable = null,
            bool encodeHtml = false,
            bool fade = true,
            bool show = true,
            string messageDetails = null,
            AlertMessageDetailsRenderingPolicy messageDetailsRendering = default
        )
        {
            return CreateAlert
            (
                AlertType.Primary,
                message,
                emphasis: emphasis,
                autoEmphasis: autoEmphasis,
                closeable: closeable,
                encodeHtml: encodeHtml,
                fade: fade,
                show: show,
                messageDetails: messageDetails,
                messageDetailsRendering: messageDetailsRendering
            );
        }

        /// <summary>
        /// Creates an <c>Alert</c> object useful for rendering Bootstrap 'secondary' alerts
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="emphasis">boldface text to prepend to the message</param>
        /// <param name="autoEmphasis">add alert type as <c>emphasis</c></param>
        /// <param name="closeable">where to add widget for user to close alert</param>
        /// <param name="fade">whether alert should attempt to fade in and out</param>
        /// <param name="show">whether alert should be immediately visible</param>
        /// <param name="encodeHtml">whether to escape HTML markup for display or let it render naturally</param>
        /// <param name="messageDetails">extra text to reinforce the message, such as the stack trace for an <c>Exception</c> error alert</param>
        /// <param name="messageDetailsRendering">hints for rendering message details in Bootstrap alerts</param>
        /// <returns>an <c>Alert</c> object</returns>
        public static Alert CreateSecondaryAlert
        (
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool? closeable = null,
            bool encodeHtml = false,
            bool fade = true,
            bool show = true,
            string messageDetails = null,
            AlertMessageDetailsRenderingPolicy messageDetailsRendering = default
        )
        {
            return CreateAlert
            (
                AlertType.Secondary,
                message,
                emphasis: emphasis,
                autoEmphasis: autoEmphasis,
                closeable: closeable,
                encodeHtml: encodeHtml,
                fade: fade,
                show: show,
                messageDetails: messageDetails,
                messageDetailsRendering: messageDetailsRendering
            );
        }

        /// <summary>
        /// Creates an <c>Alert</c> object useful for rendering Bootstrap 'dark' alerts
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="emphasis">boldface text to prepend to the message</param>
        /// <param name="autoEmphasis">add alert type as <c>emphasis</c></param>
        /// <param name="closeable">where to add widget for user to close alert</param>
        /// <param name="fade">whether alert should attempt to fade in and out</param>
        /// <param name="show">whether alert should be immediately visible</param>
        /// <param name="encodeHtml">whether to escape HTML markup for display or let it render naturally</param>
        /// <param name="messageDetails">extra text to reinforce the message, such as the stack trace for an <c>Exception</c> error alert</param>
        /// <param name="messageDetailsRendering">hints for rendering message details in Bootstrap alerts</param>
        /// <returns>an <c>Alert</c> object</returns>
        public static Alert CreateDarkAlert
        (
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool? closeable = null,
            bool encodeHtml = false,
            bool fade = true,
            bool show = true,
            string messageDetails = null,
            AlertMessageDetailsRenderingPolicy messageDetailsRendering = default
        )
        {
            return CreateAlert
            (
                AlertType.Dark,
                message,
                emphasis: emphasis,
                autoEmphasis: autoEmphasis,
                closeable: closeable,
                encodeHtml: encodeHtml,
                fade: fade,
                show: show,
                messageDetails: messageDetails,
                messageDetailsRendering: messageDetailsRendering
            );
        }

        /// <summary>
        /// Creates an <c>Alert</c> object useful for rendering Bootstrap 'light' alerts
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="emphasis">boldface text to prepend to the message</param>
        /// <param name="autoEmphasis">add alert type as <c>emphasis</c></param>
        /// <param name="closeable">where to add widget for user to close alert</param>
        /// <param name="fade">whether alert should attempt to fade in and out</param>
        /// <param name="show">whether alert should be immediately visible</param>
        /// <param name="encodeHtml">whether to escape HTML markup for display or let it render naturally</param>
        /// <param name="messageDetails">extra text to reinforce the message, such as the stack trace for an <c>Exception</c> error alert</param>
        /// <param name="messageDetailsRendering">hints for rendering message details in Bootstrap alerts</param>
        /// <returns>an <c>Alert</c> object</returns>
        public static Alert CreateLightAlert
        (
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool? closeable = null,
            bool encodeHtml = false,
            bool fade = true,
            bool show = true,
            string messageDetails = null,
            AlertMessageDetailsRenderingPolicy messageDetailsRendering = default
        )
        {
            return CreateAlert
            (
                AlertType.Light,
                message,
                emphasis: emphasis,
                autoEmphasis: autoEmphasis,
                closeable: closeable,
                encodeHtml: encodeHtml,
                fade: fade,
                show: show,
                messageDetails: messageDetails,
                messageDetailsRendering: messageDetailsRendering
            );
        }

        /// <summary>
        /// Same as <c>CreateDangerAlert()</c>
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="emphasis">boldface text to prepend to the message</param>
        /// <param name="autoEmphasis">add alert type as <c>emphasis</c></param>
        /// <param name="closeable">where to add widget for user to close alert</param>
        /// <param name="fade">whether alert should attempt to fade in and out</param>
        /// <param name="show">whether alert should be immediately visible</param>
        /// <param name="encodeHtml">whether to escape HTML markup for display or let it render naturally</param>
        /// <param name="messageDetails">extra text to reinforce the message, such as the stack trace for an <c>Exception</c> error alert</param>
        /// <param name="messageDetailsRendering">hints for rendering message details in Bootstrap alerts</param>
        /// <returns>an <c>Alert</c> object</returns>
        public static Alert CreateErrorAlert
        (
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool? closeable = null,
            bool encodeHtml = false,
            bool fade = true,
            bool show = true,
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
                fade: fade,
                show: show,
                messageDetails: messageDetails,
                messageDetailsRendering: messageDetailsRendering
            );
        }

        /// <summary>
        /// Creates an <c>Alert</c> object from an <c>Exception</c> useful for rendering Bootstrap 'danger' alerts
        /// </summary>
        /// <param name="exception">an exception</param>
        /// <param name="emphasis">boldface text to prepend to the message</param>
        /// <param name="autoEmphasis">add alert type as <c>emphasis</c></param>
        /// <param name="closeable">where to add widget for user to close alert</param>
        /// <param name="fade">whether alert should attempt to fade in and out</param>
        /// <param name="show">whether alert should be immediately visible</param>
        /// <param name="encodeHtml">whether to escape HTML markup for display or let it render naturally</param>
        /// <param name="alertType">alert type</param>
        /// <param name="typeRendering">define preference for <c>Exception</c> rendering, specifically the exception <c>Type</c></param>
        /// <param name="includeStackTrace">define preference for <c>Exception</c> rendering, specifically whether to include the stack trace</param>
        /// <param name="indent">define preference for <c>Exception</c> rendering, specifically how much to indent the stack trace</param>
        /// <param name="recursive">define preference for <c>Exception</c> rendering, specifically whether to recurse</param>
        /// <param name="exceptionRendering">define preference for <c>Exception</c> rendering, specifically whether and how to render in a Bootstrap alert</param>
        /// <returns>an <c>Alert</c> object</returns>
        /// <returns>an <c>Alert</c> object</returns>
        public static Alert CreateErrorAlert
        (
            ExceptionInfo exception,
            string emphasis = null,
            bool autoEmphasis = true,
            bool? closeable = null,
            bool encodeHtml = true,
            bool fade = true,
            bool show = true,
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
                fade: fade,
                show: show,
                messageDetails: resultantErrorRendering != default
                    ? exception?.Render(typeRendering: typeRendering, includeStackTrace: includeStackTrace, indent: indent, recursive: recursive)
                    : null,
                messageDetailsRendering: resultantErrorRendering.ToAlertMessageDetailsRendering()
            );
        }
    }
}
