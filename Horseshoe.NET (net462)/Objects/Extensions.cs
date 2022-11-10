using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.Objects
{
    public static class Extensions
    {
        public static void MapProperties(this object src, object dest, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance, bool tryMapAll = false, bool nullOverwrites = false, bool ignoreCase = false, bool ignoreErrors = false) =>
            ObjectUtil.MapProperties(src, dest, bindingFlags: bindingFlags, tryMapAll: tryMapAll, nullOverwrites: nullOverwrites, ignoreCase: ignoreCase, ignoreErrors: ignoreErrors);

        public static object Duplicate(this object src, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance, DuplicateMode duplicateMode = DuplicateMode.MapProperties, bool ignoreErrors = false) =>
            ObjectUtil.Duplicate(src, bindingFlags: bindingFlags, duplicateMode: duplicateMode, ignoreErrors: ignoreErrors);
    }
}
