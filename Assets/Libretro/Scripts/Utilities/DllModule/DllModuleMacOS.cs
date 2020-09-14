#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SK.Libretro.Utilities
{
    public sealed class DllModuleMacOS : DllModule
    {
        [DllImport("libc.dylib", EntryPoint = "dlopen")]
        private static extern IntPtr MacOSLoadLibrary([MarshalAs(UnmanagedType.LPTStr)] string lpLibFileName, int flags);

        [DllImport("libc.dylib", EntryPoint = "dlsym")]
        private static extern IntPtr MacOSGetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        [DllImport("libdc.dylib", EntryPoint = "dlclose")]
        private static extern bool MacOSFreeLibrary(IntPtr hModule);

        public override void Load(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                IntPtr hModule = MacOSLoadLibrary(path, 2); // 2 is for RTLD_NOW
                if (hModule != IntPtr.Zero)
                {
                    Name = Path.GetFileName(path);
                    _nativeHandle = hModule;
                }
                else
                {
                    throw new Exception($"Failed to load library at path '{path}'");
                }
            }
            else
            {
                throw new Exception("Library path is null or empty.");
            }
        }

        public override T GetFunction<T>(string functionName)
        {
            if (_nativeHandle != IntPtr.Zero)
            {
                IntPtr procAddress = MacOSGetProcAddress(_nativeHandle, functionName);
                if (procAddress != IntPtr.Zero)
                {
                    return Marshal.GetDelegateForFunctionPointer<T>(procAddress);
                }
                else
                {
                    throw new Exception($"Function '{functionName}' not found in library '{Name}'.");
                }
            }
            else
            {
                throw new Exception($"Library not loaded, cannot get function '{functionName}'");
            }
        }

        public override void Free()
        {
            if (_nativeHandle != IntPtr.Zero)
            {
                _ = MacOSFreeLibrary(_nativeHandle);
            }
        }
    }
}
#endif