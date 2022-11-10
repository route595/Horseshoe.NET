using System;

namespace Horseshoe.NET.Bootstrap
{
    [Flags]
    public enum AlertMessageDetailsRenderingPolicy
    {
        /// <summary>
        /// Render details "as is"
        /// </summary>
        Default = 0,

        /// <summary>
        /// Display HTML markup rather than honor it
        /// </summary>
        EncodeHtml = 1,

        /// <summary>
        /// Honor spaces and line breaks (useful for rendering exceptions)
        /// </summary>
        PreFormatted = 2,

        /// <summary>
        /// Do not make viewable in the browser (force use of developer tools or the generated source code to view the message details)
        /// </summary>
        Hidden = 4
    }
}
