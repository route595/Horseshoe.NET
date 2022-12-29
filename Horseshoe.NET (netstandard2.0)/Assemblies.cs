using System;
using System.Linq;
using System.Reflection;

using Horseshoe.NET.Compare;

namespace Horseshoe.NET
{
    /// <summary>
    /// Factory methods for assembly lookup, loading and displaying.
    /// </summary>
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
        /// Gets all assembly names loaded by the client application
        /// </summary>
        /// <returns></returns>
        public static AssemblyName[] ListAssemblyNames()
        {
            return List()
                .Select(a => a.GetName())
                .ToArray();
        }

        /// <summary>
        /// Finds a loaded assembly by name
        /// </summary>
        /// <param name="name">e.g. "Newtonsoft.Json"</param>
        /// <param name="ignoreCase">If <c>true</c> matches an assembly name if not for the letter case, default is <c>false</c>.</param>
        /// <returns>An assembly or <c>null</c>.</returns>
        /// <exception cref="AssemblyException"></exception>
        public static Assembly Find(string name, bool ignoreCase = false)
        {
            var comparator = Comparator.Equals(name, ignoreCase: ignoreCase);
            var list = List()
                .Where(a => comparator.IsMatch(a.GetName().Name))
                .ToList();
            switch (list.Count)
            {
                case 0:
                    return null;
                case 1:
                    return list.Single();
                default:
                    throw new AssemblyException("More than one assembly matches the supplied name: \"" + name + "\"");  // this should never happen
            }
        }

        /// <summary>
        /// Load an assembly by supplying its full name.
        /// </summary>
        /// <param name="fullName">e.g. "Newtonsoft.Json" or "Newtonsoft.Json, Version=13.0.0.0"</param>
        /// <param name="suppressErrors">If <c>true</c> simply return null, default is <c>false</c>.</param>
        /// <returns>An assembly.</returns>
        /// <exception cref="AssemblyException"></exception>
        public static Assembly Load(string fullName, bool suppressErrors = false)
        {
            try
            {
                return Assembly.Load(fullName);
            }
            catch (Exception ex)
            {
                if (suppressErrors) 
                    return null;
                throw new AssemblyException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Finds a loaded assembly. Failing that it tries to load it.  Equivalent of <c>Find()</c>; <c>Load();</c>.
        /// </summary>
        /// <param name="fullName">e.g. "Newtonsoft.Json"</param>
        /// <param name="ignoreCase">If <c>true</c> matches an assembly name if not for the letter case, default is <c>false</c>.</param>
        /// <param name="suppressErrors">If <c>true</c> and an exception occurs simply return <c>null</c>, default is <c>false</c>.</param>
        /// <returns>An assembly.</returns>
        /// <exception cref="AssemblyException"></exception>
        public static Assembly Get(string fullName, bool ignoreCase = false, bool suppressErrors = false)
        {
            try
            {
                return Find(fullName, ignoreCase: ignoreCase) ?? Load(fullName, suppressErrors: suppressErrors);
            }
            catch (AssemblyException)
            {
                if (suppressErrors)
                    return null;
                throw;
            }
            catch (Exception ex)
            {
                if (suppressErrors)
                    return null;
                throw new AssemblyException(ex.Message, ex);
            }
        }
    }
}
