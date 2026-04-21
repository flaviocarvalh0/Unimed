using GerenciadorTarefas.Domain.Entities;
using GerenciadorTarefas.Domain.Interfaces;
using GerenciadorTarefas.Infrastructure.Data;
using GerenciadorTarefas.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorTarefas.Infrastructure.Repositories
{
    public class TarefaRepositorio : ITarefaRepositorio
    {
        private readonly TarefaContexto _contexto;

        public TarefaRepositorio(TarefaContexto contexto)
        {
            _contexto = contexto;
        }

        public async Task<IEnumerable<Tarefa>> ObterTodasAsync(bool? concluida = null)
        {
            var consulta = _contexto.Tarefas.AsQueryable();
            if (concluida.HasValue)
                consulta = consulta.Where(t => t.EstaConcluida == concluida.Value);

            return await consulta.OrderByDescending(t => t.CriadaEm).ToListAsync();
        }

        public async Task<Tarefa?> ObterPorIdAsync(int id)
        {
            return await _contexto.Tarefas.FindAsync(id);
        }

        public async Task AdicionarAsync(Tarefa tarefa)
        {
            await _contexto.Tarefas.AddAsync(tarefa);
            await _contexto.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Tarefa tarefa)
        {
            _contexto.Tarefas.Update(tarefa);
            await _contexto.SaveChangesAsync();
        }

        public async Task RemoverAsync(Tarefa tarefa)
        {
            _contexto.Tarefas.Remove(tarefa);
            await _contexto.SaveChangesAsync();
        }
    }
}