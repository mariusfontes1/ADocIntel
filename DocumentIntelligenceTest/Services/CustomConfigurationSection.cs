using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentIntelligenceTest.Services
{
    public class CustomConfigurationSection : IConfigurationSection
    {
        private readonly CustomConfiguration _parent;
        private readonly string _key;

        public CustomConfigurationSection(CustomConfiguration parent, string key)
        {
            _parent = parent;
            _key = key;
        }

        public string Key => _key;

        public string Path => _key;

        public string? Value
        {
            get => _parent[_key];
            set => _parent[_key] = value ?? string.Empty;
        }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            var children = new List<IConfigurationSection>();
            var prefix = _key + ":";
            var processedKeys = new HashSet<string>();

            foreach (var kvp in _parent._config)
            {
                if (kvp.Key.StartsWith(prefix))
                {
                    var remainingKey = kvp.Key.Substring(prefix.Length);
                    var keyParts = remainingKey.Split(':');
                    var childKey = keyParts[0];

                    if (!processedKeys.Contains(childKey))
                    {
                        children.Add(new CustomConfigurationSection(_parent, prefix + childKey));
                        processedKeys.Add(childKey);
                    }
                }
            }

            return children;
        }

        public IConfigurationSection GetSection(string key)
        {
            var fullKey = string.IsNullOrEmpty(_key) ? key : $"{_key}:{key}";
            return new CustomConfigurationSection(_parent, fullKey);
        }

        public IChangeToken GetReloadToken()
        {
            return _parent.GetReloadToken();
        }

        public string? this[string key]
        {
            get
            {
                var fullKey = string.IsNullOrEmpty(_key) ? key : $"{_key}:{key}";
                return _parent[fullKey];
            }
            set
            {
                var fullKey = string.IsNullOrEmpty(_key) ? key : $"{_key}:{key}";
                _parent[fullKey] = value ?? string.Empty;
            }
        }
    }
}