using System;
using System.Collections.Generic;
using System.Text;

namespace Horseshoe.NET.Ldap
{
    public enum LdapAttributeType
    {
        String,
        StringArray,
        Int,
        Long,
        Double,
        Decimal,
        DateTime,
        Dn,
        DnArray,
        ByteArray,
        ArrayOfByteArray
    }
}
