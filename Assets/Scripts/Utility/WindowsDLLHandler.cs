using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Utility {
    /// <summary>
    /// Windows specific implementation for handling DLL loading. Requires kernel32.dll.
    /// </summary>
    public sealed class WindowsDLLHandler : IDLLHandler {

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr hModule);

        private static IntPtr _dllPointer = IntPtr.Zero;

        private static readonly WindowsDLLHandler instance = new WindowsDLLHandler();

        /// <summary>
        /// Prevent 'new' keyword.
        /// </summary>
        private WindowsDLLHandler() {
        }

        /// <summary>
        /// Gets the current instance (singleton).
        /// </summary>
        public static WindowsDLLHandler Instance {
            get {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                return instance;
#else
                Debug.LogError("This DLL handler is only compatible with Windows.");
                return null;
#endif
            }
        }

        public bool LoadCore(string dllPath) {
            _dllPointer = LoadLibrary(dllPath);

            if (_dllPointer == IntPtr.Zero) {
                Debug.LogError("Error loading DLL.");
                return false;
            }

            return true;
        }

        public T GetMethod<T>(string functionName) where T : class {
            if (_dllPointer == IntPtr.Zero) {
                Debug.LogError("DLL not found, cannot get method '" + functionName + "'");
                return default(T);
            }

            IntPtr pAddressOfFunctionToCall = GetProcAddress(_dllPointer, functionName);

            if (pAddressOfFunctionToCall == IntPtr.Zero) {
                return default(T);
            }

            return Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(T)) as T;
        }
    }
}