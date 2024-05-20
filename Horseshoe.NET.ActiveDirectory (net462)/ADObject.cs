using System;
using System.Collections.Generic;
using System.Linq;
using Horseshoe.NET.Collections;

namespace Horseshoe.NET.ActiveDirectory
{
    public abstract class ADObject : ADMember
    {
        public string GetStringProperty(string propertyName)
        {
            return Data.ValueOrDefault(propertyName)?.ToString();
        }

        public DateTime GetDateTimeProperty(string propertyName)
        {
            return Zap.DateTime(Data.ValueOrDefault(propertyName));
        }

        public DateTime? GetNDateTimeProperty(string propertyName)
        {
            return Zap.NDateTime(Data.ValueOrDefault(propertyName));
        }

        public object[] GetObjectArrayProperty(string propertyName)
        {
            return Data.ValueOrDefault(propertyName) as object[];
        }

        public string[] GetObjectArrayPropertyAsStrings(string propertyName)
        {
            return GetObjectArrayProperty(propertyName)?
                .Select(obj => (string)obj)
                .ToArray();
        }

        public void SetProperty(string propertyName, object value)
        {
            Data.AddRemoveOrReplace(propertyName, value);
        }

        /*
         * Begin meta properties
         */

        public override string AdsPath  // overrides ADMember.AdsPath
        {
            get => GetStringProperty(ADConstants.MetaProperties.adspath);
            set
            {
                SetProperty(ADConstants.MetaProperties.adspath, value);
                if (value != null)
                {
                    ADUtil.ExtractMemberParts(value, out string rawCn, out string rawOu, out string rawDc);
                    RawCN = rawCn;
                    RawOU = rawOu;
                    RawDC = rawDc;
                }
            }
        }

        public string Name
        {
            get => GetStringProperty(ADConstants.MetaProperties.name);
            set => SetProperty(ADConstants.MetaProperties.name, value);
        }
        
        public new string CN  // overrides ADMember.CN
        {
            get => GetStringProperty(ADConstants.MetaProperties.cn);
            set => SetProperty(ADConstants.MetaProperties.cn, value);
        }

        public string DistinguishedName
        {
            get => GetStringProperty(ADConstants.MetaProperties.distinguishedname);
            set => SetProperty(ADConstants.MetaProperties.distinguishedname, value);
        }

        public string SAMAccountName
        {
            get => GetStringProperty(ADConstants.MetaProperties.samaccountname);
            set => SetProperty(ADConstants.MetaProperties.samaccountname, value);
        }

        public DateTime WhenCreated
        {
            get => GetDateTimeProperty(ADConstants.MetaProperties.whencreated);
            set => SetProperty(ADConstants.MetaProperties.whencreated, value);
        }

        public DateTime WhenChanged
        {
            get => GetDateTimeProperty(ADConstants.MetaProperties.whenchanged);
            set => SetProperty(ADConstants.MetaProperties.whenchanged, value);
        }
        
        /*
         * End meta properties
         */

        protected IDictionary<string, object> Data { get; }

        public ADObject(IDictionary<string, object> data)
        {
            Data = data;
            if (AdsPath != null)
            {
                ADUtil.ExtractMemberParts(AdsPath, out string rawCn, out string rawOu, out string rawDc);
                RawCN = rawCn;
                RawOU = rawOu;
                RawDC = rawDc;
            }
        }

        public ADObject() : this(new Dictionary<string, object>())
        {
        }
    }
}