using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Horseshoe.NET.Http
{
    public static class WebServiceUtil
    {
        public static Action<string> JsonPayloadGenerated;

        private static Regex PropertyNameFromBackingFieldRegex { get; } = new Regex(@"(?<=\<)[A-Z_]+(?=\>k__BackingField)", RegexOptions.IgnoreCase);
        private static Regex BackingFieldRegex { get; } = new Regex(@"\<[A-Z_]+\>k__BackingField", RegexOptions.IgnoreCase);

        public static string ZapBackingFields(string rawSerializedText)
        {
            if (rawSerializedText != null)
            {
                var backingFields = new List<string>();

                foreach (Match match in BackingFieldRegex.Matches(rawSerializedText))
                {
                    backingFields.Add(match.Value);
                }

                if (backingFields.Any())
                {
                    backingFields = backingFields.Distinct().ToList();

                    foreach (var backingField in backingFields)
                    {
                        var propertyName = PropertyNameFromBackingFieldRegex.Match(backingField).Value;
                        rawSerializedText = rawSerializedText.Replace(backingField, propertyName);
                    }
                }
            }
            return rawSerializedText;
        }
    }
}
