using System.Net;
using System.Net.Sockets;

namespace IPMasking.Utilities;

/// <summary>
/// Pomocná tøída pro práci se sítìmi a IP adresami.
/// Poskytuje metody pro parsování IPv4 adres, parsování sítí ve tvaru "x.x.x.x/x"
/// a kontrolu, zda zadaná adresa patøí do dané sítì.
/// </summary>
public static class NetworkUtils
{
    /// <summary>
    /// Pokusí se parsovat text jako IPv4 adresu.
    /// </summary>
    /// <param name="text">Text s IPv4 adresou (napø. "192.168.0.1").</param>
    /// <param name="value">Vıstupní 32-bitová reprezentace adresy (bajt 0 je nejvıznamnìjší — stejné poøadí jako v zápisu "a.b.c.d").</param>
    /// <returns>true pokud je text platná IPv4 adresa, jinak false.</returns>
    /// <remarks>
    /// Metoda oøezává vstup (Trim) a pouívá <see cref="IPAddress.TryParse"/>.
    /// Vrácená hodnota v <paramref name="value"/> je v poøadí, kde nejvyšší bajt odpovídá prvnímu oktetu adresy.
    /// </remarks>
    public static bool TryParseIPv4(string? text, out uint value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(text))
            return false;

        if (!IPAddress.TryParse(text.Trim(), out var addr))
            return false;

        if (addr.AddressFamily != AddressFamily.InterNetwork)
            return false;

        value = ToUint(addr);
        return true;
    }

    /// <summary>
    /// Pokusí se parsovat sí ve formátu "adresa/maska" (napø. "192.168.0.0/24").
    /// </summary>
    /// <param name="network">Text s popisem sítì ve tvaru "x.x.x.x/x".</param>
    /// <param name="netInt">Vıstupní 32-bitová reprezentace síové adresy (normalizovaná — host bity jsou vynulovány).</param>
    /// <param name="maskInt">Vıstupní 32-bitová bitová maska s jednièkami na síové èásti (napø. /24 => 0xFFFFFF00).</param>
    /// <returns>true pokud je vstup platná sí ve formátu IPv4/CIDR, jinak false.</returns>
    /// <remarks>
    /// Metoda rozdìlí vstup podle '/' a zkontroluje, e maska je èíslo v rozsahu 0..32.
    /// Pøepoète masku na bitovou masku a normalizuje síovou adresu vynulováním host bitù pomocí bitového AND.
    /// </remarks>
    public static bool TryParseNetwork(string? network, out uint netInt, out uint maskInt)
    {
        netInt = 0;
        maskInt = 0;
        if (string.IsNullOrWhiteSpace(network))
            return false;

        var parts = network.Split('/');
        if (parts.Length != 2)
            return false;

        if (!TryParseIPv4(parts[0], out netInt))
            return false;

        if (!int.TryParse(parts[1].Trim(), out int mask) || mask < 0 || mask > 32)
            return false;

        maskInt = mask switch
        {
            0 => 0u,
            32 => uint.MaxValue,
            _ => uint.MaxValue << (32 - mask)
        };

        // normalizovat síovou adresu (vynulovat host bity)
        netInt &= maskInt;
        return true;
    }

    /// <summary>
    /// Zkontroluje, zda zadaná IPv4 adresa patøí do zadané sítì (formát sítì: "x.x.x.x/x").
    /// </summary>
    /// <param name="ip">IPv4 adresa jako text.</param>
    /// <param name="network">Sí ve formátu CIDR (napø. "10.0.0.0/8").</param>
    /// <returns>true pokud je adresa v síti; false pokud je neplatnı vstup nebo adresa není v síti.</returns>
    public static bool IsIpInNetwork(string ip, string network)
    {
        if (!TryParseIPv4(ip, out var ipVal))
            return false;

        if (!TryParseNetwork(network, out var netVal, out var maskVal))
            return false;

        return (ipVal & maskVal) == (netVal & maskVal);
    }

    /// <summary>
    /// Pøevede <see cref="IPAddress"/> (IPv4) na 32-bitové unsigned celé èíslo.
    /// </summary>
    /// <param name="address">Instanci <see cref="IPAddress"/>, která musí bıt IPv4.</param>
    /// <returns>32-bitová hodnota, kde nejvyšší bajt odpovídá prvnímu oktetu adresy.</returns>
    /// <exception cref="ArgumentException">Pokud je pøedaná adresa jiného typu ne IPv4.</exception>
    public static uint ToUint(IPAddress address)
    {
        var b = address.GetAddressBytes();
        if (b.Length != 4)
            throw new ArgumentException("Pouze IPv4 adresy jsou podporovány.", nameof(address));

        return ((uint)b[0] << 24) | ((uint)b[1] << 16) | ((uint)b[2] << 8) | b[3];
    }
}