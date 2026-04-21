using GerenciadorTarefas.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorTarefas.Domain.Interfaces
{
    public interface ITarefaRepositorio
    {
        Task<IEnumerable<Tarefa>> ObterTodasAsync(bool? concluida = null);
        Task<Tarefa?> ObterPorIdAsync(int id);
        Task AdicionarAsync(Tarefa tarefa);
        Task AtualizarAsync(Tarefa tarefa);
        Task RemoverAsync(Tarefa tarefa);
    }
}
