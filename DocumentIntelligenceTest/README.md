# Document Intelligence Test

Solução que consome documentos e extrai informações relevantes usando o Azure Document Intelligence.

## 📋 Funcionalidades

- **Análise de Documentos**: Processa PDFs e imagens usando Azure Document Intelligence
- **Extração de Dados**: Extrai campos específicos como:
  - Nome do cliente
  - Número do documento (CPF/CNPJ)
  - Data
  - Valor total
  - Tipo de documento
- **Processamento de Tabelas**: Extrai dados de tabelas estruturadas
- **Reconhecimento de Entidades**: Identifica pessoas, datas, valores monetários
- **Exportação JSON**: Salva resultados em formato JSON
- **Logging Personalizado**: Sistema de logs com arquivo e console
- **Interface Amigável**: Console com emojis e formatação clara

## 🚀 Como Usar

### Pré-requisitos

- .NET 9.0 SDK
- Conta do Azure com Document Intelligence habilitado
- Credenciais do Azure (Endpoint e API Key)

### Configuração

1. **Clone o repositório**
   ```bash
   git clone <url-do-repositorio>
   cd DocumentIntelligenceTest
   ```

2. **Configure as credenciais do Azure**
   
   Edite o arquivo `appsettings.json`:
   ```json
   {
     "AzureDocumentIntelligence": {
       "Endpoint": "https://seu-endpoint.cognitiveservices.azure.com/",
       "ApiKey": "sua-api-key-aqui",
       "ModelId": "prebuilt-document"
     }
   }
   ```

   **⚠️ IMPORTANTE**: Nunca commite credenciais reais no repositório. Use variáveis de ambiente ou Azure Key Vault em produção.

3. **Compile o projeto**
   ```bash
   dotnet build
   ```

4. **Execute o programa**
   ```bash
   # Com arquivo específico
   dotnet run -- "caminho/para/documento.pdf"
   
   # Modo interativo
   dotnet run
   ```

### Exemplos de Uso

```bash
# Analisar um PDF
dotnet run -- "documentos/nota-fiscal.pdf"

# Analisar uma imagem
dotnet run -- "documentos/recibo.jpg"

# Modo interativo (o programa pedirá o caminho)
dotnet run
```

## 📁 Estrutura do Projeto

```
DocumentIntelligenceTest/
├── Models/
│   ├── AzureConfig.cs          # Configuração do Azure
│   └── DocumentResult.cs       # Modelo de resultado
├── Services/
│   ├── CustomConfiguration.cs  # Sistema de configuração personalizado
│   ├── CustomLogger.cs         # Logger personalizado
│   ├── CustomLoggerProvider.cs # Provider do logger
│   ├── CustomConfigurationSection.cs # Seção de configuração
│   └── DocumentAnalyzerService.cs # Serviço principal de análise
├── SampleDocuments/            # Documentos de exemplo (vazio)
├── Program.cs                  # Ponto de entrada
├── appsettings.json           # Configurações
└── README.md                  # Este arquivo
```

## 🔧 Configuração Avançada

### Modelos Personalizados

Para usar um modelo personalizado do Azure Document Intelligence:

1. Treine seu modelo no Azure
2. Atualize o `ModelId` no `appsettings.json`:
   ```json
   {
     "AzureDocumentIntelligence": {
       "ModelId": "seu-modelo-personalizado"
     }
   }
   ```

### Logging

O sistema de logging está configurado para:
- **Console**: Mostra logs em tempo real
- **Arquivo**: Salva logs em `logs.txt`

Para alterar o nível de log, edite `appsettings.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  }
}
```

## 📊 Formato de Saída

O programa gera um arquivo JSON com os resultados:

```json
{
  "clientName": "João Silva",
  "documentNumber": "123.456.789-00",
  "date": "2024-01-15T00:00:00",
  "totalValue": 150.75,
  "documentType": "Nota Fiscal",
  "confidence": 0.95,
  "additionalFields": {
    "table_0": [...],
    "endereco": "Rua das Flores, 123"
  },
  "rawText": "Texto extraído do documento...",
  "processingTime": "00:00:02.5000000"
}
```

## 🛠️ Desenvolvimento

### Dependências

- `Azure.AI.FormRecognizer` (4.1.0)
- `Microsoft.Extensions.Configuration` (8.0.0)
- `Microsoft.Extensions.DependencyInjection` (8.0.0)
- `Microsoft.Extensions.Logging` (8.0.0)
- `System.Text.Json` (8.0.0)

### Compilação

```bash
# Debug
dotnet build --configuration Debug

# Release
dotnet build --configuration Release

# Executar
dotnet run --configuration Release
```

## 🔒 Segurança

- **Nunca commite credenciais** no repositório
- Use variáveis de ambiente em produção
- Considere usar Azure Key Vault para credenciais sensíveis
- Mantenha as dependências atualizadas

## 🐛 Solução de Problemas

### Erro de Configuração
```
Azure Document Intelligence configuration is missing
```
**Solução**: Verifique se o `appsettings.json` está configurado corretamente.

### Erro de Arquivo
```
Document not found: caminho/arquivo.pdf
```
**Solução**: Verifique se o caminho do arquivo está correto e se o arquivo existe.

### Erro de Conexão
```
Error analyzing document: [erro de rede]
```
**Solução**: Verifique sua conexão com a internet e as credenciais do Azure.

## 📝 Logs

Os logs são salvos em `logs.txt` com o formato:
```
[2024-01-15 10:30:45] [INFORMATION] [DocumentAnalyzerService] Starting analysis of document: documento.pdf
```

## 🤝 Contribuição

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/nova-funcionalidade`)
3. Commit suas mudanças (`git commit -m 'Adiciona nova funcionalidade'`)
4. Push para a branch (`git push origin feature/nova-funcionalidade`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

## 📞 Suporte

Para suporte, abra uma issue no repositório ou entre em contato com a equipe de desenvolvimento.
