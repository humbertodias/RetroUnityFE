#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SK.Libretro.Utilities
{
    public sealed class DllModuleMacOS : DllModule
    {
        private const int RTLD_NOW = 2;

        [DllImport("libc.dylib", EntryPoint = "dlopen")]
        private static extern IntPtr MacOSLoadLibrary([MarshalAs(UnmanagedType.LPStr)] string path, int flags);

        [DllImport("libc.dylib", EntryPoint = "dlsym")]
        private static extern IntPtr MacOSGetProcAddress(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string symbol);

        [DllImport("libc.dylib", EntryPoint = "dlclose")]
        private static extern int MacOSFreeLibrary(IntPtr handle);

        public override void Load(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception("Library path is null or empty.");

            if (!File.Exists(path))
                throw new Exception($"Library file not found at path '{path}'");

            IntPtr handle = MacOSLoadLibrary(path, RTLD_NOW);
            if (handle == IntPtr.Zero)
                throw new Exception($"Failed to load library at path '{path}'");

            _nativeHandle = handle;
            Name = Path.GetFileName(path);
        }

        public override T GetFunction<T>(string functionName)
        {
            if (_nativeHandle == IntPtr.Zero)
                throw new Exception($"Library not loaded, cannot get function '{functionName}'");

            IntPtr procAddress = MacOSGetProcAddress(_nativeHandle, functionName);
            if (procAddress == IntPtr.Zero)
                throw new Exception($"Function '{functionName}' not found in library '{Name}'");

            return Marshal.GetDelegateForFunctionPointer<T>(procAddress);
        }

        public override void Free()
        {
            if (_nativeHandle != IntPtr.Zero)
            {
                int result = MacOSFreeLibrary(_nativeHandle);
                if (result != 0)
                    UnityEngine.Debug.LogWarning($"Failed to free library '{Name}'");
                
                _nativeHandle = IntPtr.Zero;
            }
        }
    }
}
#endif
