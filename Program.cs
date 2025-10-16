using System;
using System.Net;
using System.Net.Sockets;

string? network;
uint networkAddress = 0;
uint networkMask = 0;

while (true)
{
    Console.WriteLine("Zadejte síť ve formátu x.x.x.x/x (prázdné pro ukončení)");
    network = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(network))
        return;

    if (TryParseNetwork(network, out networkAddress, out networkMask))
    {
        Console.WriteLine("Zadali jste platnou síť");
        break;
    }

    Console.WriteLine("Zadali jste neplatnou síť");
}

while (true)
{
    Console.WriteLine("Zadejte IP adresu (prázdné pro ukončení)");
    string? ip = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(ip))
        break;

    if (TryParseIPv4(ip, out uint ipValue))
    {
        Console.WriteLine("Zadali jste platnou IP adresu");
        bool isInNetwork = (ipValue & networkMask) == (networkAddress & networkMask);
        Console.WriteLine(isInNetwork ? "Zadaná adresa je v síti" : "Zadaná adresa není v síti");
    }
    else
    {
        Console.WriteLine("Zadali jste neplatnou IP adresu");
    }
}

static bool TryParseIPv4(string text, out uint value)
{
    value = 0;
    if (!IPAddress.TryParse(text, out var addr))
        return false;
    if (addr.AddressFamily != AddressFamily.InterNetwork)
        return false;

    value = ToUint(addr);
    return true;
}

static bool TryParseNetwork(string network, out uint netInt, out uint maskInt)
{
    netInt = 0;
    maskInt = 0;
    if (string.IsNullOrEmpty(network))
        return false;

    var parts = network.Split('/');
    if (parts.Length != 2)
        return false;

    if (!TryParseIPv4(parts[0], out netInt))
        return false;

    if (!int.TryParse(parts[1], out int mask))
        return false;
    if (mask < 0 || mask > 32)
        return false;

    if (mask == 0)
        maskInt = 0;
    else if (mask == 32)
        maskInt = uint.MaxValue;
    else
        maskInt = uint.MaxValue << (32 - mask);

    return true;
}

static uint ToUint(IPAddress address)
{
    var b = address.GetAddressBytes();
    // explicit pořadí bajtů -> nezávislé na endianness systému
    return ((uint)b[0] << 24) | ((uint)b[1] << 16) | ((uint)b[2] << 8) | b[3];
}