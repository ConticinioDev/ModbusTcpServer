using System.Net;
using System.Net.Sockets;
using NModbus;
using NModbus.Data;

string bindIp = args.FirstOrDefault(a => a.StartsWith("--ip="))?.Split("=")[1] ?? "0.0.0.0";
int port = int.TryParse(args.FirstOrDefault(a => a.StartsWith("--port="))?.Split("=")[1], out var p) ? p : 502;
byte unitId = byte.TryParse(args.FirstOrDefault(a => a.StartsWith("--unit="))?.Split("=")[1], out var u) ? u : (byte)1;

Console.WriteLine($"[Modbus TCP Server] Binding {bindIp}:{port} (UnitId={unitId})");

var listener = new TcpListener(IPAddress.Parse(bindIp), port);
listener.Start();

// Crear fábrica y red
var factory = new ModbusFactory();
var network = factory.CreateSlaveNetwork(listener);

// Crear data store (nuevo modelo: PointSources)
var dataStore = new DefaultSlaveDataStore();

// Inicializar valores (usa .WritePoints, base 0)
dataStore.HoldingRegisters.WritePoints(0, new ushort[] { 1500, 250, 1234 });
dataStore.InputRegisters.WritePoints(0, new ushort[] { 42 });

// Crear el slave y agregarlo a la red
var slave = factory.CreateSlave(unitId, dataStore);
network.AddSlave(slave);

// Simulación: variar el valor del primer holding register (RPM)
using var cts = new CancellationTokenSource();
_ = Task.Run(async () =>
{
    ushort rpm = 1500;
    while (!cts.Token.IsCancellationRequested)
    {
        rpm = (ushort)(rpm >= 2200 ? 1400 : rpm + 5);
        dataStore.HoldingRegisters.WritePoints(0, new ushort[] { rpm }); // 40001
        dataStore.InputRegisters.WritePoints(10, new ushort[] { (ushort)DateTime.UtcNow.Second }); // 30011
        await Task.Delay(1000, cts.Token);
    }
}, cts.Token);

Console.CancelKeyPress += (s, e) =>
{
    e.Cancel = true;
    cts.Cancel();
    listener.Stop();
};

Console.WriteLine("Listening... (Ctrl+C para salir)");
try
{
    await network.ListenAsync(cts.Token);

}catch (Exception ex) 
{
    Console.WriteLine(ex.ToString());
}
