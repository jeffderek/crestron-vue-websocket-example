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

        public class Echo : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {
                Console.WriteLine($"Rx: {e.Data}");
                Send(e.Data);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemManager"/> class.
        /// </summary>
        /// <param name="controlSystem">The ControlSystem to be managed.</param>
        public SystemManager(ControlSystem controlSystem)
        {
            ControlSystem = controlSystem;

            var server = new WebSocketServer("ws://0.0.0.0:5000");
            server.AddWebSocketService<Echo>("/echo");
            server.Start();
            Console.WriteLine("WS Server Started");


        }

        /// <summary>
        /// Gets the Control System.
        /// </summary>
        public ControlSystem ControlSystem { get; private set; }

        private WebSocketServer server;

        /// <inheritdoc/>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// IDisposable Support
        /// Dispose of all elements in the room.
        /// </summary>
        /// <param name="disposing">Disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    server.Stop();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }
    }
}
