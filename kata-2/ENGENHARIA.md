# Engenharia e Arquitetura - Kata 2

Neste documento, detalho as decisões técnicas e arquiteturais tomadas durante o desenvolvimento da API REST (Painel de Tarefas), bem como as visões de escalabilidade e manutenibilidade do projeto.

## 1. Decisões de Arquitetura no Backend

Para o backend, optei por evoluir de uma Minimal API simples para uma **Arquitetura em Camadas (N-Tier)** inspirada nos princípios do **Domain-Driven Design (DDD)** e **Clean Architecture**. A principal motivação foi a **Separação de Responsabilidades (Separation of Concerns)**:

* **Camada de Domínio (Domain):** O coração da aplicação. A entidade `Tarefa` protege seu estado através de encapsulamento (propriedades `private set` e métodos específicos para alteração, como `MudarStatus`). 
* **Camada de Infraestrutura (Infrastructure):** Isola a tecnologia de acesso a dados utilizando o **Repository Pattern**. O resto do sistema desconhece o SQLite, confiando apenas no contrato (`ITarefaRepositorio`).
* **Camada de Aplicação/API (Controllers e DTOs):** Responsável por orquestrar as requisições HTTP. A adoção de **DTOs** impede a exposição direta das entidades e evita *Over-Posting*.

## 2. Visão de Futuro e Escalabilidade do Banco de Dados

Durante a análise dos requisitos informais, o cliente mencionou a inclusão de "Prioridade" nas tarefas. No MVP atual, mapeei isso de forma simplificada, mas em um cenário de evolução do produto, minha primeira decisão arquitetural seria a **Normalização dessa Entidade**:

* **Transformação de Prioridade em Entidade:** Em vez de receber valores dinâmicos soltos (strings), a `Prioridade` seria uma tabela de domínio própria (ex: `Id`, `Descricao`, `Nivel`, `CorVisual`).
* **Vantagens:** Isso daria aos administradores o poder de gerenciar os tipos de prioridade do sistema. Além disso, garantiria a integridade referencial através de *Foreign Keys* (`PrioridadeId` na Tarefa), acabando com inconsistências de digitação e facilitando drasticamente a extração de relatórios analíticos (ex: "Quantas tarefas críticas estão pendentes?").

## 3. Confiabilidade e Múltiplos Usuários em Produção

Para garantir que a API seja resiliente em um ambiente produtivo da Unimed, eu planejaria a implementação das seguintes práticas:

1. **Autenticação e Multi-Tenant:**
    Pensando em um cenário real onde o sistema precisaria suportar múltiplos usuários, nossa arquitetura atual já deu o primeiro passo, mas precisaria das seguintes evoluções:

    **O que já antecipamos (Implementado):**
    * **Controle de Concorrência Otimista:** Adicionamos o campo `DataAlteracao` como um *Concurrency Token*. Se dois usuários abrirem a mesma tarefa e tentarem editá-la ao mesmo tempo, a API rejeitará a segunda requisição, evitando que dados sejam sobrescritos silenciosamente (problema do "último a salvar vence").

   * Implementação de provedor de identidade via tokens **JWT**.
   * Adição do `UsuarioId` nas tarefas para isolamento de dados por *Tenant*. 

2. **Qualidade e Observabilidade:**
   * **Health Checks:** Implementar endpoints `/health` para garantir que a infraestrutura de nuvem saiba se a API e o banco de dados estão saudáveis, reiniciando o serviço automaticamente em caso de falhas críticas.
   * **Rastreabilidade (Tracing):** Integrar ferramentas como Serilog e OpenTelemetry para mapear o tempo gasto em cada camada da requisição, facilitando a identificação de gargalos de performance.
   * **Esteira CI/CD:** Adicionar Testes de Integração que validem as rotas do Controller com um banco em memória, rodando automaticamente no GitHub Actions antes de qualquer *deploy*.

---

### 💡 Débito Técnico (Melhoria Futura)
Devido ao tempo delimitado para a entrega do MVP, a API atualmente utiliza os retornos nativos do ASP.NET (`Ok()`, `BadRequest()`). Em uma iteração futura, eu implementaria um **Filtro de Exceção Global (Global Exception Handler)** para padronizar 100% das respostas (*Result Pattern*), facilitando o consumo pelo Frontend. 

*Exemplo da padronização que seria aplicada:*
```json
{
  "sucesso": false,
  "dados": null,
  "erros": [
    {
      "codigo": "VALIDACAO_NEGOCIO",
      "mensagem": "A prioridade informada não existe no banco de dados."
    }
  ]
}