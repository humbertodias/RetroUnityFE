using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using System;

namespace Utility {
    /// <summary>
    /// Linux specific implementation for handling DLL loading.
    /// </summary>
    public sealed class LinuxDLLHandler : IDLLHandler {

		public static IntPtr LoadLibrary (string fileName)
		{
			IntPtr retVal = dlopen (fileName, RTLD_NOW);
			var errPtr = dlerror ();
			if (errPtr != IntPtr.Zero) {
				Debug.LogError (Marshal.PtrToStringAnsi (errPtr));
			}
			return retVal;
		}

		public static void FreeLibrary (IntPtr handle)
		{
			dlclose (handle);
		}

		const int RTLD_NOW = 2;

		[DllImport("libdl.so")]
		private static extern IntPtr dlopen (String fileName, int flags);

		[DllImport("libdl.so")]
		private static extern IntPtr dlsym (IntPtr handle, String symbol);

		[DllImport("libdl.so")]
		private static extern int dlclose (IntPtr handle);

		[DllImport("libdl.so")]
		private static extern IntPtr dlerror ();

		private static IntPtr _dllPointer = IntPtr.Zero;

        private static readonly LinuxDLLHandler instance = new LinuxDLLHandler();

        /// <summary>
        /// Prevent 'new' keyword.
        /// </summary>
        private LinuxDLLHandler() {
        }

        /// <summary>
        /// Gets the current instance (singleton).
        /// </summary>
        public static LinuxDLLHandler Instance {
            get {
				#if UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX || UNITY_WEBGL
						return instance;
				#else
						Debug.LogError("This DLL handler is only compatible with Linux.");
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

		public T GetMethod<T>(String functionName) where T : class {
			if (_dllPointer == IntPtr.Zero) {
				Debug.LogError("DLL not found, cannot get method '" + functionName + "'");
				return default(T);
			}

			IntPtr pAddressOfFunctionToCall = dlsym(_dllPointer, functionName);

			if (pAddressOfFunctionToCall == IntPtr.Zero) {
				return default(T);
			}

			return Marshal.GetDelegateForFunctionPointer(pAddressOfFunctionToCall, typeof(T)) as T;
		}


    }
}
