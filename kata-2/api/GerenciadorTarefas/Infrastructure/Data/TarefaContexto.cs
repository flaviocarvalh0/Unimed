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

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entradas = ChangeTracker
                .Entries()
                .Where(e => e.Entity is Tarefa && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entrada in entradas)
            {
                ((Tarefa)entrada.Entity).AtualizarDataAlteracao();
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}