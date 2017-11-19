using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENode.Sample.Common
{
    public class ConfigSettings
    {
        public static string ConnectionString { get; set; }
        public static string NoteTable { get; set; }

        public const int NameServerPort = 10091;

        public const int BrokerProducerPort = 10092;
        public const int BrokerConsumerPort = 10093;
        public const int BrokerAdminPort = 10094;
        public const int BrokerCommandPort = 10095;

        public static void Initialize()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
            NoteTable = "Note";
        }
    }
}
