namespace Horseshoe.NET.Text
{
    public delegate void JsonGenerated(string json);
    public delegate void CharCleaned(char charIdentified, string replacement, int position, string category, bool isCustomDictionary);
}
