using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Opc.Ua;

namespace Jupiter
{
    public class ServerAndEndpointsPair
    {
        public string DiscoveryURL { get; }
        public string ApplicationName { get; }
        public EndpointDescriptionCollection Endpoints { get; set; }

        public ServerAndEndpointsPair(string discoveryURL, string applicationName)
        {
            DiscoveryURL = discoveryURL;
            ApplicationName = applicationName;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", ApplicationName, DiscoveryURL);
        }

        public static List<string> SecurityList(List<ServerAndEndpointsPair> servers, int selectedIndex)
        {
            var list = new List<string>();

            foreach (var endpoint in servers[selectedIndex].Endpoints)
            {
                list.Add(string.Format("{0} - {1}",
                    Opc.Ua.SecurityPolicies.GetDisplayName(endpoint.SecurityPolicyUri),
                    endpoint.SecurityMode));
            }

            return list;
        }
    }
}
