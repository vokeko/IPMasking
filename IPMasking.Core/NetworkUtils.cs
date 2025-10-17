using System.Net;
using System.Net.Sockets;

namespace IPMasking.Core;

/// <summary>
/// Pomocná tøída pro práci se sítìmi a IP adresami
/// </summary>
public static class NetworkUtils
{
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

    public static bool IsIpInNetwork(string ip, string network)
    {
        if (!TryParseIPv4(ip, out var ipVal))
            return false;

        if (!TryParseNetwork(network, out var netVal, out var maskVal))
            return false;

        return (ipVal & maskVal) == (netVal & maskVal);
    }

    public static uint ToUint(IPAddress address)
    {
        var b = address.GetAddressBytes();
        if (b.Length != 4)
            throw new ArgumentException("Pouze IPv4 adresy jsou podporovány.", nameof(address));

        return ((uint)b[0] << 24) | ((uint)b[1] << 16) | ((uint)b[2] << 8) | b[3];
    }
}