using System;

namespace GerenciadorTarefas.Domain.Entities
{
    public class Tarefa
    {
        public int Id { get; private set; }
        public string Titulo { get; private set; } = string.Empty;
        public bool EstaConcluida { get; private set; }
        public DateTime CriadaEm { get; private set; }
        public string Prioridade { get; private set; } = "Normal";
        public DateTime DataAlteracao { get; private set; }

        protected Tarefa() { }

        public Tarefa(string titulo, string prioridade = "Normal")
        {
            if (string.IsNullOrWhiteSpace(titulo))
                throw new ArgumentException("O título não pode ser vazio.");

            Titulo = titulo;
            Prioridade = prioridade;
            EstaConcluida = false;
            CriadaEm = DateTime.UtcNow;
            DataAlteracao = CriadaEm;
        }

        public void AtualizarDetalhes(string titulo, string prioridade)
        {
            if (!string.IsNullOrWhiteSpace(titulo)) Titulo = titulo;
            if (!string.IsNullOrWhiteSpace(prioridade)) Prioridade = prioridade;

            DataAlteracao = DateTime.UtcNow;
        }

        public void MudarStatus(bool concluida)
        {
            EstaConcluida = concluida;
            DataAlteracao = DateTime.UtcNow;
        }
    }
}