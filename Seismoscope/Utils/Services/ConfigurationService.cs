using Seismoscope.Utils.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Seismoscope.Utils.Services
{
    public class ConfigurationService : IConfigurationService
    {
        
        public string GetDbPath()
        {
            return ConfigurationManager.AppSettings["DbPath"];
        }
        public string GetDefaultAdminUserName()
        {
            return ConfigurationManager.AppSettings["AdminUserName"];
        }
        public string GetDefaultAdminPassword()
        {
            return ConfigurationManager.AppSettings["AdminPassword"];
        }   
    }
}
