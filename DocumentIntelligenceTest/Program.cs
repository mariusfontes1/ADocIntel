using DocumentIntelligenceTest.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DocumentIntelligenceTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Configurar DI com implementações personalizadas
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            var analyzer = serviceProvider.GetRequiredService<DocumentAnalyzerService>();

            try
            {
                Console.WriteLine("=== Document Intelligence Test ===");
                Console.WriteLine("Análise de documentos com Azure Document Intelligence");
                Console.WriteLine();

                // Verificar se arquivo foi fornecido como argumento
                string filePath;
                if (args.Length > 0)
                {
                    filePath = args[0];
                }
                else
                {
                    Console.Write("Digite o caminho do documento (PDF ou imagem): ");
                    filePath = Console.ReadLine() ?? "";
                }

                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    Console.WriteLine("❌ Arquivo não encontrado ou caminho inválido.");
                    Console.WriteLine("Uso: DocumentIntelligenceTest.exe <caminho_do_arquivo>");
                    return;
                }

                logger.LogInformation($"Iniciando análise do documento: {Path.GetFileName(filePath)}");
                Console.WriteLine($"📄 Analisando documento: {Path.GetFileName(filePath)}");
                Console.WriteLine("⏳ Processando...");

                // Analisar documento
                var result = await analyzer.AnalyzeDocumentAsync(filePath);

                // Exibir resultados
                DisplayResults(result);

                // Salvar JSON
                var jsonPath = Path.ChangeExtension(filePath, ".json");
                var json = await analyzer.AnalyzeAndSaveAsJsonAsync(filePath, jsonPath);

                Console.WriteLine($"\n💾 Resultado salvo em: {jsonPath}");
                Console.WriteLine("\n✅ Análise concluída com sucesso!");

                // Perguntar se quer analisar outro documento
                Console.Write("\nDeseja analisar outro documento? (s/n): ");
                var response = Console.ReadLine()?.ToLower();
                if (response == "s" || response == "sim")
                {
                    Console.Clear();
                    await Main(new string[0]); // Recursão para nova análise
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro durante a execução");
                Console.WriteLine($"❌ Erro: {ex.Message}");
                Console.WriteLine("Verifique se:");
                Console.WriteLine("1. O arquivo existe e é válido");
                Console.WriteLine("2. As configurações do Azure estão corretas em appsettings.json");
                Console.WriteLine("3. Você tem conexão com a internet");
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Configuração personalizada
            var configuration = new CustomConfiguration();
            services.AddSingleton<IConfiguration>(configuration);

            // Logging personalizado
            services.AddLogging(builder =>
            {
                builder.AddProvider(new CustomLoggerProvider());
                builder.SetMinimumLevel(LogLevel.Information);
            });

            // Serviços
            services.AddScoped<DocumentAnalyzerService>();
        }

        private static void DisplayResults(Models.DocumentResult result)
        {
            Console.WriteLine("\n📊 RESULTADOS DA ANÁLISE:");
            Console.WriteLine("=" + new string('=', 50));

            Console.WriteLine($"⏱️  Tempo de processamento: {result.ProcessingTime.TotalSeconds:F2}s");
            Console.WriteLine($"🎯 Confiança: {result.Confidence:P1}");

            Console.WriteLine("\n�� CAMPOS EXTRAÍDOS:");
            Console.WriteLine("-" + new string('-', 30));

            if (!string.IsNullOrEmpty(result.ClientName))
                Console.WriteLine($"👤 Nome do Cliente: {result.ClientName}");

            if (!string.IsNullOrEmpty(result.DocumentNumber))
                Console.WriteLine($"�� Número do Documento: {result.DocumentNumber}");

            if (result.Date.HasValue)
                Console.WriteLine($"📅 Data: {result.Date.Value:dd/MM/yyyy}");

            if (result.TotalValue.HasValue)
                Console.WriteLine($"�� Valor Total: R$ {result.TotalValue.Value:F2}");

            if (!string.IsNullOrEmpty(result.DocumentType))
                Console.WriteLine($"📑 Tipo de Documento: {result.DocumentType}");

            if (result.AdditionalFields.Any())
            {
                Console.WriteLine("\n🔍 CAMPOS ADICIONAIS:");
                foreach (var field in result.AdditionalFields)
                {
                    Console.WriteLine($"   • {field.Key}: {field.Value}");
                }
            }

            if (!string.IsNullOrEmpty(result.RawText))
            {
                Console.WriteLine($"\n📝 TEXTO EXTRAÍDO (primeiros 200 caracteres):");
                Console.WriteLine($"   {result.RawText.Substring(0, Math.Min(200, result.RawText.Length))}...");
            }
        }
    }
}