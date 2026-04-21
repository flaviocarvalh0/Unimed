# Unimed Caruaru | Teste de Seleção - Desenvolvimento

Este repositório contém a resolução dos Katas propostos para a avaliação técnica de desenvolvimento de software da Unimed Caruaru. O projeto foi estruturado com foco em clareza, manutenibilidade e aplicação de princípios de engenharia de software (DDD, SOLID, Clean Code).

## 👤 Candidato
* **Nome completo:** Flávio Alves Teixeira de Carvalho
* **Telefone de contato:** (81) 9 2141-0615
* **E-mail:** flavioalvespw@outlook.com

---

## 🛠️ Stack(s) Utilizada(s) e Justificativa da Escolha

Optei por utilizar o ecossistema **.NET** como fundação do projeto, espelhando a stack principal utilizada no dia a dia da equipe de TI da Unimed Caruaru. 

* **C# / .NET 9:** Escolhido para a resolução dos algoritmos e construção da API. O C# oferece tipagem forte, altíssima performance e recursos modernos (como LINQ e Records) que tornam a implementação de regras de negócio limpa e declarativa.
* **xUnit:** Framework padrão de mercado no ecossistema .NET para a garantia de qualidade através de testes unitários automatizados.
* **SQLite (Kata 2):** Escolhido como banco de dados relacional para o Painel de Tarefas por reduzir a fricção na avaliação. Permite que o avaliador execute o projeto localmente sem a necessidade de configurar instâncias pesadas (como SQL Server/PostgreSQL) ou dependências de containers Docker.
* **Frontend (Kata 2):** [React + TypeScript ou Angular - *Ajustar conforme o que formos usar*] para construção de uma interface componentizada e tipada, integrando de forma fluida com a API REST.

---

## 🚀 Instruções para Executar Cada Kata Localmente

**Pré-requisitos:**
* [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) instalado.
* Terminal de sua preferência (PowerShell, Bash, etc.).

### Kata 1 - Fila de Triagem (Lógica e Algoritmos)
O Kata 1 foi desenvolvido como uma *Class Library* focada no Domínio (Regras de Negócio) com cobertura de testes unitários, isolado de frameworks de infraestrutura.

1. Navegue até a pasta da Solution do Kata 1:
   ```bash
   cd "kata-1/src/Fila de Triagem"

## 💡 Comentários Livres: O que eu faria diferente com mais tempo?

Durante o desenvolvimento, priorizei a entrega de valor, a legibilidade do código e o alinhamento com o escopo solicitado (evitando *over-engineering*). Contudo, em um cenário de projeto a longo prazo ou com mais tempo disponível, eu aplicaria as seguintes melhorias:

1. **Kata 1 (Escalabilidade Extrema com Filas de Prioridade Múltiplas):** Para o escopo atual de uma clínica, a ordenação da lista utilizando o motor interno do LINQ `O(N log N)` atende perfeitamente e mantém o código expressivo. No entanto, se o sistema precisasse escalar para redesenhar uma fila de triagem estadual com milhões de eventos simultâneos, eu substituiria a lista unificada por uma estrutura de *Buckets* (Filas de Prioridade Múltiplas). Com um número finito de urgências (4 níveis), eu instanciaria uma `Queue<Paciente>` para cada nível, reduzindo a complexidade de inserção e leitura para `O(1)`.

2. **Kata 2 (Maturidade da API e Segurança):**
   No desenvolvimento da API REST para o painel de tarefas, o tempo ditou uma abordagem mais direta. Com um prazo maior, eu implementaria:
   * **Autenticação e Autorização:** Uso de JWT (JSON Web Tokens) e separação de dados por *Tenant* ou Usuário.
   * **Paginação e CQRS:** Para garantir a performance da listagem (GET) mesmo quando a base de tarefas crescesse consideravelmente.
   * **Observabilidade Robusta:** Integração com Serilog e OpenTelemetry para rastreabilidade de requisições em ambiente produtivo.

3. **Qualidade Contínua:**
   Expandiria a pirâmide de testes, adicionando **Testes de Integração** (validando as rotas da API com um banco de dados em memória utilizando o `WebApplicationFactory`) e **Testes End-to-End (E2E)** no frontend (com Cypress ou Playwright).