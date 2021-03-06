﻿using Game.Server;
using IRSE.Modules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IRSE.Managers.ConsoleCommands
{
    internal class IRSECommands
    {
        [SvCommandMethod("opengui", "If closed, will open and/or focus the GUI to the front.", 5, new SvCommandMethod.ArgumentID[] { })]
        public static void c_openGui(object caller, List<string> parameters)
        {
            Program.SetupGUI();
        }

        [SvCommandMethod("irseforceupdate", "Forces an Update of IRSE with no prompts.", 5, new SvCommandMethod.ArgumentID[] { })]
        public static void c_forceUpdate(object caller, List<string> parameters)
        {
            UpdateManager.GUIMode = false;
            UpdateManager.Instance.CheckForUpdates(true).GetAwaiter().GetResult();
        }

        [SvCommandMethod("resetsteamcmd", "Run this to restart the steamcmd checking prompts.", 5, new SvCommandMethod.ArgumentID[] { })]
        public static void c_resetSteamCMD(object caller, List<string> parameters)
        {
            Config.Instance.Settings.DeclinedSteamCMDManagement = false;
            Config.Instance.Settings.HashedSteamUserName = "";
            Config.Instance.Settings.HashedSteamPassword = "";

            Config.Instance.SaveConfiguration(true);
            Program.Restart(true, true);
        }

        [SvCommandMethod("irsecheckupdate", "Checks for IRSE updates. Prompts user with new update details.", 5, new SvCommandMethod.ArgumentID[] { })]
        public static void c_checkUpdate(object caller, List<string> parameters)
        {
            UpdateManager.GUIMode = false;
            UpdateManager.Instance.CheckForUpdates().GetAwaiter().GetResult();
        }

        [SvCommandMethod("irserestart", "Restarts IRSE, if autostart is set the server will start automatically.", 5, new SvCommandMethod.ArgumentID[] { })]
        public static void c_restart(object caller, List<string> parameters)
        {
            Program.Restart();
        }

        [SvCommandMethod("stop", "Stops the server if its running!", 5, new SvCommandMethod.ArgumentID[] { })]
        public static void c_stop(object caller, List<string> parameters)
        {
            if (ServerInstance.Instance.IsRunning)
                ServerInstance.Instance.Stop();
            else
                Console.WriteLine("The server is not running");
        }

        [SvCommandMethod("start", "Starts the server if its not running!", 5, new SvCommandMethod.ArgumentID[] { })]
        public static void c_start(object caller, List<string> parameters)
        {
            if (!ServerInstance.Instance.IsRunning)
                ServerInstance.Instance.Start();
            else
                Console.WriteLine("The server is already running");
        }

        [SvCommandMethod("closeirse", "Attempts to shutdown and servers, then gracefully closes irse.", 5, new SvCommandMethod.ArgumentID[] { })]
        public static void c_closeIrse(object caller, List<string> parameters)
        {
            Program.CloseIRSE();
        }
    }
}