using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedPoints.Core
{
    class NodeConfiguration
    {
        public string MongoHost { get; set; }
        public int MongoPort { get; set; }
        public int MinBlockSize { get; set; }

        public static NodeConfiguration Load()
        {
            var config = new NodeConfiguration
            {
                MongoHost = ConfigurationManager.AppSettings["MongoHost"],
                MongoPort = Convert.ToInt32(ConfigurationManager.AppSettings["MongoPort"]),
                MinBlockSize = Convert.ToInt32(ConfigurationManager.AppSettings["MinBlockSize"])
            };

            return config;
        }
    }
}