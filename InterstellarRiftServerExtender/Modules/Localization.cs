﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Resources;

namespace IRSE.Modules
{
    public class Localization
    {
        public static string PathFolder = Path.Combine(FolderStructure.IRSEFolderPath, "localization");
        public static Version Version = new Version("0.0.0.5");
        private Dictionary<string, string> m_sentences = new Dictionary<string, string>();
        private static NLog.Logger mainLog;

        public Localization()
        {
            mainLog = NLog.LogManager.GetCurrentClassLogger();
        }

        public Dictionary<string, string> Sentences
        {
            get
            {
                try
                {
                    return this.m_sentences;
                }
                catch (Exception)
                {
                    Console.WriteLine("Missing Localization Key");
                    
                }
                return null;
            }
        }

        public void Load(string FileName)
        {
            string path = Path.Combine(FolderStructure.IRSEFolderPath, "localization", FileName + ".resx");

            if (File.Exists(path))
            {
                if (Version.Parse(new ResXResourceSet(path).GetString("version")) == Localization.Version)
                {
                    using (ResXResourceReader resXresourceReader = new ResXResourceReader(path))
                    {
                        foreach (DictionaryEntry dictionaryEntry in resXresourceReader)
                            this.m_sentences.Add((string)dictionaryEntry.Key, (string)dictionaryEntry.Value);
                    }
                }
                else if(FileName.StartsWith("En") && Version.Parse(new ResXResourceSet(path).GetString("version")) < Localization.Version)
                {
                    mainLog.Info("English language localization file was updated. Loading new one.");
                    Localization.CreateDefault();
                    this.Load("En");
                }
                else
                {
                    mainLog.Info("Your localization file is not updated ! Please download the latest version on our github page. English language loading...");
                    Localization.CreateDefault();
                    this.Load("En");
                }
            }
            else
            {
                mainLog.Info("No localization file detected ! English language loading...");
                Localization.CreateDefault();
                this.Load("En");
            }
        }

        public static void CreateDefault()
        {
            string path = Path.Combine(FolderStructure.IRSEFolderPath, "localization", "En.resx");

            if (File.Exists(path))
                File.Delete(path);

            File.Create(path).Close();
            using (ResXResourceWriter resXresourceWriter = new ResXResourceWriter(path))
            {

                // Program.cs
                resXresourceWriter.AddResource("version", Localization.Version.ToString());
               
                resXresourceWriter.AddResource("Initialization", "Interstellar Rift Extended Server v{0} Initializing....");
                resXresourceWriter.AddResource("IRNotFound", "IRSE: IR.EXE wasn't found.\nMake sure IRSE.exe is in the same folder as IR.exe.\nPress enter to close.");
                resXresourceWriter.AddResource("GameConsoleEnabled", "IRSE: Game Console Commands Enabled.");
                resXresourceWriter.AddResource("ServerIsAlreadyRunning", "The server is already running!");
                resXresourceWriter.AddResource("StopRunningServers", "Attempting to stop any running servers.");
                // Program.cs - Command Line Args
                resXresourceWriter.AddResource("UseDevVersion", "IRSE: (Arg: -usedevversion is set) IRSE Will use Pre-releases versions");
                resXresourceWriter.AddResource("NoUpdate", "IRSE: (Arg: -noupdate is set or option in IRSE config is enabled) IRSE will not be auto-updated.");               
                resXresourceWriter.AddResource("NoUpdateIR", "IRSE: (Arg: -noupdateir is set) IsR Dedicated Server will not be auto-updated.");
                resXresourceWriter.AddResource("AutoStart", "IRSE: Arg: -autostart or Gui's Autostart Checkbox was Checked)");
                resXresourceWriter.AddResource("NonInteractive", "Non interactive environment detected, GUI disabled");
                resXresourceWriter.AddResource("GUIDisabled", "IRSE: GUI Disabled");
                resXresourceWriter.AddResource("CommandNoExist", "IRSE: command Doesn't Exist.");
                resXresourceWriter.AddResource("CommandException", "IRSE: Command exception!");
                // Program.cs - Versioning
                resXresourceWriter.AddResource("ForGameVersion", "For Game Version: ");
                resXresourceWriter.AddResource("ThisGameVersion", "This Game Version: ");
                resXresourceWriter.AddResource("OnlineGameVersion", "Online Game Version: ");
                resXresourceWriter.AddResource("NewIRVersion", "There is a new version of Interstellar Rift! Update your IR Installation!");
                resXresourceWriter.AddResource("IRNewer", "Interstellar Rifts Version is newer than what this version of IRSE Supports, Check for IRSE updates!");
                // Program.cs - Console Commands
                resXresourceWriter.AddResource("LoadingGUI", "Loading GUI...");
                resXresourceWriter.AddResource("HelpCommand", "help - this page ;)");
                resXresourceWriter.AddResource("OpenGUICommand", "opengui - If closed, will open and/or focus the GUI to the front.");
                resXresourceWriter.AddResource("StartCommand", "start - Starts the server if its not running!");
                resXresourceWriter.AddResource("StopCommand",  "stop - stops the server if it's running!");
                resXresourceWriter.AddResource("RestartCommand", "restart - Restarts IRSE, if autostart is set the server will start automatically.");
                resXresourceWriter.AddResource("CheckUpdateCommand", "checkupdate - Checks for IRSE updates. Prompts user with new update details.");
                resXresourceWriter.AddResource("ForceUpdateCommand", "forceupdate - Forces an Update of IRSE with no prompts.");


                // PluginManager.cs
                resXresourceWriter.AddResource("FailedInitPlugin", "Failed initialization of Plugin {0}. Exception: {1}");
                resXresourceWriter.AddResource("FailedLoadPlugin", "Failed load of Plugin {0}. Exception: {1}");
                resXresourceWriter.AddResource("FailedLoadAssembly", "Failed to load assembly : {0} : {1}");
                resXresourceWriter.AddResource("FailedShutdownPlugin", "Uncaught Exception in Plugin {0}. Exception: {1}");
                resXresourceWriter.AddResource("InitializationPlugin", "Initialization of Plugin {0} failed. Could not find a public, parameterless constructor for {0}");
                resXresourceWriter.AddResource("InitializingPlugin", "Initializing Plugin : {0}");
                resXresourceWriter.AddResource("LoadingPlugin", "Loading Plugin : {0}");
                resXresourceWriter.AddResource("ShutdownPlugin", "Shutting down Plugin {0}");
            }
        }
    }
}