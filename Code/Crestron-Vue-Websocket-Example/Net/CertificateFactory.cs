// <copyright file="CertificateFactory.cs" company="jeffderek">
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
using System.Security.Cryptography.X509Certificates;
using Crestron.SimplSharp;
using Evands.Pellucid.Diagnostics;
using Org.BouncyCastle.Asn1.X509;

namespace Crestron_Vue_Websocket_Example.Net
{
    /// <summary>
    /// Factory class that reads certificates from the file system or creates Self Signed certificates if necessary.
    /// </summary>
    public class CertificateFactory
    {
        /// <summary>
        /// Locking object for reading the cert.
        /// </summary>
        private readonly object certLock = new object();

        /// <summary>
        /// Folder Path where certs are read from.
        /// </summary>
        private readonly string certFolderPath = "/user/cert/";

        /// <summary>
        /// The file name that will be used for a self signed cert if one is created.
        /// </summary>
        private readonly string selfSignedCertName = "selfSignedCrestron";

        /// <summary>
        /// The password that will be used for a self signed cert if one is created.
        /// </summary>
        private readonly string selfSignedCertPassword = "Crestr0nSelfS!gned";

        /// <summary>
        /// Initializes a new instance of the <see cref="CertificateFactory"/> class.
        /// </summary>
        public CertificateFactory()
        {
            if (!Directory.Exists(this.certFolderPath))
            {
                Directory.CreateDirectory(this.certFolderPath);
            }
        }

        /// <summary>
        /// Gets the existing self signed certificate or creates one if it does not exist.
        /// </summary>
        /// <returns>The self signed certificate.</returns>
        public X509Certificate2 GetOrCreateSelfSignedCertificate()
        {
            var certFilePath = this.GetCertificateFilePath(this.selfSignedCertName);
            if (string.IsNullOrWhiteSpace(certFilePath))
            {
                this.CreateSelfSignedCertificate();
            }

            var cert = this.ReadCertificate(this.selfSignedCertName, this.selfSignedCertPassword);

            if (cert.NotAfter.CompareTo(DateTime.UtcNow) < 0)
            {
                Logger.LogError(this, "Certificate is Expired, creating a new one.");
                this.CreateSelfSignedCertificate();
                cert = this.ReadCertificate(this.selfSignedCertName, this.selfSignedCertPassword);
            }

            return cert;
        }

        /// <summary>
        /// Get an <see cref="X509Certificate2"/> with the provided key.
        /// Returns the first certificate found in the /user/ folder on the processor.
        /// </summary>
        /// <param name="certFileName">The file name of the Certificate.</param>
        /// <param name="password">The Password for the Certificate.</param>
        /// <returns>The certificate.</returns>
        public X509Certificate2 ReadCertificate(string certFileName, string password)
        {
            var certFilePath = this.GetCertificateFilePath(certFileName);

            if (File.Exists(certFilePath))
            {
                lock (this.certLock)
                {
                    try
                    {
                        return new X509Certificate2(certFilePath, password);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(this, ex, $"Error Reading Certificate File");
                        return null;
                    }
                }
            }
            else
            {
                Logger.LogError(this, $"{certFilePath} Not Found");
            }

            return null;
        }

        /// <summary>
        /// Gets the File Path for the certificate with the provided file name.
        /// If no file name is provided, the first .pfx file in the /user/ directory will be returned.
        /// </summary>
        /// <param name="fileName">The file name for the Certificate.</param>
        /// <returns>The file path for the certificate.</returns>
        private string GetCertificateFilePath(string fileName = "*")
        {
            var certFileName = fileName + ".pfx";
            var certFileArray = Directory.GetFiles(this.certFolderPath, certFileName);
            string certFile;

            if (certFileArray.Length == 0)
            {
                Logger.LogError(this, $"Unable to load Certificate: No .pfx files in {this.certFolderPath}");
                return null;
            }
            else
            {
                certFile = certFileArray[0];
            }

            if (certFileArray.Length > 1)
            {
                Logger.LogWarning(this, $"Multiple .pfx Files found in {this.certFolderPath}, loading {certFile}");
            }
            else
            {
                Debug.WriteProgressLine(this, $"Reading pfx File from disk: {certFile}");
            }

            return certFile;
        }

        private void CreateSelfSignedCertificate()
        {
            try
            {
                Logger.LogNotice(this, "Creating self-signed certificate.  This could take a few minutes.");
                var certificateUtility = new BouncyCertificateUtility();

                var ipAddress = CrestronEthernetHelper.GetEthernetParameter(CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_CURRENT_IP_ADDRESS, 0);
                var hostName = CrestronEthernetHelper.GetEthernetParameter(CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_HOSTNAME, 0);
                var domainName = CrestronEthernetHelper.GetEthernetParameter(CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_DOMAIN_NAME, 0);

                Logger.LogNotice(this, $"DomainName: {domainName} | HostName: {hostName} | {hostName}.{domainName}@{ipAddress}");

                var certificate = certificateUtility.CreateSelfSignedCertificate($"CN={hostName}.{domainName}", new[] { $"{hostName}.{domainName}", ipAddress }, new[] { KeyPurposeID.id_kp_serverAuth, KeyPurposeID.id_kp_clientAuth });
                certificateUtility.CertificatePassword = this.selfSignedCertPassword;
                certificateUtility.WriteCertificate(certificate, this.certFolderPath, this.selfSignedCertName);
                Logger.LogNotice(this, "Self-signed Certificate Created");
            }
            catch (Exception ex)
            {
                Logger.LogException(this, ex, "Error creating self-signed certificate.");
            }
        }
    }
}
