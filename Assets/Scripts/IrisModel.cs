using static TorchSharp.torch.nn;
using static TorchSharp.torch;
using System.Text;

public class IrisModel : Module<Tensor, Tensor>
{
    //The 'parent' Module is divided into 'SubModules'
    private Module<Tensor, Tensor> fc1; // 'fc' stands for fully-connected 
    private Module<Tensor, Tensor> fc2; // 'h' stands for hidden layer 
    private Module<Tensor, Tensor> fc3;

    public IrisModel(int in_features = 4, int h1 = 9, int h2 = 9, int out_features = 3) : base(nameof(IrisModel))
    {
        fc1 = Linear(in_features, h1);
        fc2 = Linear(h1, h2);
        fc3 = Linear(h2, out_features);

        RegisterComponents();
    }

    public override Tensor forward(Tensor x)
    {
        x = functional.relu(fc1.forward(x));
        x = functional.relu(fc2.forward(x));
        x = fc3.forward(x);

        return x;
    }

    public string PrintParameters()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Model Parameters:");
        foreach (var named_param in this.named_parameters())
        {
            var paramName = named_param.name;
            var paramShape = named_param.parameter.shape;
            sb.AppendLine($"Parameter: {paramName}, Shape: {string.Join(", ", paramShape)}");
        }
        return sb.ToString();
    }

    public override string ToString()
    {
        return $"IrisModel(\n" +
               $"  (fc1): {fc1.GetType().Name}\n" +
               $"  (fc2): {fc2.GetType().Name}\n" +
               $"  (fc3): {fc3.GetType().Name}\n)";
    }
}