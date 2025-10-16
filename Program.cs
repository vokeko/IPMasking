// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Text.RegularExpressions;

string? network;


do
{
    Console.WriteLine("Zadejte síť ve formátu x.x.x.x/x");
    network = Console.ReadLine();
    bool isValid = !string.IsNullOrEmpty(network) && CheckNetworkValidity(network);
    if (!isValid)
    {
        Console.WriteLine("Zadali jste neplatnou síť");
    }
    else
    {
        Console.WriteLine("Zadali jste platnou síť");
        break;
    }
}
while (true);

do
{
    Console.WriteLine("Zadejte ip adresu");
    string? ip = Console.ReadLine();
    if (string.IsNullOrEmpty(ip))
        break;
    if (ValidateIpAddress(ip))
    {
        Console.WriteLine("Zadali jste platnou IP adresu");
        bool isInNetwork = IsIPInNetwork(ip, network!);
        Console.WriteLine(isInNetwork ? "Zadaná adresa je v síti" : "Zadaná adresa není v síti");
    }
    else
    {
        Console.WriteLine("Zadali jste neplatnou IP adresu");
        break;
    }
}
while (true);

static bool CheckNetworkValidity (string network)
{
    if (!network.Contains('/')) return false;
    string[] parts = network.Split('/');
    if (parts.Length != 2) return false;
    if (!ValidateIpAddress(parts[0])) return false;
    if (!int.TryParse(parts[1], out int mask)) return false;
    if (mask < 0 || mask > 32) return false;
    return true;
}

static bool ValidateIpAddress (string ipAddress)
{
    string pattern = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";
    Regex regex = new Regex(pattern);

    return regex.IsMatch(ipAddress);
}

static bool IsIPInNetwork(string ip, string network)
{
    // očekává formát "x.x.x.x/x" a IPv4 adresy
    if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(network)) return false;

    var parts = network.Split('/');
    if (parts.Length != 2) return false;

    if (!ValidateIpAddress(parts[0]) || !int.TryParse(parts[1], out int mask)) return false;
    if (mask < 0 || mask > 32) return false;
    if (!ValidateIpAddress(ip)) return false;

    // převedeme na 32-bit unsigned integer (big-endian -> možný převod podle endianness)
    byte[] ipBytes = IPAddress.Parse(ip).GetAddressBytes();
    byte[] netBytes = IPAddress.Parse(parts[0]).GetAddressBytes();

    if (BitConverter.IsLittleEndian)
    {
        Array.Reverse(ipBytes);
        Array.Reverse(netBytes);
    }

    uint ipInt = BitConverter.ToUInt32(ipBytes, 0);
    uint netInt = BitConverter.ToUInt32(netBytes, 0);

    uint maskInt = mask == 0 ? 0u : (uint.MaxValue << (32 - mask));

    return (ipInt & maskInt) == (netInt & maskInt);
}
