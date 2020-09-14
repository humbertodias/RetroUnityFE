#if UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SK.Libretro.Utilities
{
    public sealed class DllModuleLinux : DllModule
    {
        [DllImport("libdl.so", EntryPoint = "dlopen")]
        private static extern IntPtr LinuxLoadLibrary([MarshalAs(UnmanagedType.LPTStr)] string lpLibFileName, int flags);

        [DllImport("libdl.so", EntryPoint = "dlsym")]
        private static extern IntPtr LinuxGetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        [DllImport("libdl.so", EntryPoint = "dlclose")]
        private static extern bool LinuxFreeLibrary(IntPtr hModule);

        public override void Load(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                IntPtr hModule = LinuxLoadLibrary(path, 2); // 2 is for RTLD_NOW
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
                IntPtr procAddress = LinuxGetProcAddress(_nativeHandle, functionName);
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
                _ = LinuxFreeLibrary(_nativeHandle);
            }
        }
    }
}
#endif