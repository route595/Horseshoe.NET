using System;

namespace Horseshoe.NET.Http
{
    public struct UriString
    {
        public Uri Uri { get; }

        public UriString(string stringValue)
        {
            Uri = stringValue != null
                ? new Uri(stringValue)
                : null;
            //Uri = new Uri(stringValue ?? throw new ArgumentNullException(nameof(stringValue)));
        }

        public UriString(Uri uriValue)
        {
            Uri = uriValue;
            //Uri = uriValue ?? throw new ArgumentNullException(nameof(uriValue));
        }

        public bool IsHttps()
        {
            return Uri != null && Uri.IsHttps();
        }

        public override string ToString()
        {
            return Uri?.ToString();
        }

        public static implicit operator UriString(string stringValue) => new UriString(stringValue);
        public static implicit operator UriString(Uri uriValue) => new UriString(uriValue);
        public static implicit operator Uri(UriString uriString) => uriString.Uri;
    }
}
