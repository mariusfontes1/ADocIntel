using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DocumentIntelligenceTest.Services
{
    public class CustomConfiguration : IConfiguration
    {
        internal readonly Dictionary<string, string> _config = new();
        private readonly string _configFilePath;

        public CustomConfiguration(string configFilePath = "appsettings.json")
        {
            _configFilePath = configFilePath;
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            try
            {
                if (File.Exists(_configFilePath))
                {
                    var json = File.ReadAllText(_configFilePath);
                    var configData = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                    if (configData != null)
                    {
                        FlattenDictionary(configData, "");
                    }
                }
                else
                {
                    Console.WriteLine($"⚠️  Arquivo de configuração não encontrado: {_configFilePath}");
                    Console.WriteLine("Criando arquivo de configuração padrão...");
                    CreateDefaultConfig();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao carregar configuração: {ex.Message}");
                CreateDefaultConfig();
            }
        }

        private void FlattenDictionary(Dictionary<string, object> dict, string prefix)
        {
            foreach (var kvp in dict)
            {
                var key = string.IsNullOrEmpty(prefix) ? kvp.Key : $"{prefix}:{kvp.Key}";

                if (kvp.Value is JsonElement element)
                {
                    if (element.ValueKind == JsonValueKind.Object)
                    {
                        var nestedDict = JsonSerializer.Deserialize<Dictionary<string, object>>(element.GetRawText());
                        if (nestedDict != null)
                        {
                            FlattenDictionary(nestedDict, key);
                        }
                    }
                    else
                    {
                        _config[key] = element.ToString();
                    }
                }
                else if (kvp.Value is Dictionary<string, object> nestedDict)
                {
                    FlattenDictionary(nestedDict, key);
                }
                else
                {
                    _config[key] = kvp.Value?.ToString() ?? "";
                }
            }
        }

        private void CreateDefaultConfig()
        {
            var defaultConfig = new
            {
                AzureDocumentIntelligence = new
                {
                    Endpoint = "https://documentIntelligenceSabemi.cognitiveservices.azure.com/",
                    ApiKey = "SUA_CHAVE_AQUI",
                    ModelId = "prebuilt-document"
                },
                Logging = new
                {
                    LogLevel = new
                    {
                        Default = "Information",
                        Microsoft = "Warning"
                    }
                }
            };

            var json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_configFilePath, json);

            Console.WriteLine($"✅ Arquivo de configuração criado: {_configFilePath}");
            Console.WriteLine("⚠️  Lembre-se de atualizar com suas credenciais do Azure!");
        }

        public string? this[string key]
        {
            get => _config.TryGetValue(key, out var value) ? value : null;
            set => _config[key] = value ?? string.Empty;
        }

        public string this[string key, string defaultValue]
        {
            get => _config.TryGetValue(key, out var value) ? value : defaultValue;
            set => _config[key] = value;
        }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            var sections = new List<IConfigurationSection>();
            var processedKeys = new HashSet<string>();

            foreach (var kvp in _config)
            {
                var keyParts = kvp.Key.Split(':');
                if (keyParts.Length > 1)
                {
                    var parentKey = keyParts[0];
                    if (!processedKeys.Contains(parentKey))
                    {
                        sections.Add(new CustomConfigurationSection(this, parentKey));
                        processedKeys.Add(parentKey);
                    }
                }
            }

            return sections;
        }

        public IConfigurationSection GetSection(string key)
        {
            return new CustomConfigurationSection(this, key);
        }

        public void Reload()
        {
            _config.Clear();
            LoadConfiguration();
        }

        public IChangeToken GetReloadToken()
        {
            return new ConfigurationReloadToken();
        }
    }

    public class ConfigurationReloadToken : IChangeToken
    {
        public bool HasChanged => false;
        public bool ActiveChangeCallbacks => false;

        public IDisposable RegisterChangeCallback(Action<object?> callback, object? state)
        {
            return new NoopDisposable();
        }
    }

    public class NoopDisposable : IDisposable
    {
        public void Dispose() { }
    }
}