namespace Horseshoe.NET.Jwt
{
    public class AccessTokenHeader
    {
        public string Algorithm { get; set; }
        public string Type { get; set; }
        public string KID { get; set; }
        public string X5t { get; set; }

        public class Raw
        {
#pragma warning disable IDE1006 // Naming Styles
            public string alg { get; set; }
            public string kid { get; set; }
            public string typ { get; set; }
            public string x5t { get; set; }
#pragma warning restore IDE1006 // Naming Styles
            public AccessTokenHeader ToTokenHeader()
            {
                return new AccessTokenHeader
                {
                    Algorithm = alg,
                    Type = typ,
                    KID = kid,
                    X5t = x5t
                };
            }
        }
    }
}
