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
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Crestron_Vue_Websocket_Example
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
        private WebSocketServer server;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebUi"/> class.
        /// </summary>
        public WebUi()
        {
            // Create a new websocket server on port 5000
            server = new WebSocketServer(5000);

            // Add a WebSocketBehavior, detailing what to do with the incoming traffic.
            server.AddWebSocketService<ParseIncoming>("/app");

            // Start the server
            server.Start();
        }

        /// <summary>
        /// This code added to correctly implement the disposable pattern.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
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

                disposedValue = true;
            }
        }

        /// <summary>
        /// WebSocketBehaviors set up what to do with the incoming traffic
        /// For this demo all traffic connects to the same websocket, so it all
        /// goes through the same behavior.
        /// -
        /// Importantly, this is not a good example of how to actually write an api
        /// or parse the traffic going to and from your websocket.  It is highly
        /// recommended that you implement a custom json schema that meets
        /// the needs of your specific project.
        /// </summary>
        public class ParseIncoming : WebSocketBehavior
        {
            /// <summary>
            /// A simple object that contains our system "status" that gets reported to the touchpanel.
            /// </summary>
            private Mock mock = new Mock();

            /// <summary>
            /// When the socket is opened, send the current system status to the panel.
            /// </summary>
            protected override void OnOpen()
            {
                base.OnOpen();
                Send($"name|Crestron Vue Websocket Example");
                Send($"displays|display_1|power_{(mock.Display1Power ? "on" : "off")}");
                Send($"displays|display_2|power_{(mock.Display2Power ? "on" : "off")}");
                Send($"counter|{mock.Counter}");
            }

            /// <summary>
            /// Parse the messages from the panel.
            /// </summary>
            /// <param name="e">MessageEventArgs.</param>
            protected override void OnMessage(MessageEventArgs e)
            {
                switch (e.Data)
                {
                    case "displays|display_1|power_on":
                    {
                        mock.Display1Power = true;
                        Send(e.Data);
                        break;
                    }

                    case "displays|display_1|power_off":
                    {
                        mock.Display1Power = false;
                        Send(e.Data);
                        break;
                    }

                    case "displays|display_2|power_on":
                    {
                        mock.Display2Power = true;
                        Send(e.Data);
                        break;
                    }

                    case "displays|display_2|power_off":
                    {
                        mock.Display2Power = false;
                        Send(e.Data);
                        break;
                    }

                    case "counter|increment":
                    {
                        mock.Counter++;
                        Send($"counter|{mock.Counter}");
                        break;
                    }

                    case "counter|decrement":
                    {
                        mock.Counter--;
                        Send($"counter|{mock.Counter}");
                        break;
                    }
                }
            }
        }
    }
}
