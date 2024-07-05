using System.IO.Ports;

namespace ZplColor.Common;

/// <summary>
/// Represents the configuration settings for a printer.
/// </summary>
public class GatewayConfig
{
    /// <summary>
    /// IP address of the modbus server.
    /// </summary>
    public string IpAddressModbusServer { get; set; }


    /// <summary>
    /// IP address of the printer.
    /// </summary>
    public string IpAddressPrinter { get; set; }





    /// <summary>
    /// zpl program to send to the printer
    /// </summary>
    public string ZPLProgram { get; set; }
    /// <summary>
    /// Tolerance level for printing operations.
    /// </summary>
    public double Tolerance { get; set; }

    /// <summary>
    /// Network address of the printer.
    /// </summary>
    public int Address { get; set; }

    /// <summary>
    /// Adjustment parameter for the Audit ResultAnalyze axis.
    /// </summary>
    public double AuditAdjustAimY { get; set; }

    public bool SendProgram { get; set; }
    public int PrinterPort { get; set; }

    public PingSettings PingSettings { get; set; }

    /// <summary>
    /// Returns a string representation of the current object.
    /// </summary>
    /// <returns>A string containing the property values.</returns>
    public override string ToString()
    {
        return $"GatewayConfig: [IpAddressModbusServer={IpAddressModbusServer}, IpAddressPrinter={IpAddressPrinter}, Tolerance={Tolerance}, Address={Address}, AuditAdjustAimY={AuditAdjustAimY}, PrinterPort={PrinterPort}] \n\n ZPLProgram=[{ZPLProgram}]\n\n ";

    }
}

public class PingSettings
{
    public int MaxRetries { get; set; } = 100;  // Number of retry attempts
    public int PingTimeout { get; set; } = 200; // Timeout in milliseconds
    public int PingRetryTime { get; set; } = 100; // Number of retry attempts

    /// <summary>
    /// Returns a string representation of the current object.
    /// </summary>
    /// <returns>A string containing the property values.</returns>
    public override string ToString()
    {
        return $"PingSettings: [MaxRetries={MaxRetries}, PingTimeout={PingTimeout}, PingRetryTime={PingRetryTime}]";
    }
}


public class SerialPortConfig
{
    /// <summary>
    /// Serial port of the Gateway.
    /// </summary>

    public string PortName { get; set; } = "COM3";
    public int BaudRate { get; set; } = 9600;
    public Parity Parity { get; set; } = Parity.None;
    public StopBits StopBits { get; set; } = StopBits.One;
    public int DataBits { get; set; } = 8;
    public Handshake Handshake = Handshake.None;
    public int ReadTimeout { get; set; } = 5000;
    public int WriteTimeout { get; set; } = 500;

    public bool PortFound { get; set; }

    public bool UseFoundedSerialPort { get; set; } = true;

    /// <summary>
    /// Returns a string representation of the current object.
    /// </summary>
    /// <returns>A string containing the property values.</returns>
    public override string ToString()
    {
        return $"SerialPortConfig: [PortName={PortName}, BaudRate={BaudRate}, Parity={Parity}, StopBits={StopBits}, DataBits={DataBits}, Handshake={Handshake}, ReadTimeout={ReadTimeout}, WriteTimeout={WriteTimeout}]";
    }
}