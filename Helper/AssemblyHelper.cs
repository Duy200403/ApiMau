using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ApiWebsite.Helper
{
    public static class AssemblyHelper
    {
        private static Assembly[] _assemblies;
        private static Type[] _exportTypes;
        private static readonly object _locker = new();

        public static IEnumerable<Assembly> Assemblies
        {
            get
            {
                if (_assemblies != null) return _assemblies;
                lock (_locker)
                {
                    _assemblies = GetAssemblies().ToArray();
                }
                return _assemblies;
            }
        }

        public static IEnumerable<Type> ExportTypes
        {
            get
            {
                if (_exportTypes != null) return _exportTypes;
                lock (_locker)
                {
                    _exportTypes = Assemblies.SelectMany(asm => asm.GetExportedTypes().Where(t => !t.IsAbstract && !t.IsInterface)).ToArray();
                }
                return _exportTypes;
            }
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            var assembly = Assembly.GetEntryAssembly();
            yield return assembly;
            if (assembly is null) yield break;
            foreach (var asm in assembly.GetReferencedAssemblies())
                yield return Assembly.Load(asm);
        }
    }
}
