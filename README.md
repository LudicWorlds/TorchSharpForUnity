# TorchSharp For Unity

This project integrates TorchSharp (PyTorch for .NET) into Unity, enabling machine learning capabilities directly within the Unity engine. It includes TorchSharp version 0.102.6 and all required dependencies as DLLs imported into the 'Assets/TorchSharp/Plugins' folder.

> **Note:** The Unity integration code in this project is released to the public domain (CC0). No attribution required. See [License](#license) section below.

**Key Features:**
- Complete ML training pipeline with the classic Iris dataset example
- Interactive step-by-step training demo
- Runtime neural network training and inference
- Windows and CPU only (no GPU/CUDA support)

This version was tested in Unity 6000.3.4f1.

For more information on TorchSharp, visit the [TorchSharp GitHub repository](https://github.com/dotnet/TorchSharp).
For examples and tutorials, visit the [TorchSharp Examples repository](https://github.com/dotnet/TorchSharpExamples).


## License

**Unity Integration Code (this project):** Released under [CC0 1.0 Universal (Public Domain)](LICENSE)
You are free to use, modify, and distribute this Unity integration without any attribution or restrictions.

**TorchSharp Library:** Released under [MIT License](LICENSE-TORCHSHARP.txt) by the .NET Foundation
The TorchSharp library and its dependencies retain their original licenses.

**Third-Party Components:** TorchSharp incorporates PyTorch, Caffe2, and other third-party software.
See [THIRD-PARTY-NOTICES-TORCHSHARP.TXT](THIRD-PARTY-NOTICES-TORCHSHARP.TXT) for complete attribution and license information.


## What's Included

This project contains:
- **TorchSharp 0.102.6** - Core deep learning library with CPU-only libtorch binaries
- **Iris Classification Demo** - Complete example of training a neural network on the Iris dataset
- **All Dependencies** - Microsoft.DotNet.Interactive, System.Reactive, System.Text.Json, and other required libraries
- **Example Scripts** - `IrisModel.cs`, `IrisTraining.cs`, and basic tensor operations demo

For package distribution, a `TorchSharpForUnity.unitypackage` can be exported using the included PackageExporter script.


## Setup Instructions

### Running the Project

1. Open the project in Unity 6000.3.4f1 (or compatible version)
2. The Code Analysis package should already be installed (check `Packages/manifest.json`)
3. Open the scene: **Assets/Scenes/IrisTrainingScene.unity**
4. Enter Play Mode

## Demo Scenes

### Iris Training Demo (Primary Example)

**Scene:** `Assets/Scenes/IrisTrainingScene.unity`

This interactive demo trains a neural network to classify Iris flowers using the classic Iris dataset.

*Adapted from [Pytorch Iris Classification Tutorial](https://youtu.be/JHWqWIoac2I) by John Elder (Codemy.com), and converted to TorchSharp.*

**How to use:**
1. Enter Play Mode
2. Press **SPACEBAR** to advance through each training stage:
   - Stage 1: Load CSV data (150 samples)
   - Stage 2: Split into train/test sets (120/30 split)
   - Stage 3: Initialize 3-layer neural network (4→9→9→3)
   - Stage 4: Train for 300 epochs with Adam optimizer
   - Stage 5: Evaluate accuracy on test set
   - Stage 6: Make predictions on new data
3. Watch the Console for detailed output at each stage

**Neural Network Architecture:**
- Input: 4 features (sepal length, sepal width, petal length, petal width)
- Hidden layers: 9 neurons each with ReLU activation
- Output: 3 classes (Setosa, Versicolor, Virginica)
- Loss: CrossEntropyLoss
- Optimizer: Adam (learning rate: 0.01)

### Basic TorchSharp Test

**Scene:** `Assets/TorchSharp/Scenes/TorchSharpScene.unity`

A simple scene demonstrating basic tensor operations. Enter Play Mode to see tensor creation and manipulation logged to the Console.


## Technical Details

**Dependencies:**
- TorchSharp 0.102.6
- LibTorch CPU (Windows x64)
- Microsoft.DotNet.Interactive 1.0.0-beta.24229.4
- System.Reactive 6.0.1
- System.Text.Json 8.0.7
- SkiaSharp 2.88.6
- Unity Code Analysis package (provides Roslyn/Microsoft.CodeAnalysis dependencies)

**System Requirements:**
- Windows 64-bit
- Unity 6000.3.4f1 (or compatible version)
- CPU only (currently no GPU/CUDA support)

## Disclaimer

This project is provided "as is" without warranty of any kind. Use at your own risk. The authors are not responsible for any damage or data loss resulting from the use of this software. Compatibility and performance may vary depending on your system configuration.
