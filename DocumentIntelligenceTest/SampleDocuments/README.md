# Documentos de Exemplo

Esta pasta contém documentos de exemplo para testar o Document Intelligence Test.

## 📄 Tipos de Documentos Suportados

- **PDF**: Documentos em formato PDF
- **Imagens**: JPG, PNG, BMP, TIFF
- **Documentos Escaneados**: Imagens de documentos digitalizados

## 🧪 Como Testar

1. **Adicione documentos de exemplo** nesta pasta
2. **Execute o programa** com o caminho do documento:
   ```bash
   dotnet run -- "SampleDocuments/meu-documento.pdf"
   ```

## 📋 Exemplos de Documentos

Para testar diferentes funcionalidades, adicione documentos como:

- **Notas Fiscais**: Para testar extração de valores e datas
- **Contratos**: Para testar extração de nomes e datas
- **Faturas**: Para testar extração de tabelas e valores
- **Recibos**: Para testar extração de informações básicas
- **Documentos com Tabelas**: Para testar processamento de tabelas

## ⚠️ Nota de Segurança

- **Nunca adicione documentos com dados sensíveis** nesta pasta
- **Use apenas documentos de exemplo** ou dados fictícios
- **Remova documentos reais** antes de fazer commit

## 🔍 Dicas para Testes

1. **Teste com diferentes qualidades** de imagem
2. **Teste com documentos em português** e inglês
3. **Teste com diferentes formatos** (PDF, JPG, PNG)
4. **Teste com documentos escaneados** e digitais
5. **Teste com documentos que contenham tabelas**

## 📊 Resultados Esperados

O programa deve extrair:
- Nome do cliente
- Número do documento
- Data
- Valor total
- Texto bruto
- Campos adicionais
- Dados de tabelas
