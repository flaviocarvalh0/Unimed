import type { Tarefa } from "../types/Tarefas";

const API_URL = 'http://localhost:5132/api/tarefas'; 

export const TarefaService = {
  listar: async (concluida?: boolean): Promise<Tarefa[]> => {
    const url = concluida !== undefined ? `${API_URL}?concluida=${concluida}` : API_URL;
    const response = await fetch(url);
    if (!response.ok) throw new Error('Erro ao buscar tarefas');
    return response.json();
  },

  criar: async (titulo: string, prioridade: string = 'Normal'): Promise<Tarefa> => {
    const response = await fetch(API_URL, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ titulo, prioridade })
    });
    if (!response.ok) throw new Error('Erro ao criar tarefa');
    return response.json();
  },

  atualizarStatus: async (tarefa: Tarefa, estaConcluida: boolean): Promise<void> => {
    const response = await fetch(`${API_URL}/${tarefa.id}`, {
      method: 'PATCH',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        estaConcluida,
        dataAlteracaoOriginal: tarefa.dataAlteracao // Enviando o token para validar concorrência!
      })
    });
    
    if (response.status === 409) {
      throw new Error('Esta tarefa foi modificada por outro usuário. Recarregue a página.');
    }
    if (!response.ok) throw new Error('Erro ao atualizar tarefa');
  },

  excluir: async (id: number): Promise<void> => {
    const response = await fetch(`${API_URL}/${id}`, { method: 'DELETE' });
    if (!response.ok) throw new Error('Erro ao excluir tarefa');
  }
};