using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET
{
    public static class Lib
    {
        public static Assembly Assembly => typeof(Lib).Assembly;

        public static AssemblyName AssemblyName => Assembly.GetName();

        public static string Name => AssemblyName.Name;

        public static string FullName => AssemblyName.FullName;

        public static string Version => AssemblyName.GetDisplayVersion(minDepth: 3);

        public static string DisplayName => AssemblyName.GetDisplayName(minDepth: 3);

        public static Assembly[] Assemblies => Horseshoe.NET.Assemblies.List()
            .Where(a => a.GetName().Name.StartsWith(Name))
            .OrderBy(a => a.GetName().Name)
            .ToArray();

        public static AssemblyName[] AssemblyNames => Assemblies
            .Select(a => a.GetName())
            .ToArray();

        public static string[] Names => AssemblyNames
            .Select(a => a.Name)
            .ToArray();

        public static string[] FullNames => AssemblyNames
            .Select(an => an.FullName)
            .ToArray();

        public static string[] DisplayNames => AssemblyNames
            .Select(an => an.GetDisplayName(minDepth: 3))
            .ToArray();

        public static bool IsLoaded(string assemblyName)
        {
            return LoadAssembly(assemblyName) != null;
        }

        public static Assembly LoadAssembly(string assemblyName)
        {
            try
            {
                var assembly = Assembly.Load(assemblyName);
                return assembly;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static object TryInvokeStaticMethod(string assemblyName, string partialNamespace, string className, string methodName, out bool methodInvoked, params object[] args)
        {
            var assembly = LoadAssembly(assemblyName);
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

        public static T TryInvokeStaticMethod<T>(string assemblyName, string partialNamespace, string className, string methodName, out bool methodInvoked, params object[] args)
        {
            return (T)TryInvokeStaticMethod(assemblyName, partialNamespace, className, methodName, out methodInvoked, args);
        }
    }
}
