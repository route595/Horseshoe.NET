using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Horseshoe.NET.Bootstrap;

namespace Horseshoe.NET.WebForms
{
    public class WebFormsBootstrap4Alert : WebControl
    {
        public Bootstrap4.Alert Alert { get; }

        public WebFormsBootstrap4Alert(Bootstrap4.Alert alert)
        {
            Alert = alert;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "alert " + Alert.AlertType.ToCssClass() + (Alert.Closeable ? " alert-dismissible" : ""));
            writer.AddAttribute("role", "alert");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);                                                // Begin 'bootstrap alert' div

            if (Alert.Closeable)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Type, "button");
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "close");
                writer.AddAttribute("data-dismiss", "alert");
                writer.AddAttribute("aria-label", "Close");
                writer.RenderBeginTag("button");                                                         // Begin 'close' button
                writer.Write("&times;");
                writer.RenderEndTag();                                                                   // End 'close' button
            }

            if (Alert.Emphasis != null)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Strong);                                         // Begin 'emphasis' strong
                writer.Write(Alert.Emphasis);
                writer.RenderEndTag();                                                                   // End 'emphasis' strong
                writer.Write(" - ");
            }

            // message
            if (Alert.EncodeHtml)
            {
                writer.Write(HttpUtility.HtmlEncode(Alert.Message).Replace("\n\r", "<br />").Replace("\n", "<br />"));
            }
            else
            {
                writer.Write(Alert.Message.Replace("\n\r", "<br />").Replace("\n", "<br />"));
            }

            // render message details
            if (Alert.MessageDetails != null)
            {
                if (Alert.IsMessageDetailsHidden)
                {
                    writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);                                        // Begin 'alert details' div
                    writer.Write(Alert.MessageDetails);
                    writer.RenderEndTag();                                                               // End 'alert details' div
                }
                else
                {
                    var alertDetailsElementID  = "alert-details-" + Guid.NewGuid();
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);                                        // Begin 'toggle link' div
                    writer.AddAttribute("href", "javascript:;");
                    writer.AddAttribute("onclick", "Bootstrap4.toggleAlertDetails(this, '" + alertDetailsElementID + "')");
                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                    writer.Write("show details");
                    writer.RenderEndTag(); // A
                    writer.RenderEndTag(); // Div                                                        // End 'toggle link' div
                    var preStyles = Alert.IsMessageDetailsPreFormatted ? "font-family:Consolas,monospace;font-size:.8em;white-space:pre;" : "";
                    writer.Write("<div id=\"" + alertDetailsElementID + "\" style=\"display:none;" + preStyles + "\">");
                    if (Alert.IsMessageDetailsEncodeHtml)
                    {
                        if (Alert.IsMessageDetailsPreFormatted)
                        {
                            writer.Write(HttpUtility.HtmlEncode(Alert.MessageDetails));
                        }
                        else
                        {
                            writer.Write(HttpUtility.HtmlEncode(Alert.MessageDetails).Replace("\n\r", "<br />").Replace("\n", "<br />"));
                        }
                    }
                    else
                    {
                        writer.Write(Alert.MessageDetails.Replace("\n\r", "<br />").Replace("\n", "<br />"));
                    }
                    writer.Write("</div>");
                }
            }

            writer.RenderEndTag();                                                                       // End 'bootstrap alert' div

            if (Alert.MessageDetails != null && !Alert.IsMessageDetailsHidden)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Script);                                         // Begin 'toggle' script
                writer.Write
                (
                    @"Bootstrap4 = {
                        toggleAlertDetails: function (clickedLink, alertDetailsElementID) {
                            if (window.jQuery) {
                                var $clickedLink = $(clickedLink);
                                if ($clickedLink.prop(""toggled"")) {
                                    $(""#"" + alertDetailsElementID).hide();
                                    $clickedLink.text(""show details"");
                                    $clickedLink.prop(""toggled"", false);
                                }
                                else {
                                    $(""#"" + alertDetailsElementID).show();
                                    $clickedLink.text(""hide details"");
                                    $clickedLink.prop(""toggled"", true);
                                }
                            }
                            else {
                                if (clickedLink.toggled) {
                                    document.getElementById(alertDetailsElementID).style.display = ""none"";
                                    clickedLink.innerText = ""show details"";
                                    clickedLink.toggled = false;
                                }
                                else {
                                    document.getElementById(alertDetailsElementID).style.display = ""block"";
                                    clickedLink.innerText = ""hide details"";
                                    clickedLink.toggled = true;
                                }
                            }
                        }
                    };"
                );
                writer.RenderEndTag();                                                                   // End 'toggle' script
            }
        }
    }
}
