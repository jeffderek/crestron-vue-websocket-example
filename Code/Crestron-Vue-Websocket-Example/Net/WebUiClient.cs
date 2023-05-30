// <copyright file="WebUiClient.cs" company="jeffderek">
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

using Crestron.SimplSharp;
using Evands.Pellucid;
using Evands.Pellucid.Diagnostics;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Crestron_Vue_Websocket_Example.Net
{
    /// <summary>
    /// WebSocketBehaviors set up what to do with the incoming traffic
    /// For this demo all traffic connects to the same websocket, so it all
    /// goes through the same behavior.
    /// -
    /// Importantly, this is not a good example of how to actually write an api
    /// or parse the traffic going to and from your websocket.  It is highly
    /// recommended that you implement a custom json schema that meets
    /// the needs of your specific project.  This example only intends to show
    /// creating the actual connection between the UI and the processor.
    /// </summary>
    public class WebUiClient : WebSocketBehavior
    {
        /// <summary>
        /// Gets or sets the System Manager that owns the devices in the system.
        /// You could tie pretty much anything here instead of the System Manager.
        /// If you use a pub/sub system you could tie the Message Publisher here.
        /// </summary>
        public SystemManager SystemManager { get; set; }

        /// <summary>
        /// When the socket is opened, send the current system status to the panel.
        /// </summary>
        protected override void OnOpen()
        {
            base.OnOpen();
            Debug.WriteSuccessLine(this, "Socket Connected");

            // This sends the current status of all of the devices to the panel.  In a production system, you would want
            // to create a way to subscribe to these states so that you get updates.  In this example, if you open two different browsers
            // and make changes to one, you won't see the values update on the other.
            this.SendToClient($"name|Crestron Vue Websocket Example");
            this.SendToClient($"displays|display_1|power_{(this.SystemManager.Mock.Display1Power ? "on" : "off")}");
            this.SendToClient($"displays|display_2|power_{(this.SystemManager.Mock.Display2Power ? "on" : "off")}");
            this.SendToClient($"counter|{this.SystemManager.Mock.Counter}");
        }

        /// <inheritdoc/>
        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            Debug.WriteErrorLine(this, "Socket Disconnected");
        }

        /// <summary>
        /// Parse the messages from the panel.
        /// </summary>
        /// <param name="e">MessageEventArgs.</param>
        protected override void OnMessage(MessageEventArgs e)
        {
            Debug.WriteDebugLine(this, $"[Rx]{e.Data}");
            CrestronConsole.PrintLine($"[Rx]{e.Data}");

            switch (e.Data)
            {
                case "displays|display_1|power_on":
                {
                    this.SystemManager.Mock.Display1Power = true;
                    this.SendToClient(e.Data);
                    break;
                }

                case "displays|display_1|power_off":
                {
                    this.SystemManager.Mock.Display1Power = false;
                    this.SendToClient(e.Data);
                    break;
                }

                case "displays|display_2|power_on":
                {
                    this.SystemManager.Mock.Display2Power = true;
                    this.SendToClient(e.Data);
                    break;
                }

                case "displays|display_2|power_off":
                {
                    this.SystemManager.Mock.Display2Power = false;
                    this.SendToClient(e.Data);
                    break;
                }

                case "counter|increment":
                {
                    this.SystemManager.Mock.Counter++;
                    this.SendToClient($"counter|{this.SystemManager.Mock.Counter}");
                    break;
                }

                case "counter|decrement":
                {
                    this.SystemManager.Mock.Counter--;
                    this.SendToClient($"counter|{this.SystemManager.Mock.Counter}");
                    break;
                }
            }
        }

        /// <summary>
        /// This function really only exists so I can print all messages to Debug.  You could just
        /// use this.Send everywhere if you didn't want to be able to debug message.
        /// </summary>
        /// <param name="message">Message to send.</param>
        private void SendToClient(string message)
        {
            CrestronConsole.PrintLine($"[Tx]{message}");
            Debug.WriteDebugLine(this, ProConsole.Colors.Yellow.FormatText($"[Tx]{message}"));
            this.Send(message);
        }
    }
}
