using UnityEditor;
using UnityEngine;
using System.IO;

public class PackageExporter : MonoBehaviour
{
    [MenuItem("Tools/Export TorchSharp Package")]
    static void ExportPackage()
    {
        // Define the package path and name
        string packagePath = "../UnityPackages";
        string packageName = "TorchSharpForUnity.unitypackage";

        // Ensure the target directory exists
        if (!Directory.Exists(packagePath))
        {
            Directory.CreateDirectory(packagePath);
        }

        // Export the entire TorchSharp folder
        string sourcePath = "Assets/TorchSharp";
        AssetDatabase.ExportPackage(sourcePath, Path.Combine(packagePath, packageName), ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);

        // Create a package manifest to include 'com.unity.code-analysis' dependency
        string manifestPath = Path.Combine(packagePath, "Packages", "manifest.json");
        string manifestDirectory = Path.GetDirectoryName(manifestPath);
        if (!Directory.Exists(manifestDirectory))
        {
            Directory.CreateDirectory(manifestDirectory);
        }

        File.WriteAllText(manifestPath, @"
{
  ""dependencies"": {
    ""com.unity.code-analysis"": ""latest""
  }
}");

        Debug.Log("Package exported to: " + packagePath);
    }
}