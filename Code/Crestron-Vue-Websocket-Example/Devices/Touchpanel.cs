// <copyright file="Touchpanel.cs" company="jeffderek">
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
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.UI;
using Crestron_Vue_Websocket_Example.Net;
using Evands.Pellucid.Diagnostics;

namespace Crestron_Vue_Websocket_Example.Devices
{
    /// <summary>
    /// The touchpanel class for handling the Standard IP Table communication with the panel.
    /// This class will handle the panels online/offline status and sending the processor's IP
    /// address to the panel, but none of the regular communication,  All of the websocket
    /// communication gets handled in the <see cref="WebUi"/> class.
    /// </summary>
    public class Touchpanel : IDisposable
    {
        /// <summary>
        /// IDisposable Support to detect redundant calls.
        /// </summary>
        private bool disposedValue = false;

        /// <summary>
        /// The actual touchpanel device.
        /// </summary>
        private Tsw1070 device;

        /// <summary>
        /// Initializes a new instance of the <see cref="Touchpanel"/> class.
        /// </summary>
        /// <param name="controlSystem">ControlSystem.</param>
        public Touchpanel(ControlSystem controlSystem)
        {
            this.device = new Tsw1070('\x03', controlSystem);

            if (this.device.Register() != Crestron.SimplSharpPro.eDeviceRegistrationUnRegistrationResponse.Success)
            {
                Logger.LogError(this, $"Error Registering the Touchpanel on IpId 0x{this.device.ID:X2}: {this.device.RegistrationFailureReason}");
            }
            else
            {
                this.device.IpInformationChange += (sender, args) =>
                {
                    var panelIp = args.DeviceIpAddress;
                    string processorIp = string.Empty;

                    // Check for a control subnet
                    if (InitialParametersClass.NumberOfExternalEthernetInterfaces > 0)
                    {
                        var csIp = CrestronEthernetHelper.GetEthernetParameter(CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_CURRENT_IP_ADDRESS, CrestronEthernetHelper.GetAdapterdIdForSpecifiedAdapterType(EthernetAdapterType.EthernetCSAdapter));
                        if (csIp.Substring(0, csIp.IndexOf('.')) == panelIp.Substring(0, panelIp.IndexOf('.')))
                        {
                            processorIp = csIp;
                        }
                    }

                    var lanIp = CrestronEthernetHelper.GetEthernetParameter(CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_CURRENT_IP_ADDRESS, CrestronEthernetHelper.GetAdapterdIdForSpecifiedAdapterType(EthernetAdapterType.EthernetLANAdapter));
                    if (lanIp.Substring(0, lanIp.IndexOf('.')) == panelIp.Substring(0, panelIp.IndexOf('.')))
                    {
                        processorIp = lanIp;
                    }

                    if (!string.IsNullOrWhiteSpace(processorIp))
                    {
                        this.device.StringInput[1].StringValue = processorIp;
                    }
                    else
                    {
                        Logger.LogError(this, $"Unable to determine the Processor IP for {this.device.ID} at {panelIp} to connect to");
                    }
                };
            }
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
                    this.device?.UnRegister();
                    this.device?.Dispose();
                }

                this.disposedValue = true;
            }
        }
    }
}
