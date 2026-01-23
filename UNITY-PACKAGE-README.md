# TorchSharpForUnity.unitypackage

This 'TorchSharpForUnity.unitypackage' contains a collection of DLLs, obtained via nuget.org, that install TorchSharp within your Unity Project. The required DLLs are imported into the 'Assets/Plugins' folder of your project. This includes TorchSharp version 0.105.2 and the DLLs on which it depends.

This TorchSharp package is Windows and CPU only (no GPU/Cuda support). It has been tested in Unity 6000.3.5f1.

For more information on TorchSharp, visit the [TorchSharp GitHub repository](https://github.com/dotnet/TorchSharp).  
For examples and tutorials, visit the [TorchSharp Examples repository](https://github.com/dotnet/TorchSharpExamples).


## License

**Unity Integration Code (this project):** Released under [CC0 1.0 Universal (Public Domain)](LICENSE)
You are free to use, modify, and distribute this Unity integration without any attribution or restrictions.

**TorchSharp Library:** Released under [MIT License](LICENSE-TORCHSHARP.txt) by the .NET Foundation
The TorchSharp library and its dependencies retain their original licenses.

**Third-Party Components:** TorchSharp incorporates PyTorch, Caffe2, and other third-party software.
See [THIRD-PARTY-NOTICES-TORCHSHARP](THIRD-PARTY-NOTICES-TORCHSHARP.txt) for complete attribution and license information.

**Third-Party Dependencies:** Each library in the Plugins folder includes its own license file. See the LICENSE.txt or LICENSE.TXT files within each subfolder for specific licensing terms.


## Step-by-Step Installation Guide

### 1. Download the 'TorchSharpForUnity.unitypackage'

Download `TorchSharpForUnity.unitypackage` from the [latest Release](https://github.com/LudicWorlds/TorchSharpForUnity/releases/latest).


### 2. Import the 'TorchSharpForUnity.unitypackage'

1. Go to **Assets > Import Package > Custom Package...**.
2. Select the `TorchSharpForUnity.unitypackage` file.
3. In the Import Unity Package window, ensure all items are checked and click **Import**.


## Test TorchSharp in your Project

1. After importing the 'TorchSharpForUnity.unitypackage', restart the Unity Editor to ensure that the TorchSharp DLLs are loaded correctly.
2. In the Unity Editor, open the scene: 'TorchSharp/Scenes/TorchSharpScene'.

Notice in the Hierarchy View, there is a 'Script' GameObject with the 'TorchSharpTest' script attached.

3. Click the 'Play' button in the Editor to start 'Play Mode'.

In the Console Panel, you should notice some Debug Logs relating to a TorchSharp Tensor.


## Disclaimer

The 'TorchSharpForUnity.unitypackage' is provided "as is" without warranty of any kind. Use at your own risk. The authors are not responsible for any damage or data loss resulting from the use of this package. Compatibility and performance may vary depending on your system configuration. This version is designed for Windows and currently only supports CPU operations.