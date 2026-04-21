# Análise de Requisitos - Kata 2 (Painel de Tarefas)

## 1. Ambiguidades Identificadas e Decisões

A partir do relato informal do cliente, identifiquei as seguintes ambiguidades e premissas faltantes antes de iniciar a implementação:

| Ambiguidade / Informação Faltante | Pergunta que eu faria ao cliente | Decisão Técnica Tomada (MVP) |
| :--- | :--- | :--- |
| **Escopo de Usuário ("minhas tarefas")** | O sistema será acessado por múltiplos usuários com necessidade de login, ou será um painel de uso individual/compartilhado localmente? | **Sem autenticação no MVP.** Assumirei que é uma ferramenta de uso local ou para uma equipe enxuta que compartilha a mesma visão. O banco terá IDs únicos preparados para futura segregação por usuário (Tenant). |
| **Definição da "Situação"** | Os status das tarefas são binários (Pendente e Concluída) ou existem estados intermediários (ex: Em Andamento, Bloqueada, Cancelada)? | **Status Binário.** Implementarei um controle de status simples (Pendente/Concluída) para focar na entrega rápida da funcionalidade principal solicitada. |
| **Comportamento da Exclusão** | Ao deletar uma tarefa, ela deve ser removida definitivamente do banco de dados (Hard Delete) ou apenas inativada para histórico (Soft Delete)? | **Hard Delete.** Como o cliente disse "deletar as que não preciso mais", a exclusão será definitiva para economizar armazenamento e simplificar a API neste primeiro momento. |

---

## 2. Requisitos Funcionais (RF)

Com base no relato refinado, o sistema deve atender às seguintes funcionalidades:

* **RF01:** O sistema deve permitir a criação de uma nova tarefa, exigindo apenas o Título. A tarefa deve nascer com o status inicial de "Pendente".
* **RF02:** O sistema deve listar as tarefas cadastradas, exibindo o Título e a Situação atual.
* **RF03:** O sistema deve permitir a filtragem da listagem de tarefas com base em três visões: Todas, Somente Pendentes e Somente Concluídas.
* **RF04:** O sistema deve permitir a alteração do status de uma tarefa específica (marcar como concluída ou reabrir como pendente).
* **RF05:** O sistema deve permitir a exclusão definitiva de uma tarefa específica.

---

## 3. Requisitos Não Funcionais (RNF)

* **RNF01 (Backend):** A API deve ser desenvolvida em C#/.NET, seguindo os princípios de uma arquitetura RESTful.
* **RNF02 (Persistência):** O armazenamento de dados utilizará o **SQLite**.
  * *Justificativa:* A escolha do SQLite foi estratégica para o escopo desta avaliação. Ele garante a persistência real dos dados sem exigir que o avaliador instale, configure ou levante containers de bancos de dados pesados (como PostgreSQL ou SQL Server) para testar a aplicação localmente.
* **RNF03 (Frontend):** A interface deve ser uma aplicação web responsiva (SPA), consumindo os endpoints da API de forma assíncrona.
* **RNF04 (Confiabilidade):** A API deve implementar tratamento global de erros e retornar os códigos de status HTTP apropriados (ex: `200 OK`, `201 Created`, `204 No Content`, `404 Not Found`).

---

## 4. Gestão do Backlog: O Requisito de "Prioridade"

O cliente mencionou que a inclusão de uma prioridade nas tarefas "pode ficar pra depois". 

**Como eu trataria isso no backlog do produto:**
1. **Classificação (Produto):** Utilizando a matriz MoSCoW, este item seria classificado como um *Could-Have* (Desejável) e não entraria na primeira entrega (MVP). Ele seria documentado como uma User Story para a próxima Sprint.
2. **Prevenção Arquitetural (Engenharia):** Embora a interface web e a API inicial não exponham a funcionalidade de Prioridade, eu modelaria a tabela do banco de dados já contendo uma coluna `Priority` (com um valor padrão, ex: `Normal`). Isso evita a necessidade de criar migrações complexas de banco de dados (*breaking changes*) no momento em que o cliente solicitar a ativação da funcionalidade no frontend.