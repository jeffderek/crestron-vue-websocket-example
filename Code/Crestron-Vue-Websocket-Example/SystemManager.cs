// <copyright file="SystemManager.cs" company="jeffderek">
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crestron_Vue_Websocket_Example.Devices;
using Crestron_Vue_Websocket_Example.Net;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Crestron_Vue_Websocket_Example
{
    /// <summary>
    /// Primary class for the entire system.
    /// </summary>
    public class SystemManager : IDisposable
    {
        /// <summary>
        /// IDisposable Support to detect redundant calls.
        /// </summary>
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemManager"/> class.
        /// </summary>
        /// <param name="controlSystem">The ControlSystem to be managed.</param>
        public SystemManager(ControlSystem controlSystem)
        {
            this.ControlSystem = controlSystem;
            this.Mock = new Mock();
            this.Touchpanel = new Touchpanel(controlSystem);

            // In an actual system, you would probably read these values in from a config file.
            // The WebUi hosts BOTH the HTTP project and the Websocket.  The WebUi class needs to know
            // where on the processor the compiled HTTP project exists at.
            // The WebSocketPath and port can be pretty much whatever you want.  The Path is used to
            // differentiate between different types of WebSocketBehaviors for the same server, but
            // this design uses the same WebSocketBehavior (WebUiClient) for all connections.
            this.WebUi = new WebUi(this, "/user/html", "app", 5620, true);
        }

        /// <summary>
        /// Gets the Touchpanel that handles IP Table communication.
        /// </summary>
        public Touchpanel Touchpanel { get; private set; }

        /// <summary>
        /// Gets a simple object that contains our fake devices that get reported to the touchpanel.
        /// </summary>
        public Mock Mock { get; private set; }

        /// <summary>
        /// Gets the WebUi that all socket communication goes over.
        /// </summary>
        public WebUi WebUi { get; private set; }

        /// <summary>
        /// Gets the Control System.
        /// </summary>
        public ControlSystem ControlSystem { get; private set; }

        /// <inheritdoc/>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// IDisposable Support
        /// Dispose of all elements in the room.
        /// </summary>
        /// <param name="disposing">Disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.WebUi.Dispose();
                    this.Touchpanel.Dispose();
                }

                this.disposedValue = true;
            }
        }
    }
}
