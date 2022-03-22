using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace DDC.Helpers
{
    internal class DebugFileChecker
    {
        internal static IEnumerable<(string Filename, bool IsDebug)> Check(string? path)
        {
            if (path == null)
                yield break;

            string[] searchExtensions = { ".dll", ".exe", ".lib" };
            var files = Directory
                        .GetFiles(path!, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(file => searchExtensions.Any(ext => file.ToLower().Contains(ext)))
                        .ToList();
            foreach (var file in files)
            {
                if (!IsAssembly(file))
                    continue;

                if (IsInDebugMode(file))
                {
                    yield return (file, true);
                }
                else
                {
                    yield return (file, false);
                }
            }
        }

        #region private
        static bool IsAssembly(string path)
        {
            using FileStream? fs = new(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            // Try to read CLI metadata from the PE file.
            using PEReader? peReader = new(fs);

            if (!peReader.HasMetadata)
            {
                return false; // File does not have CLI metadata.
            }

            // Check that file has an assembly manifest.
            MetadataReader reader = peReader.GetMetadataReader();
            return reader.IsAssembly;
        }
        static bool IsInDebugMode(string FileName)
        {
            Assembly assembly = Assembly.LoadFile(FileName);
            return IsInDebugMode(assembly);
        }
        static bool IsInDebugMode(Assembly Assembly)
        {
            var attributes = Assembly.GetCustomAttributes(typeof(System.Diagnostics.DebuggableAttribute), false);
            if (attributes.Length > 0)
            {
                if (attributes[0] is System.Diagnostics.DebuggableAttribute debuggable)
                    return (debuggable.DebuggingFlags & System.Diagnostics.DebuggableAttribute.DebuggingModes.Default) == System.Diagnostics.DebuggableAttribute.DebuggingModes.Default;
                else
                    return false;
            }
            else
                return false;
        }
        #endregion
    }
}
