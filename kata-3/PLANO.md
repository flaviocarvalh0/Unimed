# Análise de Engenharia e Plano de Ação - Sistema de Pedidos

**Contexto:** Sistema legado de e-commerce (5 anos), processando ~800 pedidos/dia.
**Objetivo:** Diagnosticar incidentes recentes, priorizar ações técnicas e definir estratégia de modernização arquitetural garantindo a continuidade do negócio.

---

## Seção 1 — Diagnóstico

Abaixo, a análise de causa raiz, risco e priorização (Matriz de Eisenhower) para os 5 incidentes relatados:

### 1. Endpoint de consulta de pedidos demora 8–12 segundos
* **Causa Raiz Provável:** Ausência de paginação adequada, falta de índices no banco de dados (gerando *Full Table Scan*), problema de consultas N+1 no ORM ou acoplamento com APIs externas síncronas.
* **Risco Técnico:** Esgotamento de *pool* de conexões do banco e *timeout* na aplicação, podendo gerar indisponibilidade em cascata.
* **Risco de Negócio:** Abandono de carrinho/aplicativo, frustração do cliente e sobrecarga no time de atendimento (SAC).
* **Prioridade:** **Urgente e Importante** (Foco imediato).

### 2. Pedidos criados em duplicidade
* **Causa Raiz Provável:** Ausência de chaves de idempotência na API, falta de *constraints* de unicidade no banco de dados e falta de bloqueio de duplo clique no *frontend*.
* **Risco Técnico:** Inconsistência na base de dados e concorrência transacional falha.
* **Risco de Negócio:** Prejuízo financeiro direto (envio de mercadoria duplicada), problemas com estornos de cartão de crédito e perda de confiança do cliente.
* **Prioridade:** **Urgente e Importante** (Foco imediato - afeta a receita).

### 3. Bug de frete corrigido direto em produção (sem PR/Testes)
* **Causa Raiz Provável:** Ausência de pipeline de CI/CD, permissões excessivas para desenvolvedores em ambiente produtivo e cultura de "apagar incêndios" sem processos estruturados.
* **Risco Técnico:** Derrubar o sistema inteiro por um erro de digitação e impossibilidade de *rollback* rápido.
* **Risco de Negócio:** Impacto na estabilidade geral das vendas.
* **Prioridade:** **Importante, mas Não Urgente** (Problema de processo/cultura a ser resolvido a curto/médio prazo).

### 4. Arquivo da camada de negócio com +4.000 linhas
* **Causa Raiz Provável:** Acúmulo crônico de débito técnico, quebra do princípio de Responsabilidade Única (SRP) e padrão *God Object* (Classe Deus).
* **Risco Técnico:** Altíssimo acoplamento. Qualquer nova *feature* tem grande probabilidade de quebrar uma funcionalidade existente (*side-effects*).
* **Risco de Negócio:** Aumento drástico no *Lead Time* (tempo para entregar novas funcionalidades ao mercado).
* **Prioridade:** **Importante, mas Não Urgente** (Projeto de longo prazo).

### 5. Zero testes automatizados
* **Causa Raiz Provável:** Priorização de velocidade de entrega em detrimento da qualidade e falta de cultura de engenharia de software (TDD/Testes).
* **Risco Técnico:** Impossibilidade de refatorar código com segurança. Regressões constantes.
* **Risco de Negócio:** Sistema instável, gerando custos altos de manutenção corretiva em vez de evolução do produto.
* **Prioridade:** **Importante, mas Não Urgente** (Trabalho contínuo e progressivo).

---

## Seção 2 — Plano de Ação

Dado o cenário crítico, a prioridade máxima é **"estancar o sangramento"** financeiro e de experiência do usuário, além de proteger o ambiente produtivo. As três ações prioritárias são:

### Ação 1: Resolução da Duplicidade de Pedidos (Integridade Financeira)
* **O que será feito:** 1. Implementar o padrão de **Idempotência** no endpoint de criação de pedidos (ex: envio de um `Idempotency-Key` no *header* gerado pelo *frontend*).
  2. Adicionar uma *Unique Constraint* no banco de dados (ex: `UsuarioId` + `HashDoCarrinho`).
* **Esforço Estimado:** 2 a 3 dias úteis.
* **Critério de Sucesso:** Zero registros de pedidos duplicados no monitoramento/log por 15 dias consecutivos.

### Ação 2: Otimização do Endpoint de Consulta (UX e Estabilidade)
* **O que será feito:** 1. Análise do plano de execução da query (*Query Execution Plan*) para identificar gargalos.
  2. Criação de índices no banco de dados para os filtros mais comuns (Data, Status, Cliente).
  3. Implementação de paginação estrita e/ou uso de Cache (ex: Redis) para dados de leitura pesada e pouca mutação.
* **Esforço Estimado:** 3 a 5 dias úteis.
* **Critério de Sucesso:** Redução do *Response Time* no percentil 95 (P95) de 12 segundos para menos de 800 milissegundos nos horários de pico.

### Ação 3: Trava de Segurança em Produção e Pipeline Básico (Processo)
* **O que será feito:** 1. Bloquear *commits* diretos na *branch* `main/master` (proteção de branch). 
  2. Exigir *Pull Requests* com pelo menos 1 aprovação para *merge*.
  3. Criar uma esteira de *Continuous Integration* (CI) básica que execute testes de compilação (*build*) antes de qualquer *deploy*.
* **Esforço Estimado:** 1 a 2 dias úteis.
* **Critério de Sucesso:** 100% das novas alterações passam pelo fluxo de PR e CI, bloqueando edições "ninja" direto no servidor de produção.

---

## Seção 3 — Decisão de Arquitetura

Para lidar com o arquivo de +4.000 linhas da camada de negócios, minha escolha técnica é a **Opção A — Refatoração incremental**.

**Justificativa Técnica e de Negócios:**
A Opção B (Reescrita do zero) é extremamente tentadora para engenheiros, mas em um sistema de 5 anos **sem nenhum teste automatizado**, reescrever do zero é o maior risco que a empresa poderia assumir. O arquivo de 4.000 linhas possui dezenas de regras de negócio obscuras, *edge cases* e comportamentos não documentados que garantem o faturamento diário (800 pedidos/dia). Tentar adivinhar essas regras em um sistema novo fatalmente geraria regressões graves.

A **Opção A** permite aplicar o padrão **Strangler Fig (Figueira Estranguladora)**. A estratégia será:
1. Escrever "Testes de Caracterização" (*Golden Master Pattern*) para garantir o comportamento atual daquele trecho específico do arquivo legado.
2. Extrair o domínio fatiado gradualmente (ex: primeiro extrair apenas o cálculo de frete para um novo módulo).
3. Testar unitariamente esse novo módulo.
4. Direcionar a chamada do código legado para o novo módulo.
5. Repetir o processo.

Isso entrega valor continuamente, mantém a empresa faturando sem paralisações e reduz o risco ao mínimo, mitigando o fato de termos um time já ocupado com a operação diária.

---

## Seção 4 — Requisitos Não Funcionais (RNFs) Ignorados

Abaixo os 3 principais atributos de qualidade do sistema que estão claramente comprometidos, com base nos incidentes relatados:

### 1. Desempenho (Performance)
* **Por que está comprometido:** O tempo de resposta de 8–12 segundos do endpoint de consulta de pedidos no horário de pico demonstra completa incapacidade da arquitetura atual de processar carga em tempo hábil.
* **Métrica de Monitoramento:** Monitorar via APM (ex: New Relic, Datadog) garantindo que a latência no **Percentil 95 (P95) seja <= 500ms** e o Apdex *score* fique acima de 0.90.

### 2. Integridade e Confiabilidade de Dados (Reliability / Data Integrity)
* **Por que está comprometido:** A duplicação de pedidos evidencia falhas no tratamento de transações, falta de resiliência contra falhas de rede (tentativas de envio duplo pelo cliente) e ausência de travas estruturais no banco.
* **Métrica de Monitoramento:** Estabelecer um indicador de anomalias (Alertas no Grafana) apontando para **0 incidentes de concorrência ou duplicação por mês**.

### 3. Manutenibilidade (Maintainability)
* **Por que está comprometido:** Um arquivo com 4.000 linhas de lógica de negócios aliado à inexistência de testes automatizados torna o código ilegível, testável apenas manualmente e perigoso de evoluir.
* **Métrica de Monitoramento:** Analisador estático de código (ex: SonarQube) com metas incrementais de diminuição de complexidade ciclomática e acompanhamento do **aumento da cobertura de testes (*Test Coverage*)**, visando atingir no mínimo 70% nas lógicas de *core business* novas e refatoradas.

