# Análise de Engenharia de Dados - Kata 4

## 1. Quais foram as principais decisões de tratamento que você tomou?
* **Registros Órfãos (Entregas sem Pedidos):** Optei por utilizar um *Left Join* partindo da tabela de Pedidos. Isso significa que a entrega `1003` (atrelada ao pedido fantasma `999`) foi descartada. A justificativa de negócio é que uma entrega sem um pedido validado não possui cliente, valor financeiro ou status de venda atrelado, caracterizando uma anomalia de sistema que deve ser ignorada no relatório gerencial principal e enviada para uma fila de auditoria (Dead Letter).
* **Normalização de Cidades:** Para unificar "São Paulo", "sao paulo" e "SAO PAULO", apliquei uma técnica de remoção de diacríticos (separando os acentos das letras usando `NormalizationForm.FormD`), removi caracteres especiais via Regex e forcei a conversão para maiúsculo (`ToUpper`).
* **Tratamento de Datas:** Criei um *parser* com *fallback*. O sistema tenta ler no padrão brasileiro (DD/MM/AAAA); se falhar, tenta o formato universal (ISO); e, se falhar novamente, interpreta como Unix Timestamp (como no pedido 103).
* **Campos Monetários:** Padronizei o separador decimal substituindo vírgulas por pontos antes do *parse*, garantindo que valores como "1500,50" fossem somados corretamente usando `CultureInfo.InvariantCulture`.

## 2. Seu pipeline é idempotente? Justifique.
**Sim, o pipeline é estritamente idempotente.** A idempotência garante que executar o processo uma vez ou mil vezes sobre o mesmo conjunto de dados de origem produzirá exatamente o mesmo estado final. Nosso pipeline lê os arquivos CSV em modo de apenas leitura (*read-only*), aplica funções puras de transformação em memória e **sobrescreve** o arquivo de saída `consolidado.csv`. Como não há acúmulo de estado (ex: não fazemos *append* cego no final do arquivo), o resultado final será sempre idêntico, independentemente de quantas vezes o `Program.cs` for executado.

## 3. Se esse pipeline fosse executado diariamente com arquivos de 10 milhões de linhas, o que você mudaria na abordagem?
A arquitetura atual causaria um colapso de memória (*Out of Memory Exception*). O uso de `.ToList()` carrega todos os dados na memória RAM simultaneamente. Para escalar para 10 milhões de linhas:
1. **Streaming em C#:** Mudaria a leitura para fluxos contínuos utilizando `IAsyncEnumerable` e `StreamReader`, processando e gravando os dados em lotes (*chunks* ou *batches*) sem segurar tudo na RAM.
2. **Mudança de Paradigma (Ferramentas de Big Data):** Em um cenário real corporativo, o C# puro não é a melhor ferramenta para ETL massivo. Eu faria o *ingest* desses arquivos brutos em um Data Warehouse (como Google BigQuery ou Snowflake) e usaria ferramentas como **dbt (Data Build Tool)** ou **Apache Spark / Databricks** para realizar essas transformações de forma distribuída (em cluster).

## 4. Que testes você escreveria para garantir a qualidade das transformações?
Para um pipeline "à prova de balas", eu implementaria testes em três camadas:
* **Testes de Unidade (Unitários):** Validar as funções isoladas de limpeza (`ParseData`, `NormalizarCidade`). Exemplo: garantir que passar "32/13/2026" retorne nulo e não quebre o sistema.
* **Testes de Integração:** Criar pequenos arquivos CSV "mockados" (com 3 linhas cada, contendo cenários de erro conhecidos) e garantir que a saída final do `.csv` consolidado bata com um resultado esperado pré-calculado.
* **Data Quality Checks (Contratos de Dados):** Testes executados *em tempo de execução* antes de iniciar o cruzamento. Regras como: "A coluna ID_Pedido não pode conter nulos" ou "A Data_Realizada não pode ser menor que a Data_Pedido". Se essas regras falharem, o pipeline aborta (padrão *Circuit Breaker*) e emite um alerta, evitando que dados corrompidos poluam o relatório final.