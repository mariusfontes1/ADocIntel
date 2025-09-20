using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace DocumentIntelligenceTest.Services
{
    public class CustomLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly string _logFilePath;

        public CustomLogger(string categoryName, string logFilePath = "logs.txt")
        {
            _categoryName = categoryName;
            _logFilePath = logFilePath;

            // Criar diretório de logs se não existir
            var logDir = Path.GetDirectoryName(_logFilePath);
            if (!string.IsNullOrEmpty(logDir) && !Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var message = formatter(state, exception);
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var logLevelString = logLevel.ToString().ToUpper();

            var logMessage = $"[{timestamp}] [{logLevelString}] [{_categoryName}] {message}";

            if (exception != null)
            {
                logMessage += $"\nException: {exception}";
            }

            // Escrever no console
            Console.WriteLine(logMessage);

            // Escrever no arquivo
            try
            {
                File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
            }
            catch
            {
                // Se não conseguir escrever no arquivo, continua sem erro
            }
        }
    }
}