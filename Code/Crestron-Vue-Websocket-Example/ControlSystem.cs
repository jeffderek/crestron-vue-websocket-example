// <copyright file="ControlSystem.cs" company="jeffderek">
// The MIT License (MIT)
// Copyright (c) Jeff McAleer
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software
// and associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using System;
using Crestron.SimplSharp;                              // For Basic SIMPL# Classes
using Crestron.SimplSharpPro;                           // For Basic SIMPL#Pro classes
using Crestron.SimplSharpPro.CrestronThread;            // For Threading
using Evands.Pellucid;
using Evands.Pellucid.Diagnostics;
using Evands.Pellucid.Terminal.Commands;

namespace Crestron_Vue_Websocket_Example
{
    /// <summary>
    /// Control System.
    /// </summary>
    public class ControlSystem : CrestronControlSystem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlSystem"/> class.
        /// ControlSystem Constructor. Starting point for the SIMPL#Pro program.
        /// Use the constructor to:
        /// * Initialize the maximum number of threads (max = 400)
        /// * Register devices
        /// * Register event handlers
        /// * Add Console Commands
        ///
        /// Please be aware that the constructor needs to exit quickly; if it doesn't
        /// exit in time, the SIMPL#Pro program will exit.
        ///
        /// You cannot send / receive data in the constructor.
        /// </summary>
        public ControlSystem()
            : base()
        {
            try
            {
                Thread.MaxNumberOfUserThreads = 20;

                // Subscribe to the controller events (System, Program, and Ethernet)
                CrestronEnvironment.SystemEventHandler += new SystemEventHandler(ControllerSystemEventHandler);
                CrestronEnvironment.ProgramStatusEventHandler += new ProgramStatusEventHandler(ControllerProgramEventHandler);
                CrestronEnvironment.EthernetEventHandler += new EthernetEventHandler(ControllerEthernetEventHandler);

                // Register the Pellucid log writer
                Logger.RegisterLogWriter(new CrestronLogWriter());

                // Set the Pellucid log levels based on the Debug/Release status
#if DEBUG
                Options.Instance.LogLevels = LogLevels.All;
                Options.Instance.DebugLevels = DebugLevels.All;
#else
                Evands.Pellucid.Options.Instance.LogLevels = LogLevels.AllButDebug;
                Evands.Pellucid.Options.Instance.DebugLevels = DebugLevels.AllButDebug;
#endif

                // Set up terminal formatting
                Evands.Pellucid.Terminal.Formatting.Formatters.Chrome = new Evands.Pellucid.Terminal.Formatting.RoundedChrome();

                // Set up the global commands
                GlobalCommand = new GlobalCommand("app", "Application Commands.", Access.Programmer);
                GlobalCommand.AddToConsole();
                Console.InitializeConsole(GlobalCommand.Name);
                GlobalCommand.CommandExceptionEncountered += (sender, e) =>
                {
                    this.LogException(e.Exception, $"Error handling CommandContent: {e.CommandContent}");
                };
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in the constructor: {0}", e.Message);
            }
        }

        /// <summary>
        /// Gets the Global Console Command for Pellucid Console.
        /// </summary>
        public GlobalCommand GlobalCommand { get; private set; }

        /// <summary>
        /// Gets the SystemManager that runs the whole system.
        /// </summary>
        public SystemManager SystemManager { get; private set; }

        /// <summary>
        /// InitializeSystem - this method gets called after the constructor
        /// has finished.
        ///
        /// Use InitializeSystem to:
        /// * Start threads
        /// * Configure ports, such as serial and verisports
        /// * Start and initialize socket connections
        /// Send initial device configurations
        ///
        /// Please be aware that InitializeSystem needs to exit quickly also;
        /// if it doesn't exit in time, the SIMPL#Pro program will exit.
        /// </summary>
        public override void InitializeSystem()
        {
            try
            {
                System.Threading.ThreadPool.QueueUserWorkItem(_ =>
                {
                    // Wait for InitializeSystem to exit and for the program to report as started
                    Thread.Sleep(750);

                    try
                    {
                        // Create the SystemManager
                        SystemManager = new SystemManager(this);
                    }
                    catch (Exception e)
                    {
                        Logger.LogException(this, e, "Error in Primary Thread");
                    }
                });
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in InitializeSystem: {0}", e.Message);
            }
        }

        /// <summary>
        /// Event Handler for Ethernet events: Link Up and Link Down.
        /// Use these events to close / re-open sockets, etc.
        /// </summary>
        /// <param name="ethernetEventArgs">This parameter holds the values
        /// such as whether it's a Link Up or Link Down event. It will also indicate
        /// wich Ethernet adapter this event belongs to.
        /// </param>
        public void ControllerEthernetEventHandler(EthernetEventArgs ethernetEventArgs)
        {
            switch (ethernetEventArgs.EthernetEventType)
            {
                // Determine the event type Link Up or Link Down
                case eEthernetEventType.LinkDown:
                {
                    // Next need to determine which adapter the event is for.
                    // LAN is the adapter is the port connected to external networks.
                    if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetLANAdapter)
                    {
                        ErrorLog.Notice("LAN Link Down");
                    }
                    else if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetCSAdapter)
                    {
                        ErrorLog.Notice("CS Link Down");
                    }

                    break;
                }

                case eEthernetEventType.LinkUp:
                {
                    if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetLANAdapter)
                    {
                        ErrorLog.Notice("LAN Link Up");
                    }
                    else if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetCSAdapter)
                    {
                        ErrorLog.Notice("CS Link Up");
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Event Handler for Programmatic events: Stop, Pause, Resume.
        /// Use this event to clean up when a program is stopping, pausing, and resuming.
        /// This event only applies to this SIMPL#Pro program, it doesn't receive events
        /// for other programs stopping.
        /// </summary>
        /// <param name="programStatusEventType">Program Status Event Type.</param>
        private void ControllerProgramEventHandler(eProgramStatusEventType programStatusEventType)
        {
            switch (programStatusEventType)
            {
                case eProgramStatusEventType.Paused:
                {
                    // The program has been paused.  Pause all user threads/timers as needed.
                    break;
                }

                case eProgramStatusEventType.Resumed:
                {
                    // The program has been resumed. Resume all the user threads/timers as needed.
                    break;
                }

                case eProgramStatusEventType.Stopping:
                {
                    SystemManager.Dispose();
                    break;
                }
            }
        }

        /// <summary>
        /// Event Handler for system events, Disk Inserted/Ejected, and Reboot
        /// Use this event to clean up when someone types in reboot, or when your SD /USB
        /// removable media is ejected / re-inserted.
        /// </summary>
        /// <param name="systemEventType">System Event TYpe.</param>
        private void ControllerSystemEventHandler(eSystemEventType systemEventType)
        {
            switch (systemEventType)
            {
                case eSystemEventType.DiskInserted:
                {
                    // Removable media was detected on the system
                    break;
                }

                case eSystemEventType.DiskRemoved:
                {
                    // Removable media was detached from the system
                    break;
                }

                case eSystemEventType.Rebooting:
                {
                    // The system is rebooting.
                    // Very limited time to perform clean up and save any settings to disk.
                    break;
                }
            }
        }
    }
}
