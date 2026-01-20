using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// Initializes TorchSharp native libraries before any TorchSharp code runs.
/// This script must have a very early execution order.
/// </summary>
public static class TorchSharpInitializer
{
    private static bool _initialized = false;

    // Windows API for loading DLLs
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr LoadLibrary(string lpFileName);

    [DllImport("kernel32.dll")]
    private static extern uint GetLastError();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        if (_initialized) return;

        try
        {
            // Get the path to the native libraries
            string pluginsPath = Path.Combine(Application.dataPath, "TorchSharp", "Plugins");
            string nativeLibPath = Path.Combine(pluginsPath, "libtorch-cpu-win-x64", "win-x64");

            Debug.Log($"[TorchSharpInitializer] Loading native libraries from: {nativeLibPath}");

            // Load libraries in dependency order
            string[] librariesToLoad = new string[]
            {
                "libiomp5md.dll",
                "libiompstubs5md.dll",
                "asmjit.dll",
                "c10.dll",
                "fbgemm.dll",
                "uv.dll",
                "torch_cpu.dll",
                "torch.dll",
                "torch_global_deps.dll",
                "fbjni.dll",
                "pytorch_jni.dll"
            };

            foreach (string lib in librariesToLoad)
            {
                string libPath = Path.Combine(nativeLibPath, lib);
                if (File.Exists(libPath))
                {
                    IntPtr handle = LoadLibrary(libPath);
                    if (handle != IntPtr.Zero)
                    {
                        Debug.Log($"[TorchSharpInitializer] Loaded: {lib}");
                    }
                    else
                    {
                        uint error = GetLastError();
                        Debug.LogWarning($"[TorchSharpInitializer] Failed to load: {lib} (Error: {error})");
                    }
                }
                else
                {
                    Debug.LogWarning($"[TorchSharpInitializer] Not found: {libPath}");
                }
            }

            // Load LibTorchSharp.dll from the Plugins folder
            string libTorchSharpPath = Path.Combine(pluginsPath, "LibTorchSharp.dll");
            if (File.Exists(libTorchSharpPath))
            {
                IntPtr handle = LoadLibrary(libTorchSharpPath);
                if (handle != IntPtr.Zero)
                {
                    Debug.Log($"[TorchSharpInitializer] Loaded: LibTorchSharp.dll");
                }
                else
                {
                    uint error = GetLastError();
                    Debug.LogError($"[TorchSharpInitializer] Failed to load LibTorchSharp.dll (Error: {error})");
                }
            }

            _initialized = true;
            Debug.Log("[TorchSharpInitializer] Native library initialization complete.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[TorchSharpInitializer] Error initializing native libraries: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
