using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GerenciadorTarefas.Domain.Entities;
using GerenciadorTarefas.Domain.Interfaces;
using GerenciadorTarefas.Application.DTOs;
using Application.DTOs;

namespace GerenciadorTarefas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TarefasController : ControllerBase
    {
        private readonly ITarefaRepositorio _repositorio;

        public TarefasController(ITarefaRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] bool? concluida)
        {
            var tarefas = await _repositorio.ObterTodasAsync(concluida);
            return Ok(tarefas);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] TarefaCriacaoDto dto)
        {
            var novaTarefa = new Tarefa(dto.Titulo, dto.Prioridade ?? "Normal");
            await _repositorio.AdicionarAsync(novaTarefa);
            return CreatedAtAction(nameof(Listar), new { id = novaTarefa.Id }, novaTarefa);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] TarefaAtualizacaoDto dto)
        {
            var tarefa = await _repositorio.ObterPorIdAsync(id);
            if (tarefa == null) return NotFound();

            // Validação Manual de Concorrência
            if (tarefa.DataAlteracao != dto.DataAlteracaoOriginal)
            {
                return Conflict(new
                {
                    mensagem = "Os dados foram alterados por outro usuário. Por favor, recarregue a lista."
                });
            }

            try
            {
                if (dto.EstaConcluida.HasValue)
                    tarefa.MudarStatus(dto.EstaConcluida.Value);

                tarefa.AtualizarDetalhes(dto.Titulo ?? string.Empty, dto.Prioridade ?? string.Empty);

                await _repositorio.AtualizarAsync(tarefa);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { mensagem = "Erro de concorrência ao persistir os dados no banco." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            var tarefa = await _repositorio.ObterPorIdAsync(id);
            if (tarefa == null) return NotFound();

            await _repositorio.RemoverAsync(tarefa);
            return NoContent();
        }
    }
}