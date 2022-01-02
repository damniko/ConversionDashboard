using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace DesktopUI
{
    public class Config : IConfiguration
    {
        private Dictionary<string, string> _settings = new();

        public string this[string key]
        {
            get
            {
                if(_settings.ContainsKey(key) is false)
                {
                    throw new ArgumentException($"Setting with key {key} was not found");
                }
                return _settings[key];
            }
            set
            {
                if(_settings.ContainsKey(key) is false)
                {
                    _settings.Add(key, value);
                }
                else
                {
                    _settings[key] = value;
                }
            }
        }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            throw new NotImplementedException();
        }

        public IChangeToken GetReloadToken()
        {
            throw new NotImplementedException();
        }

        public IConfigurationSection GetSection(string key)
        {
            throw new NotImplementedException();
        }
    }
}
