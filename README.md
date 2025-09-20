# ADocIntel
Solução que consome documentos e extrai informações relevantes usando o Azure Document Intelligence

## 📁 Estrutura do Projeto

- **DocumentIntelligenceTest/**: Aplicação principal de análise de documentos
  - Veja [DocumentIntelligenceTest-README.md](DocumentIntelligenceTest-README.md) para instruções detalhadas
  - Interface de console para análise de PDFs e imagens
  - Extração de campos específicos e exportação para JSON

## 🚀 Como Começar

1. **Clone o repositório**
   ```bash
   git clone https://github.com/mariusfontes1/ADocIntel.git
   cd ADocIntel
   ```

2. **Configure as credenciais do Azure**
   - Edite `DocumentIntelligenceTest/appsettings.json`
   - Adicione sua API key do Azure Document Intelligence

3. **Execute o projeto**
   ```bash
   cd DocumentIntelligenceTest
   dotnet run
   ```

## 📚 Documentação

- [DocumentIntelligenceTest-README.md](DocumentIntelligenceTest-README.md) - Instruções completas do projeto principal
- [SampleDocuments/README.md](DocumentIntelligenceTest/SampleDocuments/README.md) - Documentos de exemplo

## 🛠️ Tecnologias

- .NET 9.0
- Azure Document Intelligence
- C# com Dependency Injection
- Sistema de logging personalizado

## 📄 Licença

Este projeto está sob a licença MIT.