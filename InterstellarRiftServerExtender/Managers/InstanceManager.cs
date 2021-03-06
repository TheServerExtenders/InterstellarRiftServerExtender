﻿using Game.Configuration;
using Game.Framework;
using Game.Framework.Threading;
using Game.GameStates;
using Game.Server;
using Game.Universe;
using IRSE.Managers.Events;
using IRSE.Modules;
using IRSE.ReflectionWrappers.ServerWrappers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace IRSE.Managers
{
    public class ServerInstance
    {
        #region Fields

        private static Thread m_serverThread;
        private Assembly m_assembly;
        private Assembly m_frameworkAssembly;
        private static ServerInstance m_serverInstance;
        private ServerWrapper m_serverWrapper;
        private HandlerManager m_handlerManager;
        private DateTime m_launchedTime;
        private static NLog.Logger mainLog; //mainLog.Error

        private bool m_isRunning;
        private bool m_isStarting;

        private PluginManager m_pluginManager = null;

        public static List<string> SystemNames = new List<string>(new string[] { "Vectron Syx", "Alpha Ventura", "Sentinel Prime", "Scaverion" });

        #endregion Fields

        #region Properties

        public static ServerInstance Instance { get { return m_serverInstance; } }

        public HandlerManager Handlers { get { return m_handlerManager; } }
        public PluginManager PluginManager { get { return m_pluginManager; } internal set { } }

        public GameServer Server { get; private set; }

        public Assembly Assembly { get { return m_assembly; } }

        public TimeSpan Uptime { get { return DateTime.Now - m_launchedTime; } }

        private void SetIsRunning(bool run = true)
        {
            m_isRunning = run;
            if (run)
                OnServerStarted?.Invoke();
            else
                OnServerStopped?.Invoke();
        }

        internal void SetIsStarting(bool starting = true)
        {
            m_isStarting = starting;
            if (starting)
                OnServerStarting?.Invoke();
        }

        public Boolean IsRunning => m_isRunning;

        public Boolean IsStarting => m_isStarting;

        #endregion Properties

        #region Events

        public delegate void ServerRunningEvent();

        public event ServerRunningEvent OnServerStarted;

        public event ServerRunningEvent OnServerStopped;

        public event ServerRunningEvent OnServerStarting;

        #endregion Events

        public ServerInstance()
        {
            m_serverInstance = this;

            mainLog = NLog.LogManager.GetCurrentClassLogger();

            m_launchedTime = DateTime.MinValue;
            m_serverThread = null;

            // Wrap IR.exe
            m_assembly = Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IR.exe"));

            // Wrap Aluna Framework as GameState was moved here.
            m_frameworkAssembly = Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AlunaNetFramework.dll"));

            // Wrap both assemblies
            m_serverWrapper = new ServerWrapper(m_assembly, m_frameworkAssembly);

            m_pluginManager = new PluginManager();
        }

        #region Methods

        private void WaitForHandlers()
        {
            // Get the Handlers from the Controller
            while (m_handlerManager.GetHandlers() == null)
            {
                try
                {
                    Thread.Sleep(1000);
                    if (m_handlerManager.GetHandlers() != null)
                    {
                        Server = m_handlerManager.GetHandlers();
                        break;
                    }
                }
                catch (Exception)
                {
                    break;
                }
            }
        }

        internal void Hook()
        {
            m_launchedTime = DateTime.Now;
            try
            {
                m_handlerManager = new HandlerManager(m_assembly, m_frameworkAssembly);

                WaitForHandlers();

                ControllerManager controllerManager = m_handlerManager.ControllerManager;

                // Intercept events to copy and invoke
                EventManager.Instance.Intercept(controllerManager);

                // ir command loader
                mainLog.Info("IRSE: Loading Game Console Commands..");
                ConsoleCommandManager.InitAndReplace(controllerManager);

                Program.ConsoleCoroutine = ConsoleCommandManager.IRSECommandSystem
                    .Logic(controllerManager, Game.Configuration.Globals.NoConsoleAutoComplete || Program.CommandLineArgs.Contains("-noConsoleAutoComplete"));

                // plugin loader
                mainLog.Info("IRSE: Initializing Plugins...");
                m_pluginManager.InitializeAllPlugins();

                // Wait 5 seconds before activating ServerInstance.Instance.IsRunning
                mainLog.Info("IRSE: Startup Procedure Complete!");

                Program.Wait = false;

                SetIsRunning(); // Server is running by now
                SetIsStarting(false);

                Program.SetTitle(true);
            }
            catch (Exception ex)
            {
                mainLog.Info("IRSE: Startup Procedure FAILED!");
                mainLog.Info("IRSE: Haulting Server.. Major problem detected!!!!!!!! - Exception:");
                mainLog.Error(ex.ToString());

                Stop();
            }
        }

        public void Start()
        {
            if (IsRunning)
                return;

            List<string> serverArgs = Program.CommandLineArgs;

            serverArgs.Add("-server");

            m_serverThread = ServerWrapper.Program.StartServer(serverArgs.ToArray());
            m_serverWrapper.Init();
        }

        public void Stop(bool killProcess = false)
        {
            if (IsRunning)
            {
                Console.WriteLine("Shutting Down IR.");
                Thread.Sleep(2000);
                SetIsRunning(false);

                try
                {
                    Console.WriteLine("Saving Galaxy...");
                    Save();
                    Console.WriteLine("Shutting down plugins...");
                    PluginManager.ShutdownAllPlugins();
                    Console.WriteLine("Stopping server..");
                    ServerWrapper.Program.StopServer();

                    if (killProcess) Process.GetCurrentProcess().Kill();
                }
                catch (Exception)
                {
                }
            }
        }

        internal void Save()
        {
            (Handlers.ControllerManager.Universe.Galaxy as ServerGalaxy).SaveGalaxy(Handlers.ControllerManager, "user", Handlers.ControllerManager.Universe.Galaxy.Name.ToLower(), true);
        }

        #endregion Methods
    }
}