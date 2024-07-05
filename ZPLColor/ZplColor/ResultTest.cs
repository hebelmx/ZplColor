using ZplColor.Common;

namespace ZplColor;

public class ResultTest()
{
    public double MaxiCruceH { get; set; }
    public double MaxiCruceV { get; set; }
    public ModelConfig Model { get; set; }


    public string Data { get; set; }
    public bool Print { get; set; }

    public bool Retransmit { get; set; }

    public ResultTest DeepCopy()
    {
        return new ResultTest
        {
            MaxiCruceH = this.MaxiCruceH,
            MaxiCruceV = this.MaxiCruceV,
            Model = this.Model,
            Data = this.Data,
            Print = this.Print,
        };
    }

    public override string ToString()
    {

        return $" MaxiCruceV {MaxiCruceV} MaxiCruceH {MaxiCruceH}   : Model {Model}  : Print {Print}  ";

    }
}