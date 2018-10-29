using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Ua;

namespace Jupiter
{
    public class ApplicationConfiguration
    {
        public static Opc.Ua.ApplicationConfiguration Load(string filepath)
        {
            var fi = new System.IO.FileInfo(filepath);
            if(!fi.Exists)
            {
                var config = DefaultConfiguration();
                config.Validate(ApplicationType.Client).Wait();
                config.SaveToFile(filepath);
                return config;
            }

            var application = new Opc.Ua.Configuration.ApplicationInstance();
            application.ApplicationType = ApplicationType.Client;
            application.LoadApplicationConfiguration(filepath, false).Wait();
            application.CheckApplicationInstanceCertificate(false, 0).Wait();
            return application.ApplicationConfiguration;
        }

        private static Opc.Ua.ApplicationConfiguration DefaultConfiguration()
        {
            var commonApplicationData = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            return new Opc.Ua.ApplicationConfiguration()
            {
                ApplicationName = "Jupiter",
                ApplicationType = ApplicationType.Client,
                ApplicationUri = "urn:" + Utils.GetHostName() + ":bamchoh:Jupiter",
                SecurityConfiguration = new SecurityConfiguration
                {
                    ApplicationCertificate = new CertificateIdentifier
                    {
                        StoreType = "Directory",
                        StorePath = commonApplicationData + @"\Jupiter\pki\own",
                        SubjectName = "CN=Jupiter, C=JP, S=Osaka, O=Jupiter, DC=localhost"
                    },
                    TrustedPeerCertificates = new CertificateTrustList
                    {
                        StoreType = "Directory",
                        StorePath = commonApplicationData + @"\Jupiter\pki\trusted"
                    },
                    TrustedIssuerCertificates = new CertificateTrustList
                    {
                        StoreType = "Directory",
                        StorePath = commonApplicationData + @"\Jupiter\pki\issuer"
                    },
                    RejectedCertificateStore = new CertificateStoreIdentifier
                    {
                        StoreType = "Directory",
                        StorePath = commonApplicationData + @"\Jupiter\pki\rejected"
                    },
                    NonceLength = 32,
                    AutoAcceptUntrustedCertificates = true
                },
                TransportConfigurations = new TransportConfigurationCollection(),
                TransportQuotas = new TransportQuotas { OperationTimeout = 120000 },
                ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 600000 },
            };
        }
    }
}
