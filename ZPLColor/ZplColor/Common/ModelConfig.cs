using System.Text;

namespace ZplColor.Common;

public class ModelConfig
{
    public string Model { get; set; }
    public string ModelClassHandler { get; set; }
    public string NumberPart { get; set; }
    public bool MarcaCromatica { get; set; }

    public void DeepCopy(ModelConfig modelData)
    {

        if (modelData == null)
        {
            throw new ArgumentNullException(nameof(modelData));
        }

        this.Model = modelData.Model;
        this.ModelClassHandler = modelData.ModelClassHandler;
        this.NumberPart = modelData.NumberPart;
        this.MarcaCromatica = modelData.MarcaCromatica;


    }

    // Static method to create a default instance
    public static ModelConfig CreateDefault()
    {
        return new ModelConfig
        {
            Model = "NONE",
            ModelClassHandler = "PrinterCSharp.Color.NoMarcaCromatica",
            NumberPart = "NONE",
            MarcaCromatica = false
        };
    }

    // Override ToString() method
    public override string ToString()
    {
        return $"Model: {Model}, ModelClassHandler: {ModelClassHandler}, NumberPart: {NumberPart}, MarcaCromatica: {MarcaCromatica}";
    }
}

public class ModelConfigurations
{
    public Dictionary<string, ModelConfig> Models { get; set; }

    // Override ToString() method
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("ModelConfigurations:");
        foreach (var kvp in Models)
        {
            sb.AppendLine($"Key: {kvp.Key}, Value: {kvp.Value.ToString()}");
            sb.AppendLine("");
        }
        return sb.ToString();
    }
}