using System;
using System.Linq;
using System.Reflection;

namespace Horseshoe.NET
{
    /// <summary>
    /// Information about this Horseshoe.NET library.
    /// </summary>
    public static class Lib
    {
        /// <summary>
        /// Gets the current Horseshoe.NET assembly.
        /// </summary>
        public static Assembly Assembly => typeof(Lib).Assembly;

        /// <summary>
        /// Gets the current Horseshoe.NET assembly's <c>AssemblyName</c>.
        /// </summary>
        public static AssemblyName AssemblyName => Assembly.GetName();

        /// <summary>
        /// Gets the current Horseshoe.NET assembly's name.
        /// </summary>
        public static string Name => AssemblyName.Name;

        /// <summary>
        /// Gets the current Horseshoe.NET assembly's full name.
        /// </summary>
        public static string FullName => AssemblyName.FullName;

        /// <summary>
        /// Gets the current Horseshoe.NET assembly's version (3+ levels).
        /// </summary>
        public static string Version => AssemblyName.GetDisplayVersion(minDepth: 3);

        /// <summary>
        /// Gets the current Horseshoe.NET assembly's display name (w/ version 3+ levels).
        /// </summary>
        public static string DisplayName => AssemblyName.GetDisplayName(minDepth: 3);

        /// <summary>
        /// Gets all currently loaded Horseshoe.NET assemblies.
        /// </summary>
        public static Assembly[] Assemblies => Horseshoe.NET.Assemblies.List()
            .Where(a => a.GetName().Name.StartsWith(Name))
            .OrderBy(a => a.GetName().Name)
            .ToArray();

        /// <summary>
        /// Gets all currently loaded Horseshoe.NET assemblies' <c>AssemblyName</c>s.
        /// </summary>
        public static AssemblyName[] AssemblyNames => Assemblies
            .Select(a => a.GetName())
            .ToArray();

        /// <summary>
        /// Gets all currently loaded Horseshoe.NET assemblies' names.
        /// </summary>
        public static string[] Names => AssemblyNames
            .Select(a => a.Name)
            .ToArray();

        /// <summary>
        /// Gets all currently loaded Horseshoe.NET assemblies' full names.
        /// </summary>
        public static string[] FullNames => AssemblyNames
            .Select(an => an.FullName)
            .ToArray();

        /// <summary>
        /// Gets all currently loaded Horseshoe.NET assemblies' display name.
        /// </summary>
        public static string[] DisplayNames => AssemblyNames
            .Select(an => an.GetDisplayName(minDepth: 3))
            .ToArray();

        /// <summary>
        /// Tries to invoke a method from an assembly that may or may not be loaded, if not then fails gracefully.
        /// </summary>
        /// <param name="assemblyName">The assembly name.</param>
        /// <param name="partialNamespace">The namespace part beyond the assembly name, if applicable.</param>
        /// <param name="className">The class name.</param>
        /// <param name="methodName">The method name.</param>
        /// <param name="methodInvoked">Out parameter, indicates whether the method in question was invoked.</param>
        /// <param name="args">Arguments that must match the method parameters, if any.</param>
        /// <returns>The method result</returns>
        public static object TryInvokeStaticMethod(string assemblyName, string partialNamespace, string className, string methodName, out bool methodInvoked, params object[] args)
        {
            var assembly = Horseshoe.NET.Assemblies.Get(assemblyName, suppressErrors: true);
            var type = assembly?.GetType(assemblyName + (partialNamespace == null ? "" : "." + partialNamespace) + "." + className);
            methodInvoked = false;
            if (type == null)
            {
                return null;
            }
            try
            {
                var obj = type.InvokeMember(methodName, BindingFlags.InvokeMethod, null, null, args);
                methodInvoked = true;
                return obj;
            }
            catch (MethodAccessException) { throw; }
            catch (MissingMethodException) { throw; }
            catch (TargetException) { throw; }
            catch (AmbiguousMatchException) { throw; }
            catch { }
            return null;
        }

        /// <summary>
        /// Tries to invoke a method from an assembly that may or may not be loaded, if not then fails gracefully.
        /// </summary>
        /// <typeparam name="T">The return type of the method.</typeparam>
        /// <param name="assemblyName">The assembly name.</param>
        /// <param name="partialNamespace">The namespace part beyond the assembly name, if applicable.</param>
        /// <param name="className">The class name.</param>
        /// <param name="methodName">The method name.</param>
        /// <param name="methodInvoked">Out parameter, indicates whether the method in question was invoked.</param>
        /// <param name="args">Arguments that must match the method parameters, if any.</param>
        /// <returns>The method result</returns>
        public static T TryInvokeStaticMethod<T>(string assemblyName, string partialNamespace, string className, string methodName, out bool methodInvoked, params object[] args)
        {
            return (T)TryInvokeStaticMethod(assemblyName, partialNamespace, className, methodName, out methodInvoked, args);
        }
    }
}
