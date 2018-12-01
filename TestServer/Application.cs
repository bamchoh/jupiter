using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Configuration;
using System.IO;

namespace TestServer
{
    public static class Application
    {
        static string GenerateConfigurationFile()
        {
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            var strm = asm.GetManifestResourceStream("TestServer.Configuration.xml");
            var name = Path.GetTempFileName();

            using (var writer = new FileStream(name, FileMode.Create))
            {
                strm.CopyTo(writer);
            }

            strm.Close();

            return name;
        }

        public static ApplicationInstance GetApplicationInstance()
        {
            var conffile = GenerateConfigurationFile();
            var application = new ApplicationInstance();
            application.ApplicationType = ApplicationType.Server;
            application.ConfigSectionName = "TestServer";
            application.LoadApplicationConfiguration(conffile, false).Wait();
            application.CheckApplicationInstanceCertificate(false, 0).Wait();
            return application;
        }
    }
}
