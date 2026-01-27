using System.Collections.Generic;
using System;
using TorchSharp;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq;
using static TorchSharp.torch.optim;
using TorchSharp.Modules;
using SkiaSharp;
using LudicWorlds;

public class IrisTraining : MonoBehaviour
{
    [SerializeField]
    private TextAsset _iris_csv;

    [SerializeField]
    [Tooltip("Optional: RawImage to display the training loss plot")]
    private RawImage _lossPlotImage;

    private const int RANDOM_SEED = 42;

    //Data
    private List<IrisData> _irisData = new List<IrisData>();
    
    private List<IrisData> _trainData;
    private List<IrisData> _testData;

    private torch.Tensor _x_train;
    private torch.Tensor _y_train;
    private torch.Tensor _x_test;
    private torch.Tensor _y_test;

    //Training
    private IrisModel _irisModel;
    private CrossEntropyLoss _criterion;
    private Adam _optimizer;

    private int _epochs;
    private List<float> _losses = new List<float>();

    private int _stage = 0;

    // Plot texture
    private Texture2D _lossPlotTexture;
    private Canvas _skiaPanelCanvas;


    void Awake()
    {
        _stage = 0;

        // Get the Canvas from the RawImage's parent and hide it initially
        if (_lossPlotImage != null)
        {
            _skiaPanelCanvas = _lossPlotImage.GetComponentInParent<Canvas>();
            if (_skiaPanelCanvas != null)
            {
                _skiaPanelCanvas.enabled = false;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DebugPanel.SetStatus("PrintTorchSharpCaps()");
        Debug.Log("TorchSharp - Iris Dataset Demo\n");
        PrintTorchSharpCaps();
        Debug.Log("Press [SPACEBAR] to advance demo.\n");
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ExecuteNextStage(); //of the Iris Training demo.
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Application.Quit();
        }
    }

    private void ExecuteNextStage()
    {
        switch (_stage)
        {
            case 0:
                InitData();
                break;
            case 1:
                PrepareTrainAndTestDatasets();
                break;
            case 2:
                InitIrisModel();
                break;
            case 3:
                Train();
                break;
            case 4:
                if (_skiaPanelCanvas != null) _skiaPanelCanvas.enabled = true;
                PlotTrainingLoss();
                break;
            case 5:
                if (_skiaPanelCanvas != null) _skiaPanelCanvas.enabled = false;
                Test();
                break;
            case 6:
                PredictUsingNewData();
                break;
        }

        _stage++;
    }

    private void PrintTorchSharpCaps()
    {
        string caps = $"Torch Version: {torch.__version__}\nCuda is available: {torch.cuda.is_available()}\n"; 
        Debug.Log(caps);
        torch.TensorStringStyle = TensorStringStyle.Numpy;
    }

    private void InitData()
    {
        DebugPanel.SetStatus("InitData()");

        if (_iris_csv != null)
        {
            //Debug.Log("InitData() - CSV Data Loaded Successfully");
            ParseCSV(_iris_csv.text);

            //Iris Data Loaded Successfully!
            string irisDataPrintOut = "Iris Dataset:\n";
            irisDataPrintOut += PrintOutData();
            irisDataPrintOut += $"Iris Dataset Count: {_irisData.Count}\n";
            Debug.Log(irisDataPrintOut + "\n");
            Debug.Log("Press [SPACEBAR] to advance demo.\n");
        }
        else
        {
            Debug.Log("InitData() - _iris_csv is NULL!");
        }
    }

    private void PrepareTrainAndTestDatasets()
    {
        DebugPanel.SetStatus("PrepareTrainAndTestDatasets()");

        (_trainData, _testData) = TrainTestSplit(_irisData, 0.8f);

        // Separate the features and labels
        (List<float[]> x_train, List<int> y_train) = SeparateFeaturesAndLabels(_trainData);
        (List<float[]> x_test, List<int> y_test) = SeparateFeaturesAndLabels(_testData);

        // Convert Features(x) and Label(y) to TorchSharp tensors
        _x_train = ConvertFeaturesToTensor(x_train);
        _y_train = ConvertLabelsToTensor(y_train);
        _x_test = ConvertFeaturesToTensor(x_test);
        _y_test = ConvertLabelsToTensor(y_test);

        //pyTorch:
        //X_train = torch.FloatTensor(X_train);
        //x_test = torch.FloatTensor(x_test);
        // Now you can use the tensors with TorchSharp.. :)

        string trainTestInfo =  $"Train Data Count: {_trainData?.Count}\n";
        trainTestInfo +=        $"Test Data Count: {_testData?.Count}\n\n";
        trainTestInfo +=        $"Test Features:\n";
        trainTestInfo +=        $"_x_test: {_x_test?.ToString(TensorStringStyle.Default)}\n";
        trainTestInfo +=        $"_x_test Rows: {_x_test?.shape[0]}\n\n";
        trainTestInfo +=        $"Test Labels:\n";
        trainTestInfo +=        $"_y_test: {_y_test?.ToString(TensorStringStyle.Default)}\n";
        trainTestInfo +=        $"_y_test Rows: {_y_test?.shape[0]}\n\n";

        Debug.Log(trainTestInfo);
        Debug.Log("Press [SPACEBAR] to advance demo.\n");
    }

    private void InitIrisModel()
    {
        DebugPanel.SetStatus("InitIrisModel()");

        torch.random.manual_seed(RANDOM_SEED);
        _irisModel = new IrisModel();

        //Set the criterion of model to measure the error, how far off the predictions are from the data
        _criterion = torch.nn.CrossEntropyLoss();

        //Choose Adam Optimizer. lr = learning rate .Tweak the value of parameter's during Back propagation?
        //(if error doesn't go down aftere a bunch of (Epochs) , we probably want to lower the learning rate)
        //the lower the learning rate, the longer it takes to train the model.
        //pyTorch:
        //optimizer == torch.optim.Adam(model.parameters(), lr = 0.01)

        // Define the optimizer
        _optimizer = torch.optim.Adam(_irisModel.parameters(), lr: 0.01);

        //Debug.Log("-> Model Parameters: " + _irisModel.named_parameters());

        Debug.Log(_irisModel.PrintParameters());
        //PrintParameters();
        Debug.Log("Press [SPACEBAR] to advance demo.\n");
    }

    private void Train()
    { //Train the Iris Model
        DebugPanel.SetStatus("Train()");
        Debug.Log("Train()");

        if (_irisModel is null || _x_train is null || _criterion is null
            || _y_train is null || _optimizer is null) return;

        _irisModel.train(); // ensure that the model is in training mode

        _epochs = 300;
        _losses.Clear();
        float lossVal = 0f;
        int i = 0;

        for (i = 0; i < _epochs; i++)
        {
            // Go forward and get a prediction - _x_train is the training data
            var y_pred = _irisModel.forward(_x_train); // Get predicted results

            var loss = _criterion.call(y_pred, _y_train); // predicted values vs the y_train
            lossVal = loss.detach().cpu().item<float>();

            _losses.Add(lossVal);

            if (i % 10 == 0)
            {
                Debug.Log($"Epoch: {i} and loss: {lossVal}");
            }

            // Do some back propagation: take the error rate of forward propagation and feed it back
            // thru the network to fine tune the weights
            _optimizer.zero_grad();
            loss.backward();
            _optimizer.step();      
        }

        GC.Collect();
        Debug.Log($"Epoch: {i} and loss: {lossVal}\n");
        Debug.Log("Press [SPACEBAR] to show loss curve.\n");
    }

    private void PlotTrainingLoss()
    {
        DebugPanel.SetStatus("PlotTrainingLoss()");

        if (_losses.Count == 0) return;

        //Debug.Log("Plotting training loss...");

        using (var plot = new SkiaPlot(750, 500))
        {
            plot.Plot(_losses, SKColors.Blue, "Training Loss")
                .SetTitle("Iris Model Training")
                .SetXLabel("Epoch")
                .SetYLabel("Loss")
                .ShowGrid(true)
                .ShowLegend(true);

            // Clean up previous texture
            if (_lossPlotTexture != null)
            {
                Destroy(_lossPlotTexture);
            }

            _lossPlotTexture = plot.ToTexture2D();
        }

        // Display on RawImage if assigned
        if (_lossPlotImage != null)
        {
            _lossPlotImage.texture = _lossPlotTexture;
            Debug.Log("Loss plot displayed on RawImage.");
        }
        else
        {
            Debug.Log("Loss plot generated. Assign a RawImage to _lossPlotImage to display it.");
        }
    }

    private void Test()
    {
        DebugPanel.SetStatus("Test()");
        Debug.Log("Test()");

        if (_irisModel is null || _x_test is null || _criterion is null
        || _y_test is null) return;

        int correct = 0;

        _irisModel.eval(); // Set the model to evaluation mode

        //Evaluate Model on Test Data Set (validate model on test set)
        using (torch.no_grad()) //no_grad turns off back propagation
        {
            //Forwrd pass: compute predicted outputs by passing inputs to the model
            var y_eval = _irisModel.forward(_x_test); //X-test are features from our test set
                                                      //y_eval will be predictions

            // Measure the loss/error using the criterion object
            var loss = _criterion.call(y_eval, _y_test); // Find the loss or error

            string testInfo = $"Test Loss: {loss.item<float>()}\n";

            //-----------------

            for (int i = 0; i < _x_test.size(0); i++)
            {
                var data = _x_test[i];
                var y_val = _irisModel.forward(data);
                string y_val_str = y_val.ToString(TensorStringStyle.Default);
                long y_test_long = _y_test[i].item<long>();

                // Get the predicted class (argmax) and convert it to an integer
                long predictedClass = y_val.argmax().item<long>();

                // Print the type of flower class
                testInfo += $"{i + 1}.)\t{y_val_str}     \t{y_test_long}\t{predictedClass}\n";

                if (y_test_long == predictedClass)
                    correct++;
            }

            testInfo += $"We got {correct} correct!\n";
            Debug.Log(testInfo);
            GC.Collect();
        }

        Debug.Log("Press [SPACEBAR] to advance demo.\n");
    }

    private void PredictUsingNewData()
    {
        DebugPanel.SetStatus("PredictUsingNewData()");

        if (_irisModel is null) return;

        _irisModel.eval();

        // Create a new tensor for the iris data
        var newIris = torch.tensor(new float[] { 4.7f, 3.2f, 1.3f, 0.2f });

        // 5.9,3.0,5.1,1.8          2 virginica
        var newer_iris = torch.tensor(new float[] { 5.9f, 3.0f, 5.1f, 1.8f });

        //Evaluate Model on Test Data Set (validate model on test set)
        using (torch.no_grad()) //no_grad turns off back propagation
        {
            // Make a prediction
            var prediction1 = _irisModel.forward(newIris);
            string predictionInfo = $"Pred 1: {prediction1.ToString(TensorStringStyle.Default)}\n";

            var prediction2 = _irisModel.forward(newer_iris);
            predictionInfo += $"Pred 2:  {prediction2.ToString(TensorStringStyle.Default)}\n";

            Debug.Log(predictionInfo);
        }

        Debug.Log("End of Demo!\n");
    }


    (List<IrisData> trainData, List<IrisData> testData) TrainTestSplit(List<IrisData> data, float trainRatio)
    {
        // Shuffle the data
        System.Random rand = new System.Random(RANDOM_SEED);
        data = data.OrderBy(x => rand.Next()).ToList();

        // Calculate the split index
        int trainCount = (int)(data.Count * trainRatio);
        //Debug.Log("-> TrainTestSplit() - trainCount: " + trainCount);

        // Split the data
        List<IrisData> trainData = data.Take(trainCount).ToList();
        List<IrisData> testData = data.Skip(trainCount).ToList();

        return (trainData, testData);
    }

    (List<float[]> features, List<int> labels) SeparateFeaturesAndLabels(List<IrisData> data)
    {
        var features = new List<float[]>();
        var labels = new List<int>();

        foreach (var row in data)
        {
            features.Add(new float[] { row.SepalLength, row.SepalWidth, row.PetalLength, row.PetalWidth });
            labels.Add(row.SpeciesId);
        }

        return (features, labels);
    }

    torch.Tensor ConvertFeaturesToTensor(List<float[]> data)
    {
        var shape = new long[] { data.Count, data[0].Length };
        var tensor = torch.zeros(shape, dtype: torch.float32);

        for (int i = 0; i < data.Count; i++)
        {
            for (int j = 0; j < data[i].Length; j++)
            {
                tensor[i, j] = data[i][j];
            }
        }

        return tensor;
    }

    torch.Tensor ConvertLabelsToTensor(List<int> data)
    {
        var shape = new long[] { data.Count };
        var tensor = torch.zeros(shape, dtype: torch.int64); //int64 is a long

        for (int i = 0; i < data.Count; i++)
        {
            tensor[i] = data[i];
        }

        return tensor;
    }

    private void ParseCSV(string csvText)
    {
        string[] lines = csvText.Split('\n');

        //the first line, the labels header, should be skipped...
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            if (!string.IsNullOrEmpty(line))
            {
                string[] values = line.Split(',');
                if (values.Length == 6) // Ensure there are exactly 6 columns
                {
                    int id = int.Parse(values[0]);
                    float sepalLength = float.Parse(values[1]);
                    float sepalWidth = float.Parse(values[2]);
                    float petalLength = float.Parse(values[3]);
                    float petalWidth = float.Parse(values[4]);
                    string species = values[5];

                    IrisData irisDataRow = new IrisData(id, sepalLength, sepalWidth, petalLength, petalWidth, species);
                    _irisData.Add(irisDataRow);
                }
            }
        }
    }


    private string PrintOutData(int numLines = 5)
    {
        string str = "";
        
        numLines = Math.Min(numLines, _irisData.Count);

        for (int i = 0; i < numLines; i++)
        {
            str += "[ " + _irisData[i].ToString() + " ]\n";
        }

        return str;
    }

    void OnApplicationQuit()
    {
        // Dispose TorchSharp native resources
        _x_train?.Dispose();
        _y_train?.Dispose();
        _x_test?.Dispose();
        _y_test?.Dispose();
        _irisModel?.Dispose();
        _criterion?.Dispose();
        _optimizer?.Dispose();

        // Clean up plot texture
        if (_lossPlotTexture != null)
        {
            Destroy(_lossPlotTexture);
            _lossPlotTexture = null;
        }
    }
}
