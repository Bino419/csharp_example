using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        System.Net.IPAddress ip = System.Net.IPAddress.Parse("172.0.180.235");
        int port = 22;
        // Timeout label is reserved for threading use cases, so we use waittime here
        int waittime = 10;
        ConnectionParams connectionParams = new ConnectionParams(ip, port, waittime);
        string result = Connect.TestConnection(connectionParams).GetAwaiter().GetResult().ToString();
        Console.Write(result);
    }
}

public class ConnectionParams
{
    public System.Net.IPAddress IP { get; set; }
    public int Port { get; set; }
    public int WaitTime { get; set; }

    public ConnectionParams(System.Net.IPAddress ip, int port, int waittime)
    {
        IP = ip;
        Port = port;
        WaitTime = waittime;
    }
}

public class Connect
{
    public static async Task<bool> TestConnection(ConnectionParams obj)
    {
        using (TcpClient client = new TcpClient())
        {
            using (var cts = new CancellationTokenSource())
            {
                var connectionTask = client.ConnectAsync(obj.IP, obj.Port);
                var connectionTimeout = Task.Delay(obj.WaitTime * 1000, cts.Token);
                var completedTask = await Task.WhenAny(connectionTask, connectionTimeout);
                if (completedTask == connectionTask)
                {
                    cts.Cancel();
                    try
                    {
                        await connectionTask;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
