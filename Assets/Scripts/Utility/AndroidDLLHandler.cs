using System.Reflection;
using UnityEngine;

namespace Utility {
    /// <summary>
    /// Android specific implementation for handling DLL loading.
    /// </summary>
    public sealed class AndroidDLLHandler : IDLLHandler {

        private Assembly currentAssembly;

        private static readonly AndroidDLLHandler instance = new AndroidDLLHandler();

        /// <summary>
        /// Prevent 'new' keyword.
        /// </summary>
        private AndroidDLLHandler() {
        }

        /// <summary>
        /// Gets the current instance (singleton).
        /// </summary>
        public static AndroidDLLHandler Instance {
            get {
#if UNITY_ANDROID
                return instance;
#else
                Debug.LogError("This DLL handler is only compatible with Android.");
                return null;
#endif
            }
        }

        public bool LoadCore(string dllPath) {
            // TODO: Not working yet.
            using (var www = new WWW(dllPath)) {
                while (!www.isDone) { }
                currentAssembly = Assembly.Load(www.bytes);
            }
            
            return currentAssembly != null;
        }

        public T GetMethod<T>(string functionName) where T : class {
            throw new System.NotImplementedException();
        }
    }
}
