using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentIntelligenceTest.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;

namespace DocumentIntelligenceTest.Services
{
    public class DocumentAnalyzerService
    {
        private readonly DocumentAnalysisClient _client;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DocumentAnalyzerService> _logger;

        public DocumentAnalyzerService(IConfiguration configuration, ILogger<DocumentAnalyzerService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            var endpoint = _configuration["AzureDocumentIntelligence:Endpoint"];
            var apiKey = _configuration["AzureDocumentIntelligence:ApiKey"];

            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("Azure Document Intelligence configuration is missing");
                throw new InvalidOperationException("Azure Document Intelligence configuration is missing. Please check appsettings.json");
            }

            _logger.LogInformation("Inicializando cliente Azure Document Intelligence");
            _client = new DocumentAnalysisClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
        }

        public async Task<DocumentResult> AnalyzeDocumentAsync(string filePath)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.LogInformation($"Starting analysis of document: {filePath}");

                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Document not found: {filePath}");
                }

                using var stream = File.OpenRead(filePath);
                var modelId = _configuration["AzureDocumentIntelligence:ModelId"] ?? "prebuilt-document";

                var operation = await _client.AnalyzeDocumentAsync(WaitUntil.Completed, modelId, stream);
                var result = operation.Value;

                stopwatch.Stop();

                var documentResult = ProcessAnalysisResult(result, stopwatch.Elapsed);

                _logger.LogInformation($"Document analysis completed in {stopwatch.Elapsed.TotalSeconds:F2} seconds");

                return documentResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error analyzing document: {filePath}");
                throw;
            }
        }

        private DocumentResult ProcessAnalysisResult(AnalyzeResult result, TimeSpan processingTime)
        {
            // Calcular confiança baseada nos campos extraídos
            double calculatedConfidence = CalculateConfidence(result);

            var documentResult = new DocumentResult
            {
                ProcessingTime = processingTime,
                Confidence = calculatedConfidence
            };

            // Extrair texto bruto
            if (result.Pages?.Count > 0)
            {
                documentResult.RawText = string.Join("\n", result.Pages.Select(p =>
                    string.Join("\n", p.Lines?.Select(l => l.Content) ?? new List<string>())));
            }

            // Processar campos específicos
            ProcessKeyValuePairs(result, documentResult);
            ProcessTables(result, documentResult);
            ProcessEntities(result, documentResult);

            return documentResult;
        }

        private void ProcessKeyValuePairs(AnalyzeResult result, DocumentResult documentResult)
        {
            if (result.KeyValuePairs == null) return;

            foreach (var kvp in result.KeyValuePairs)
            {
                var key = kvp.Key?.Content?.ToLower() ?? "";
                var value = kvp.Value?.Content ?? "";

                // Mapear campos específicos
                switch (key)
                {
                    case var k when k.Contains("cliente") || k.Contains("nome"):
                        documentResult.ClientName = value;
                        break;
                    case var k when k.Contains("documento") || k.Contains("numero") || k.Contains("cpf") || k.Contains("cnpj"):
                        documentResult.DocumentNumber = value;
                        break;
                    case var k when k.Contains("data") || k.Contains("date"):
                        if (DateTime.TryParse(value, out var date))
                            documentResult.Date = date;
                        break;
                    case var k when k.Contains("total") || k.Contains("valor") || k.Contains("amount"):
                        if (decimal.TryParse(value.Replace("R$", "").Replace(",", ".").Trim(), out var amount))
                            documentResult.TotalValue = amount;
                        break;
                    default:
                        documentResult.AdditionalFields[key] = value;
                        break;
                }
            }
        }

        private void ProcessTables(AnalyzeResult result, DocumentResult documentResult)
        {
            if (result.Tables == null) return;

            foreach (var table in result.Tables)
            {
                var tableData = new List<Dictionary<string, string>>();

                for (int row = 0; row < table.RowCount; row++)
                {
                    var rowData = new Dictionary<string, string>();
                    for (int col = 0; col < table.ColumnCount; col++)
                    {
                        var cell = table.Cells.FirstOrDefault(c => c.RowIndex == row && c.ColumnIndex == col);
                        if (cell != null)
                        {
                            rowData[$"col_{col}"] = cell.Content ?? "";
                        }
                    }
                    tableData.Add(rowData);
                }

                documentResult.AdditionalFields[$"table_{table.Cells.Count}"] = tableData;
            }
        }

        private void ProcessEntities(AnalyzeResult result, DocumentResult documentResult)
        {
            // Entities não está disponível na versão atual da API
            // Esta funcionalidade foi removida temporariamente
            // TODO: Implementar quando a API suportar entidades
        }

        private double CalculateConfidence(AnalyzeResult result)
        {
            double totalConfidence = 0.0;
            int confidenceCount = 0;

            // Calcular confiança baseada nos KeyValuePairs
            if (result.KeyValuePairs != null)
            {
                foreach (var kvp in result.KeyValuePairs)
                {
                    totalConfidence += kvp.Confidence;
                    confidenceCount++;
                }
            }

            // Calcular confiança baseada nas tabelas (células não têm confiança individual)
            if (result.Tables != null)
            {
                foreach (var table in result.Tables)
                {
                    // Usar confiança da tabela se disponível
                    if (table.Cells != null && table.Cells.Count > 0)
                    {
                        // Assumir confiança média baseada no número de células
                        totalConfidence += 0.8; // Confiança estimada para tabelas
                        confidenceCount++;
                    }
                }
            }

            // Calcular confiança baseada nas páginas
            if (result.Pages != null)
            {
                foreach (var page in result.Pages)
                {
                    if (page.Lines != null)
                    {
                        // Assumir confiança média baseada no número de linhas
                        totalConfidence += 0.9; // Confiança estimada para linhas
                        confidenceCount++;
                    }
                }
            }

            _logger.LogInformation($"Confidence calculated: {totalConfidence / confidenceCount:P1} (from {confidenceCount} elements)");

            return confidenceCount > 0 ? totalConfidence / confidenceCount : 0.0;
        }

        public async Task<string> AnalyzeAndSaveAsJsonAsync(string filePath, string? outputPath = null)
        {
            var result = await AnalyzeDocumentAsync(filePath);

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(result, jsonOptions);

            if (string.IsNullOrEmpty(outputPath))
            {
                outputPath = Path.ChangeExtension(filePath, ".json") ?? filePath + ".json";
            }

            await File.WriteAllTextAsync(outputPath, json);

            _logger.LogInformation($"Results saved to: {outputPath}");

            return json;
        }
    }
}
