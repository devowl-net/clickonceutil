using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ClickOnceUtil4UI.Utils.EmbeddedManifests
{
    /// <summary>
    /// Reader for embedded manifest.
    /// </summary>
    public class EmbeddedManifest
    {
        private Stream _manifest;
        
        private EmbeddedManifest(string path)
        {
            var result = IntPtr.Zero;
            try
            {
                result = NativeMethods.LoadLibraryExW(path, IntPtr.Zero, 2);
                if (result != IntPtr.Zero)
                {
                    NativeMethods.EnumResourceNames(result, NativeMethods.RT_MANIFEST, EnumCallback, IntPtr.Zero);
                }
            }
            finally
            {
                if (result != IntPtr.Zero)
                {
                    NativeMethods.FreeLibrary(result);
                }
            }
        }

        /// <summary>
        /// Read embedded _manifest XML.
        /// </summary>
        /// <param name="path">Path to file.</param>
        /// <returns>Manifest stream.</returns>
        public static Stream Read(string path)
        {
            return new EmbeddedManifest(path)._manifest;
        }

        private bool EnumCallback(IntPtr module, IntPtr type, IntPtr name, IntPtr param)
        {
            if (name != new IntPtr(1)) 
            {
                return false;
            }

            var intPtr = NativeMethods.FindResource(module, name, NativeMethods.RT_MANIFEST);
            if (intPtr == IntPtr.Zero)
            {
                return false;
            }

            var embeddedResource = NativeMethods.LoadResource(module, intPtr);
            NativeMethods.LockResource(embeddedResource);
            var resource = new byte[NativeMethods.SizeofResource(module, intPtr)];
            Marshal.Copy(embeddedResource, resource, 0, resource.Length);

            _manifest = new MemoryStream(resource, false);
            return false;
        }
    }
}