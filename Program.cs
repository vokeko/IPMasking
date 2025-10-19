using IPMasking.Utilities;

string? network;
uint networkAddress;
uint networkMask;

while (true)
{
    Console.WriteLine("Zadejte síť ve formátu x.x.x.x/x");
    network = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(network))
        return;

    if (NetworkUtils.TryParseNetwork(network, out networkAddress, out networkMask))
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

    if (NetworkUtils.TryParseIPv4(ip, out uint ipValue))
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