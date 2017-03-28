using System;
using System.Runtime.InteropServices;

namespace ClickOnceUtil4UI.Utils.EmbeddedManifests
{
    /// <summary>
    /// WinAPI methods.
    /// </summary>
    public static class NativeMethods
    {
        /// <summary>
        /// If this value is used, the system maps the file into the calling process's virtual address space as if it were a data file.
        /// </summary>
        public const uint LOAD_LIBRARY_AS_DATAFILE = 2;

        /// <summary>
        /// Embedded resource id.
        /// </summary>
        public static readonly IntPtr RT_MANIFEST = new IntPtr(24);
        
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
        public static extern int EnumResourceNames(
            IntPtr hModule,
            IntPtr pType,
            EnumResNameProc enumFunc,
            IntPtr param);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = false, SetLastError = true)]
        public static extern IntPtr FindResource(IntPtr hModule, IntPtr pName, IntPtr pType);

        [DllImport("Kernel32.dll", CharSet = CharSet.None, ExactSpelling = false, SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("mscorwks.dll", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
        public static extern object GetAssemblyIdentityFromFile([In] string filePath, [In] ref Guid riid);

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = false, SetLastError = true)]
        public static extern IntPtr LoadLibraryExW(string strFileName, IntPtr hFile, uint ulFlags);

        [DllImport("Kernel32.dll", CharSet = CharSet.None, ExactSpelling = false, SetLastError = true)]
        public static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResource);

        [DllImport("oleaut32.dll", CharSet = CharSet.Unicode, ExactSpelling = false, PreserveSig = false)]
        public static extern void LoadTypeLibEx(
            string strTypeLibName,
            RegKind regKind,
            out object typeLib);

        [DllImport("Kernel32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        public static extern IntPtr LockResource(IntPtr hGlobal);

        [DllImport("sfc.dll", CharSet = CharSet.Unicode, ExactSpelling = false)]
        public static extern int SfcIsFileProtected(IntPtr RpcHandle, string ProtFileName);

        [DllImport("Kernel32.dll", CharSet = CharSet.None, ExactSpelling = false, SetLastError = true)]
        public static extern uint SizeofResource(IntPtr hModule, IntPtr hResource);

        public delegate bool EnumResNameProc(IntPtr hModule, IntPtr pType, IntPtr pName, IntPtr param);

        public enum RegKind
        {
            RegKind_Default,

            RegKind_Register,

            RegKind_None
        }
    }
}