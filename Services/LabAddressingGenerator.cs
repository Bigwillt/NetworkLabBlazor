using NetworkLabBlazor.Models;

namespace NetworkLabBlazor.Services;

public static class LabAddressingGenerator
{
    private static readonly Random Rand = new();

    // Generate a random /30 WAN network (e.g., 172.16.100.0/30)
    private static (string Net, string R1IP, string R2IP, string Mask) GenerateWan30()
    {
        int third = Rand.Next(0, 255);
        int fourth = (Rand.Next(0, 63) * 4); // multiples of 4 for /30

        string network = $"172.16.{third}.{fourth}";
        string r1 = $"172.16.{third}.{fourth + 1}";
        string r2 = $"172.16.{third}.{fourth + 2}";
        string mask = "255.255.255.252";

        return (network, r1, r2, mask);
    }

    // Generate a random LAN subnet (e.g., 10.10.X.0/26)
    private static (string Net, string Mask, string Gateway, string HostIP, string SwitchIP) GenerateLan()
    {
        int third = Rand.Next(0, 255);
        int block = Rand.Next(0, 4) * 64;

        string network = $"10.10.{third}.{block}";
        string mask = "255.255.255.192";
        string gateway = $"10.10.{third}.{block + 1}";
        string switchIP = $"10.10.{third}.{block + 2}";
        string host = $"10.10.{third}.{block + Rand.Next(10, 62)}";

        return (network, mask, gateway, host, switchIP);
    }

    public static List<AddressGroup> Generate()
    {
        var wan = GenerateWan30();
        var lan1 = GenerateLan();
        var lan2 = GenerateLan();

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
                    new() { Interface = "F0/1", IP = lan1.SwitchIP, Subnet = lan1.Mask, Gateway = lan1.Gateway }
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
                    new() { Interface = "", IP = lan1.HostIP, Subnet = lan1.Mask, Gateway = lan1.Gateway }
                }
            },
            new AddressGroup
            {
                Device = "PC2",
                Rows = new()
                {
                    new() { Interface = "", IP = lan2.HostIP, Subnet = lan2.Mask, Gateway = lan2.Gateway }
                }
            },
        };
    }
}
