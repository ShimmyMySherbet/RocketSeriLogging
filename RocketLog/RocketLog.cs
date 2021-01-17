using System;
using System.IO;
using HarmonyLib;
using SDG.Framework.Modules;
using SDG.Unturned;
using Serilog;
using RocketLogger = Rocket.Core.Logging.Logger;
using SeriLogger = Serilog.Core.Logger;

namespace RocketLog
{
    public class RocketLog : IModuleNexus
    {
        public static RocketLog Instance;
        public SeriLogger SeriLogger;
        public Harmony Harmony;

        public void initialize()
        {
            Instance = this;
            LoggerConfiguration config = new LoggerConfiguration().WriteTo.Console().WriteTo.File(Path.Combine(PathHelper.LogDirectory, "RocketLog._.log"), Serilog.Events.LogEventLevel.Debug, rollingInterval: RollingInterval.Day);
            SeriLogger = config.CreateLogger();
            SeriLogger.Information("Loading SeriLogger...");
            Harmony = new Harmony("RocketLog");
            InitLogger();
        }

        public void shutdown()
        {
            Harmony.UnpatchAll("RocketLog");
        }

        public void InitLogger()
        {
            SeriLogger.Information("Patching Log Methods...");

            // RocketMod
            Harmony.Patch(ReflectionHelper.FindMethod<RocketLogger, string, bool>("Log"), ReflectionHelper.FindHMethod<LogPatches>("Patch_Log1"));
            Harmony.Patch(ReflectionHelper.FindMethod<RocketLogger, string, ConsoleColor>("Log"), ReflectionHelper.FindHMethod<LogPatches>("Patch_Log2"));
            Harmony.Patch(ReflectionHelper.FindMethod<RocketLogger, string>("LogWarning"), ReflectionHelper.FindHMethod<LogPatches>("Patch_LogWarning"));
            Harmony.Patch(ReflectionHelper.FindMethod<RocketLogger, string>("LogError"), ReflectionHelper.FindHMethod<LogPatches>("Patch_LogError1"));
            Harmony.Patch(ReflectionHelper.FindMethod<RocketLogger, Exception, string>("LogError"), ReflectionHelper.FindHMethod<LogPatches>("Patch_LogError2"));
            // CommandWindow
            Harmony.Patch(ReflectionHelper.FindMethod<CommandWindow, object>("Log"), ReflectionHelper.FindHMethod<LogPatches>("Patch_U_Log"));
            Harmony.Patch(ReflectionHelper.FindMethod<CommandWindow, string, object[]>("LogFormat"), ReflectionHelper.FindHMethod<LogPatches>("Patch_U_LogFormat"));
            Harmony.Patch(ReflectionHelper.FindMethod<CommandWindow, object>("LogWarning"), ReflectionHelper.FindHMethod<LogPatches>("Patch_U_LogWarning"));
            Harmony.Patch(ReflectionHelper.FindMethod<CommandWindow, string, object[]>("LogWarningFormat"), ReflectionHelper.FindHMethod<LogPatches>("Patch_U_LogWarningFormat"));
            Harmony.Patch(ReflectionHelper.FindMethod<CommandWindow, object>("LogError"), ReflectionHelper.FindHMethod<LogPatches>("Patch_U_LogError"));
            Harmony.Patch(ReflectionHelper.FindMethod<CommandWindow, string, object[]>("LogErrorFormat"), ReflectionHelper.FindHMethod<LogPatches>("Patch_U_LogErrorFormat"));
            // UnturnedLog
            Harmony.Patch(ReflectionHelper.FindMethod<string>("info", typeof(UnturnedLog)), ReflectionHelper.FindHMethod<LogPatches>("Patch_UL_info"));
            Harmony.Patch(ReflectionHelper.FindMethod<string>("warn", typeof(UnturnedLog)), ReflectionHelper.FindHMethod<LogPatches>("Patch_UL_warn"));
            Harmony.Patch(ReflectionHelper.FindMethod<string>("error", typeof(UnturnedLog)), ReflectionHelper.FindHMethod<LogPatches>("Patch_UL_error"));
            Harmony.Patch(ReflectionHelper.FindMethod<Exception>("exception", typeof(UnturnedLog)), ReflectionHelper.FindHMethod<LogPatches>("Patch_UL_exception1"));
            Harmony.Patch(ReflectionHelper.FindMethod<Exception, string, object[]>("exception", typeof(UnturnedLog)), ReflectionHelper.FindHMethod<LogPatches>("Patch_UL_exception2"));
            Harmony.Patch(ReflectionHelper.FindMethod<object>("info", typeof(UnturnedLog)), ReflectionHelper.FindHMethod<LogPatches>("Patch_UL_object_info"));
            Harmony.Patch(ReflectionHelper.FindMethod<object>("warn", typeof(UnturnedLog)), ReflectionHelper.FindHMethod<LogPatches>("Patch_UL_object_warn"));
            Harmony.Patch(ReflectionHelper.FindMethod<object>("error", typeof(UnturnedLog)), ReflectionHelper.FindHMethod<LogPatches>("Patch_UL_object_error"));
            Harmony.Patch(ReflectionHelper.FindMethod<string, object[]>("info", typeof(UnturnedLog)), ReflectionHelper.FindHMethod<LogPatches>("Patch_UL_format_info"));
            Harmony.Patch(ReflectionHelper.FindMethod<string, object[]>("warn", typeof(UnturnedLog)), ReflectionHelper.FindHMethod<LogPatches>("Patch_UL_format_warn"));
            Harmony.Patch(ReflectionHelper.FindMethod<string, object[]>("error", typeof(UnturnedLog)), ReflectionHelper.FindHMethod<LogPatches>("Patch_UL_format_error"));

            SeriLogger.Information("Patched 22 methods.");

        }
    }

    public class LogPatches
    {
        public static SeriLogger Logger => RocketLog.Instance.SeriLogger;
        #region "RocketLogger"
        // Rocket Logger
        public static bool Patch_Log1(string message, bool sendToConsole)
        {
            Logger.Write(Serilog.Events.LogEventLevel.Information, message);
            return false;
        }

        public static bool Patch_Log2(string message, ConsoleColor color = ConsoleColor.White)
        {
            Logger.Write(Serilog.Events.LogEventLevel.Information, message);
            return false;
        }

        public static bool Patch_LogWarning(string message)
        {
            Logger.Write(Serilog.Events.LogEventLevel.Warning, message);
            return false;
        }

        public static bool Patch_LogError1(string message)
        {
            Logger.Write(Serilog.Events.LogEventLevel.Error, message);
            return false;
        }

        public static bool Patch_LogError2(Exception ex, string v)
        {
            Logger.Write(Serilog.Events.LogEventLevel.Error, v, ex);
            return false;
        }
        #endregion
        #region "command window"
        // Command Window

        public static bool Patch_U_Log(object text)
        {
            if (text is string str)
            {
                Logger.Information(str);
            } else
            {
                Logger.Information("{0}", text);
            }
            return false;
        }

        public static bool Patch_U_LogFormat(string format, params object[] args)
        {
            Logger.Information(format, args);
            return false;
        }
        public static bool Patch_U_LogWarning(object text)
        {
            if (text is string str)
            {
                Logger.Warning(str);
            }
            else
            {
                Logger.Warning("{0}", text);
            }
            return false;
        }
        public static bool Patch_U_LogWarningFormat(string format, params object[] args)
        {
            Logger.Warning(format, args);
            return false;
        }

        public static bool Patch_U_LogError(object text)
        {
            if (text is string str)
            {
                Logger.Error(str);
            }
            else if (text is Exception ex)
            {
                Logger.Error("", ex);
            }
            else
            {
                Logger.Error($"{0}", text);
            }
            return false;
        }

        public static bool Patch_U_LogErrorFormat(string format, params object[] args)
        {
            Logger.Error(format, args);
            return false;
        }
        #endregion
        #region "Unturned Log"
        // Unturned Log

        public static bool Patch_UL_info(string message)
        {
            Logger.Information(message);
            return false;
        }

        public static bool Patch_UL_warn(string message)
        {
            Logger.Warning(message);
            return false;
        }

        public static bool Patch_UL_error(string message)
        {
            Logger.Error(message);
            return false;
        }


        public static bool Patch_UL_exception1(Exception e)
        {
            Logger.Error("", e);
            return false;
        }


        public static bool Patch_UL_exception2(Exception e, string format, params object[] args)
        {
            Logger.Error(format, args);
            Logger.Error("", e);
            return false;
        }


        public static bool Patch_UL_object_info(object message)
        {
            if (message is string str)
            {
                Logger.Information(str);
            }
            else
            {
                Logger.Information("{0}", message);
            }
            return false;
        }

        public static bool Patch_UL_object_warn(object message)
        {
            if (message is string str)
            {
                Logger.Warning(str);
            }
            else
            {
                Logger.Warning("{0}", message);
            }
            return false;
        }

        public static bool Patch_UL_object_error(object message)
        {
            if (message is string str)
            {
                Logger.Error(str);
            }
            else
            {
                Logger.Error("{0}", message);
            }
            return false;
        }


        public static bool Patch_UL_format_info(string format, params object[] args)
        {
            Logger.Information(format, args);
            return false;
        }

        public static bool Patch_UL_format_warn(string format, params object[] args)
        {
            Logger.Warning(format, args);
            return false;
        }

        public static bool Patch_UL_format_error(string format, params object[] args)
        {
            Logger.Error(format, args);
            return false;
        }
        #endregion
    }

}