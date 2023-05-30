// <copyright file="WebUi.cs" company="jeffderek">
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
using System.IO;
using Crestron_Vue_Websocket_Example.Devices;
using Evands.Pellucid.Diagnostics;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Crestron_Vue_Websocket_Example.Net
{
    /// <summary>
    /// The primary class for handling communication with the websocket.
    /// </summary>
    public class WebUi : IDisposable
    {
        /// <summary>
        /// IDisposable Support to detect redundant calls.
        /// </summary>
        private bool disposedValue = false;

        /// <summary>
        /// The actual websocket server.
        /// </summary>
        private HttpServer server;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebUi"/> class.
        /// </summary>
        /// <param name="systemManager">The System Manager that contains the devices the WebUiClient will talk to.</param>
        /// <param name="httpPath">Path on the processor for where the http project is located.</param>
        /// <param name="webSocketPath">URI Path for the websocket service.</param>
        /// <param name="port">Port to host the server on.</param>
        /// <param name="secure">Whether to host a secure (https/wss) or unsecure (http/ws) server.</param>
        public WebUi(SystemManager systemManager, string httpPath, string webSocketPath, int port, bool secure)
        {
            // Create a new server
            this.server = new HttpServer(port, secure);

            // Add a WebSocketBehavior, detailing what to do with the incoming traffic.
            // Also pass the SystemManager to the client so that the client can talk to the actual devices in the system.
            // There are MANY ways to handle the client talking to devices in the system, passing a Manager in is just one of them.
            this.server.AddWebSocketService<WebUiClient>($"/{webSocketPath}", (client) =>
            {
                Debug.WriteDebugLine(this, "Creating New Client");
                client.SystemManager = systemManager;
            });

            this.server.WebSocketServices.KeepClean = false;
            this.server.DocumentRootPath = httpPath;
            if (!Directory.Exists(this.server.DocumentRootPath))
            {
                Directory.CreateDirectory(this.server.DocumentRootPath);
            }

            var httpGetHandler = new HttpGetHandler();
            this.server.OnGet += httpGetHandler.Http_OnGet;

            if (secure)
            {
                this.server.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                var certificateFactory = new CertificateFactory();
                this.server.SslConfiguration.ServerCertificate = certificateFactory.GetOrCreateSelfSignedCertificate();
            }

            // Start the server
            this.server.Start();
        }

        /// <summary>
        /// This code added to correctly implement the disposable pattern.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(disposing: true);
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
                    this.server.Stop();
                }

                this.disposedValue = true;
            }
        }
    }
}
