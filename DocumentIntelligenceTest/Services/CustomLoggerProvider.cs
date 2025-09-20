using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace DocumentIntelligenceTest.Services
{
    public class CustomLoggerProvider : ILoggerProvider
    {
        private readonly Dictionary<string, ILogger> _loggers = new();
        private readonly string _logFilePath;

        public CustomLoggerProvider(string logFilePath = "logs.txt")
        {
            _logFilePath = logFilePath;
        }

        public ILogger CreateLogger(string categoryName)
        {
            if (!_loggers.ContainsKey(categoryName))
            {
                _loggers[categoryName] = new CustomLogger(categoryName, _logFilePath);
            }
            return _loggers[categoryName];
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}