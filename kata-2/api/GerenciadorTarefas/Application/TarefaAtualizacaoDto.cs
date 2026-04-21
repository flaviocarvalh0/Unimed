using System;

namespace GerenciadorTarefas.Application.DTOs
{
    public class TarefaAtualizacaoDto
    {
        public string? Titulo { get; set; }
        public bool? EstaConcluida { get; set; }
        public string? Prioridade { get; set; }

        /// <summary>
        /// Data que o frontend possui antes de tentar editar. 
        /// Essencial para validar concorrência.
        /// </summary>
        public DateTime DataAlteracaoOriginal { get; set; }
    }
}