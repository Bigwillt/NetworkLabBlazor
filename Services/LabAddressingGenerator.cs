using NetworkLabBlazor.Models;

namespace NetworkLabBlazor.Services;

public static class LabAddressingGenerator
{
    //private static readonly Random Rand = new();

    // Generate a random /30 WAN network (e.g., 172.16.100.0/30)
    private static (string Net, string R1IP, string R2IP, string Mask) GenerateWan30(Random rng)
    {
        int third = rng.Next(0, 255);
        int fourth = (rng.Next(0, 63) * 4); // multiples of 4 for /30

        string network = $"172.16.{third}.{fourth}";
        string r1 = $"172.16.{third}.{fourth + 1}";
        string r2 = $"172.16.{third}.{fourth + 2}";
        string mask = "255.255.255.252";

        return (network, r1, r2, mask);
    }

    // Generate a random LAN subnet (e.g., 10.10.X.0/26)
    private static (string Net, string Mask, string Gateway, string HostIP, string SwitchIP) GenerateLan(Random rng)
    {
        int third = rng.Next(0, 255);

        string network = $"10.10.{third}.0";
        string mask = "255.255.255.0";

        // Router LAN interface (this IS the default gateway)
        string gateway = $"10.10.{third}.1";

        // Switch management IP
        string switchIP = $"10.10.{third}.2";

        // Random host
        string host = $"10.10.{third}.{rng.Next(10, 254)}";

        return (network, mask, gateway, host, switchIP);
    }

    public static List<AddressGroup> Generate(int? seed = null)
    {
        Random rng = seed.HasValue ? new Random(seed.Value) : new Random();
        var wan = GenerateWan30(rng);
        var lan1 = GenerateLan(rng);
        var lan2 = GenerateLan(rng);
        
        return new()
        {
            new AddressGroup
            {
                Device = "R1",
                Rows = new()
                {
                    new() { Interface = "G0/0", IP = lan1.Gateway, Subnet = lan1.Mask, Gateway = "" },
                    new() { Interface = "G0/1 (WAN)", IP = wan.R1IP, Subnet = wan.Mask, Gateway = "" }
                }
            },
            new AddressGroup
            {
                Device = "R2",
                Rows = new()
                {
                    new() { Interface = "G0/0", IP = lan2.Gateway, Subnet = lan2.Mask, Gateway = "" },
                    new() { Interface = "G0/1 (WAN)", IP = wan.R2IP, Subnet = wan.Mask, Gateway = "" }
                }
            },
            new AddressGroup
            {
                Device = "S1",
                Rows = new()
                {
                    new() { Interface = "F0/1", IP = lan1.SwitchIP, Subnet = lan1.Mask, Gateway = "" }
                }
            },

            new AddressGroup
            {
                Device = "S2",
                Rows = new()
                {
                    new() { Interface = "F0/1", IP = lan2.SwitchIP, Subnet = lan2.Mask, Gateway = "" }
                }
            },
            new AddressGroup
            {
                Device = "PC1",
                Rows = new()
                {
                    new() { Interface = "", IP = lan1.HostIP, Subnet = lan1.Mask, Gateway = "" }
                }
            },
            new AddressGroup
            {
                Device = "PC2",
                Rows = new()
                {
                    new() { Interface = "", IP = lan2.HostIP, Subnet = lan2.Mask, Gateway = "" }
                }
            },
        };
    }
}
