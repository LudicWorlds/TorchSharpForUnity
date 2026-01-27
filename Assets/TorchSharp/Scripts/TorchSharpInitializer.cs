using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// Initializes TorchSharp and SkiaSharp native libraries before any managed code runs.
/// This script must have a very early execution order.
/// </summary>
public static class TorchSharpInitializer
{
    private static bool _initialized = false;

    /// <summary>
    /// Returns the path to native plugins (handles Editor vs Build differences).
    /// Editor: Assets/TorchSharp/Plugins/x86_64
    /// Build:  [AppName]_Data/Plugins/x86_64
    /// </summary>
    public static string GetNativePluginsPath()
    {
#if UNITY_EDITOR
        return Path.Combine(Application.dataPath, "TorchSharp", "Plugins", "x86_64");
#else
        return Path.Combine(Application.dataPath, "Plugins", "x86_64");
#endif
    }

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
            // Get the path to the native libraries (platform-specific subfolder)
            string pluginsPath = GetNativePluginsPath();

            if (!Directory.Exists(pluginsPath))
            {
                Debug.LogError($"[TorchSharpInitializer] Plugins directory not found: {pluginsPath}");
                Debug.LogError("[TorchSharpInitializer] Ensure native DLLs are in Assets/TorchSharp/Plugins/x86_64/ and have correct .meta settings.");
                return;
            }

            Debug.Log($"[TorchSharpInitializer] Loading native libraries from: {pluginsPath}");

            // Load libraries in dependency order (libtorch-cpu 2.7.1 + SkiaSharp 2.88.6)
            string[] librariesToLoad = new string[]
            {
                "libSkiaSharp.dll",     // SkiaSharp native library
                "libiomp5md.dll",
                "libiompstubs5md.dll",
                "asmjit.dll",
                "c10.dll",
                "fbgemm.dll",
                "uv.dll",
                "torch_cpu.dll",
                "torch.dll",
                "torch_global_deps.dll"
            };

            foreach (string lib in librariesToLoad)
            {
                string libPath = Path.Combine(pluginsPath, lib);
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
