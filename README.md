# Unimed Caruaru | Teste de Seleção - Desenvolvimento

Este repositório contém a resolução dos Katas propostos para a avaliação técnica de desenvolvimento de software da Unimed Caruaru. O projeto foi estruturado com foco em clareza, manutenibilidade e aplicação de princípios de engenharia de software (DDD, SOLID, Clean Code).

## 👤 Candidato
* **Nome completo:** Flávio Alves Teixeira de Carvalho
* **Telefone de contato:** (81) 9 2141-0615
* **E-mail:** flavioalvespw@outlook.com

---

## 🛠️ Stack(s) Utilizada(s) e Justificativa da Escolha

Optei por utilizar o ecossistema **.NET** como fundação do projeto, espelhando a stack principal utilizada no dia a dia da equipe de TI da Unimed Caruaru. 

* **C# / .NET 9:** Escolhido para a resolução dos algoritmos e construção da API e Pipelines. O C# oferece tipagem forte, altíssima performance e recursos modernos (como LINQ e Records) que tornam a implementação de regras de negócio limpa e declarativa.
* **xUnit:** Framework padrão de mercado no ecossistema .NET para a garantia de qualidade através de testes unitários automatizados.
* **SQLite (Kata 2):** Escolhido como banco de dados relacional para o Painel de Tarefas por reduzir a fricção na avaliação. Permite que o avaliador execute o projeto localmente sem a necessidade de configurar instâncias pesadas (como SQL Server/PostgreSQL) ou dependências de containers Docker.
* **Frontend (Kata 2):** React com TypeScript e Vite. Escolhido para a construção de uma interface moderna, tipada, componentizada e de altíssima performance no build, integrando de forma fluida com a API REST.

---

## 🚀 Instruções para Executar Cada Kata Localmente

**Pré-requisitos Globais:**
* [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) instalado.
* [Node.js](https://nodejs.org/) (versão 18+ recomendada) para o Frontend.

### Kata 1 - Fila de Triagem (Lógica e Algoritmos)
O Kata 1 foi desenvolvido como uma *Class Library* focada no Domínio (Regras de Negócio) com cobertura de testes unitários, isolado de frameworks de infraestrutura.

1. Navegue até a pasta da Solution:
   ```bash
   cd "kata-1/src/Fila de Triagem"
   ```
2. Execute os testes automatizados para validar as regras:
   ```bash
   dotnet test
   ```

### Kata 2 - Painel de Tarefas (API REST + Frontend)
Este Kata é fullstack. É necessário rodar o Backend e o Frontend em terminais separados.

**Rodando o Backend (API C#):**
1. Abra um terminal e navegue até a pasta da API:
   ```bash
   cd kata-2/api
   ```
2. Execute o projeto (o banco SQLite será criado e populado automaticamente):
   ```bash
   dotnet run
   ```
   *A API estará disponível (geralmente em `http://localhost:5000` ou `https://localhost:5001`).*

**Rodando o Frontend (React):**
1. Abra um **novo** terminal e navegue até a pasta web:
   ```bash
   cd kata-2/web
   ```
2. Instale as dependências e inicie o servidor de desenvolvimento:
   ```bash
   npm install
   npm run dev
   ```
   *Acesse o link gerado no terminal (geralmente `http://localhost:5173`) para interagir com o painel.*

### Kata 3 - Sistema Legado em Colapso (Arquitetura)
Este Kata é focado em análise, diagnóstico e plano de ação técnico (não exige execução de código).
* O documento de entrega encontra-se na pasta raiz do Kata: `kata-3/PLANO.md`.

### Kata 4 - Pipeline de Relatório (Engenharia de Dados)
Desenvolvido como um Console Application focado em higienização de CSVs, cruzamento de dados via LINQ e idempotência.

1. Navegue até a pasta do projeto:
   ```bash
   cd kata-4/PipelineDados
   ```
2. Execute o pipeline:
   ```bash
   dotnet run
   ```
*O relatório consolidado de indicadores será impresso no terminal, e o arquivo final higienizado será gerado em `dados/consolidado.csv`. A análise e as decisões de tratamento tomadas estão documentadas no arquivo `kata-4/ANALISE.md`.*

## 💡 Comentários Livres: O que eu faria diferente com mais tempo?

Durante o desenvolvimento, priorizei a entrega de valor, a legibilidade do código e o alinhamento com o escopo solicitado (evitando *over-engineering*). Contudo, em um cenário de projeto a longo prazo ou com mais tempo disponível, eu aplicaria as seguintes melhorias:

1. **Kata 1 (Escalabilidade Extrema com Filas de Prioridade Múltiplas):** Para o escopo atual de uma clínica, a ordenação da lista utilizando o motor interno do LINQ `O(N log N)` atende perfeitamente e mantém o código expressivo. No entanto, se o sistema precisasse escalar para redesenhar uma fila de triagem estadual com milhões de eventos simultâneos, eu substituiria a lista unificada por uma estrutura de *Buckets* (Filas de Prioridade Múltiplas). Com um número finito de urgências (4 níveis), eu instanciaria uma `Queue<Paciente>` para cada nível, reduzindo a complexidade de inserção e leitura para `O(1)`.

2. **Kata 2 (Maturidade da API e Segurança):**
   No desenvolvimento da API REST para o painel de tarefas, o tempo ditou uma abordagem mais direta. Com um prazo maior, eu implementaria:
   * **Autenticação e Autorização:** Uso de JWT (JSON Web Tokens) e separação de dados por *Tenant* ou Usuário.
   * **Paginação e CQRS:** Para garantir a performance da listagem (GET) mesmo quando a base de tarefas crescesse consideravelmente.
   * **Observabilidade Robusta:** Integração com Serilog e OpenTelemetry para rastreabilidade de requisições em ambiente produtivo.

 