using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET
{
    public static class Assemblies
    {
        /// <summary>
        /// Get all assemblies loaded by the client application
        /// </summary>
        /// <returns></returns>
        public static Assembly[] List()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        /// <summary>
        /// Find a loaded assembly by name
        /// </summary>
        /// <param name="name">e.g. "System.Web"</param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static Assembly Lookup(string name, bool ignoreCase = false)
        {
            var list = List()
                .Where(a => a.GetName().Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                .ToList();
            switch (list.Count)
            {
                case 0:
                    return null;
                case 1:
                    return list.Single();
                default:
                    throw new UtilityException("More than one assembly matches the supplied name: \"" + name + "\"");  // this should never happen
            }
        }

        /// <summary>
        /// Get all assembly names loaded by the client application
        /// </summary>
        /// <returns></returns>
        public static AssemblyName[] ListAssemblyNames()
        {
            return List()
                .Select(a => a.GetName())
                .ToArray();
        }

        /// <summary>
        /// Load an assembly by supplying its full name
        /// </summary>
        /// <param name="fullName">e.g. "System.Web, Version=4.0.0.0"</param>
        /// <param name="suppressErrors"></param>
        /// <returns></returns>
        public static Assembly Load(string fullName, bool suppressErrors = false)
        {
            try
            {
                return Assembly.Load(fullName);
            }
            catch (Exception ex)
            {
                if (suppressErrors) return null;
                throw ex;
            }
        }
    }
}
