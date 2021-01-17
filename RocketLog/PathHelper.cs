using System;
using System.IO;
using SDG.Unturned;

namespace RocketLog
{
    public static class PathHelper
    {
        public static readonly string UnturnedDirectory = Environment.CurrentDirectory;

        public static string ModulesDirectory
        {
            get
            {
                return Path.Combine(UnturnedDirectory, "Modules");
            }
        }

        public static string ProfilerDirectory
        {
            get
            {
                return Path.Combine(ModulesDirectory, "UnturnedMapMigrator");
            }
        }

        public static string ConfigFile
        {
            get
            {
                return Path.Combine(ProfilerDirectory, "Config.ini");
            }
        }

        public static string ServersDirectory
        {
            get
            {
                return Path.Combine(UnturnedDirectory, "Servers");
            }
        }

        public static string ServerDirectory
        {
            get
            {
                return Path.Combine(ServersDirectory, Provider.serverID);
            }
        }

        public static string LogDirectory
        {
            get
            {
                return Path.Combine(ServerDirectory, "Rocket", "Logs");
            }
        }
    }
}