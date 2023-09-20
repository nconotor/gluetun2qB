using QBittorrent.Client;
using System.Text.Json;
using System.Text.Json.Serialization;

var gluetunUrl = Environment.GetEnvironmentVariable("GLUETUN_URL");
var qbUrl = Environment.GetEnvironmentVariable("QB_URL");
var qbUser = Environment.GetEnvironmentVariable("QB_USER");
var qbPw = Environment.GetEnvironmentVariable("QB_PW");

Console.WriteLine("Starting gluetun2qB");
Console.Write($"Gluetun URL: {gluetunUrl} \nqBittorrent: {qbUrl} \nqBittorrent User: {qbUser}\n");

try
{
    var client = new HttpClient();
    var qbClient = new QBittorrentClient(new Uri(qbUrl));
    qbClient.LoginAsync(qbUser, qbPw).Wait();
    while (true)
    {
        var qbPort = (await qbClient.GetPreferencesAsync()).ListenPort;
        Console.WriteLine($"Current qBittorrent Port {qbPort}");
        var json = await client.GetStringAsync($"{gluetunUrl}/v1/openvpn/portforwarded");
        var portNat = JsonSerializer.Deserialize<PortType>(json)?.Port ?? 0;
        Console.WriteLine($"Current gluetun Port {portNat} as json: {json}");

        if (portNat == 0)
        {
            Console.WriteLine("Failed to fetch nat port.");
        }
        else
        {
            if (qbPort != portNat)
            {
                var p = new Preferences
                {
                    ListenPort = portNat
                };
                await qbClient.SetPreferencesAsync(p);
                Console.WriteLine($"Changed qBittorrent {qbPort} to {portNat}");
            }
            else
            {
                Console.WriteLine($"No port update");
            }
        }

        await Task.Delay(TimeSpan.FromMinutes(1));
    }
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
#if DEBUG
    throw;
#endif
}


record PortType
{
    [JsonPropertyName("port")] public int Port { get; set; }
}