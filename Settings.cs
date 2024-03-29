using System;

namespace uni_elastic_manager
{
    public class Settings
    {
        public string Prometheus => Environment.GetEnvironmentVariable("Prometheus");
        public string Instance => Environment.GetEnvironmentVariable("Instance");
        public string Metric => Environment.GetEnvironmentVariable("Metric");
        public string OS => Environment.GetEnvironmentVariable("OS");
        public string Name => Environment.GetEnvironmentVariable("Name");
        public string CPUThresholdUpper => Environment.GetEnvironmentVariable("CPUThresholdUpper");
        public string CPUThresholdLower => Environment.GetEnvironmentVariable("CPUThresholdLower");
        public string Runnable => Environment.GetEnvironmentVariable("Runnable");
        public string SocketDocker => Environment.GetEnvironmentVariable("SocketDocker");
        public string Image => Environment.GetEnvironmentVariable("Image");
        public bool IsFake => bool.Parse(Environment.GetEnvironmentVariable("IsFake"));
        public bool BinPacking => bool.Parse(Environment.GetEnvironmentVariable("BinPacking"));
        public string SwarmCommand => Environment.GetEnvironmentVariable("SwarmCommand");
        public string SSHLogin => Environment.GetEnvironmentVariable("SSHLogin");
        public string SSHPass => Environment.GetEnvironmentVariable("SSHPass");
        
    }
}