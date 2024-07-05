using System.Text.Json;

namespace ZplColor.Common;

public class CounterService
{
    private int _counter;
    private const string FilePath = "counter.json";

    public CounterService()
    {
        LoadCounter();
    }

    public int GetCounter()
    {
        return _counter;
    }

    public int Counter => GetCounter();

    public void IncrementCounter()
    {
        _counter++;
        SaveCounter();
    }

    private void SaveCounter()
    {
        var json = JsonSerializer.Serialize(_counter);
        File.WriteAllText(FilePath, json);
    }

    private void LoadCounter()
    {
        if (File.Exists(FilePath))
        {
            var json = File.ReadAllText(FilePath);
            _counter = JsonSerializer.Deserialize<int>(json);
        }
        else
        {
            _counter = 0;
        }
    }
}