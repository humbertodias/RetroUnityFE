#if UNITY_ANDROID

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SK.Libretro.Utilities
{
    public sealed class DllModuleAndroid : DllModule
    {
        [DllImport("libdl.so", EntryPoint = "dlopen")]
        private static extern IntPtr dlopen (String fileName, int flags);

        [DllImport("libdl.so", EntryPoint = "dlsym")]
        private static extern IntPtr dlsym (IntPtr handle, String symbol);

        [DllImport("libdl.so", EntryPoint = "dlclose")]
        private static extern int dlclose (IntPtr handle);

        [DllImport("libdl.so", EntryPoint = "dlerror")]
        private static extern IntPtr dlerror ();

        public override void Load(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                IntPtr hModule = dlopen(path, 2); // 2 is for RTLD_NOW
                var errPtr = dlerror ();
                if (errPtr != IntPtr.Zero) {
                    var errString =  Marshal.PtrToStringAnsi (errPtr);
                    throw new Exception($"Failed to load library at path '{path}' error '{errString}'");
                }
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
                IntPtr procAddress = dlsym(_nativeHandle, functionName);
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
                _ = dlclose(_nativeHandle);
            }
        }
    }
}
#endif