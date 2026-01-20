using UnityEngine;
using TorchSharp;

public class TorchSharpTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Torch Version: " + torch.__version__);
        torch.TensorStringStyle = TensorStringStyle.Numpy;

        //Basic math operations on Tensors
        var x = torch.ones(2, 2) * 3;
        var y = torch.ones(2, 2) * 4;

        //in-place addition -> the values of x are added into y
        y.add_(x);
        Debug.Log(y.ToString(TensorStringStyle.Default));

        //subtraction
        //var z = y - x;
        var z = torch.sub(y, x);
        z = torch.mul(y, x); //multiplication
        z = torch.div(y, x); //division
    }
}

