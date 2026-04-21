using GerenciadorTarefas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace GerenciadorTarefas.Infrastructure.Data
{
    public class TarefaContexto : DbContext
    {
        public TarefaContexto(DbContextOptions<TarefaContexto> options) : base(options) { }

        public DbSet<Tarefa> Tarefas => Set<Tarefa>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Tarefa>()
                .Property(t => t.DataAlteracao)
                .IsConcurrencyToken();
        }
    }
}