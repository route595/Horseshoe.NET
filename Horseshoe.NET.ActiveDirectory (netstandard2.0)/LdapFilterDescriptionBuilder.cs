namespace Horseshoe.NET.ActiveDirectory
{
    public class LdapFilterDescriptionBuilder
    {
        public string CN { get; set; }
        public string ObjectClass { get; set; }
        public string ObjectCategory { get; set; }

        public override string ToString()
        {
            if (CN != null)
            {
                if (ObjectCategory != null)
                {
                    if (ObjectClass != null)
                        return "AD " + ObjectCategory + " " + ObjectClass + ": " + CN;
                    return "AD " + ObjectCategory + ": " + CN;
                }
                if (ObjectClass != null)
                    return "AD " + ObjectClass + ": " + CN;
                return "AD item: " + CN;
            }
            if (ObjectCategory != null)
            {
                if (ObjectClass != null)
                    return "AD " + ObjectCategory + " " + ObjectClass;
                return "AD " + ObjectCategory;
            }
            if (ObjectClass != null)
                return "AD " + ObjectClass;
            return "AD item";
        }
    }
}