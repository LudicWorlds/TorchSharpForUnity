# TorchSharpForUnity.unitypackage

This 'TorchSharpForUnity.unitypackage' contains a collection of DLLs, obtained via nuget.org, that install TorchSharp within your Unity Project. The required DLLs are imported into the 'Assets/Plugins' folder of your project. This includes TorchSharp version 0.102.6 and the DLLs on which it depends.

This TorchSharp package is Windows and CPU only (no GPU/Cuda support). It has been tested in Unity 6000.3.4f1.

For more information on TorchSharp, visit the [TorchSharp GitHub repository](https://github.com/dotnet/TorchSharp).  
For examples and tutorials, visit the [TorchSharp Examples repository](https://github.com/dotnet/TorchSharpExamples).


## License

**Unity Integration:** Released under [CC0 1.0 Universal (Public Domain)](LICENSE)
You are free to use, modify, and distribute this Unity integration (and test scripts) without any attribution or restrictions.

**Third-Party Dependencies:** Each library in the Plugins folder includes its own license file. See the LICENSE.txt or LICENSE.TXT files within each subfolder for specific licensing terms.


## Step-by-Step Installation Guide

Download `TorchSharpForUnity.unitypackage` from the [latest Release](https://github.com/LudicWorlds/TorchSharpForUnity/releases/latest).

### 1. Install the 'Code Analysis' Package

1. In Unity, open the Package Manager:
   - Go to **Window > Package Manager** in the Unity Editor.

2. Enable the visibility of preview packages:
   - Click on the gear icon in the top right corner of the Package Manager window.
   - Select **Advanced Project Settings**.
   - In the **Project Settings** window, under the **Package Manager** section, check the box for **Enable Preview Packages**.

3. Add the 'Code Analysis' package:
   - In the Package Manager, click on the **+** button in the top left corner.
   - Select **Add package from git URL...**.
   - In the 'Name' text field, enter `com.unity.code-analysis` and click **Add**.
   - The package will be installed and ready for use in your project.

### 2. Import the 'TorchSharpForUnity.unitypackage'

1. Go to **Assets > Import Package > Custom Package...**.
2. Select the `TorchSharpForUnity.unitypackage` file.
3. In the Import Unity Package window, ensure all items are checked and click **Import**.

### Important Notes:
- Enabling preview packages allows you to install experimental and preview versions of packages that are not fully released. Use these packages with caution as they may have limited support and could introduce instability.
- Ensure that you follow the order of these steps to avoid any dependency issues.


## Test TorchSharp in your Project

1. After importing the 'TorchSharpForUnity.unitypackage', restart the Unity Editor to ensure that the TorchSharp DLLs are loaded correctly.
2. In the Unity Editor, open the scene: 'TorchSharp/Scenes/TorchSharpScene'.

Notice in the Hierarchy View, there is a 'Script' GameObject with the 'TorchSharpTest' script attached.

3. Click the 'Play' button in the Editor to start 'Play Mode'.

In the Console Panel, you should notice some Debug Logs relating to a TorchSharp Tensor.


## Disclaimer

The 'TorchSharpForUnity.unitypackage' is provided "as is" without warranty of any kind. Use at your own risk. The authors are not responsible for any damage or data loss resulting from the use of this package. Compatibility and performance may vary depending on your system configuration. This version is designed for Windows and currently only supports CPU operations.