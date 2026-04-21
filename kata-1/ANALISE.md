1. Qual estrutura de dados você escolheu para modelar a fila e por quê?
    Optei por utilizar uma Lista Genérica (List<T>) combinada com o motor de ordenação LINQ do .NET.

    Justificativa: Para o contexto de uma fila de triagem de clínica médica, o volume de dados (pacientes aguardando) é baixo, geralmente variando de dezenas a poucas centenas de registros simultâneos. O uso de uma lista com LINQ privilegia a legibilidade e a manutenibilidade. A estrutura permite expressar as regras de negócio de forma declarativa e clara, facilitando auditorias de código e alterações futuras sem a complexidade de estruturas de dados personalizadas.

2. Qual a complexidade de tempo do seu algoritmo de ordenação? Seria diferente se a lista tivesse 1 milhão de pacientes?

    A complexidade de tempo atual é O(N log N). Isso ocorre porque o método OrderBy do .NET utiliza algoritmos de ordenação eficientes (como o Quicksort ou Timsort).

    Cenário com 1 milhão de pacientes: Sim, a abordagem precisaria ser alterada para manter a performance. Em um cenário de altíssimo volume, reordenar a lista inteira a cada inserção seria custoso. A solução ideal seria utilizar Filas de Prioridade Múltiplas (Buckets):

    1. Como os níveis de urgência são finitos (apenas 4 níveis), criaríamos 4 filas distintas (uma para cada urgência).

    2. A inserção passaria a ser O(1) (apenas adicionar ao final da fila correspondente).

    3. O consumo (chamar o próximo paciente) também seria O(1), bastando buscar o primeiro registro da fila de maior prioridade que não esteja vazia.

3. As regras 4 e 5 interagem entre si? Descreva o que acontece quando um paciente tem 15 anos e urgência MÉDIA.

    As regras 4 e 5 tratam de grupos biológicos distintos (idosos e menores de idade), portanto, não há sobreposição de aplicação no mesmo indivíduo.

    1. Cenário (15 anos, Urgência MÉDIA):

    2. A Regra 4 é ignorada (o paciente não tem 60+ anos).

    3. A Regra 5 é aplicada: pacientes menores de 18 anos ganham +1 nível de prioridade.

    4. A urgência efetiva do paciente sobe de MÉDIA para ALTA.

    Na ordenação final, ele será posicionado junto aos demais pacientes de urgência ALTA, respeitando o critério de desempate por ordem de chegada (FIFO).

4. Se a clínica adicionasse uma 6ª regra amanhã, como seu código lidaria com essa extensão?

    O código foi projetado seguindo o Princípio Aberto/Fechado (SOLID), utilizando o padrão de projeto Strategy.

    Em vez de códigos fixos dentro do serviço, utilizei uma interface IRegraTriagem. O FilaService recebe uma lista dinâmica dessas regras.

    Extensibilidade: Para adicionar uma 6ª regra (ex: prioridade para gestantes), basta criar uma nova classe que implemente a interface e registrá-la no sistema.

    Segurança: Não é necessário alterar o código principal de ordenação para incluir, remover ou modificar regras individuais, o que reduz drasticamente o risco de efeitos colaterais e bugs em regras já existentes.