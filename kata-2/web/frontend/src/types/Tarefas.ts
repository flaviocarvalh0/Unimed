export interface Tarefa {
  id: number;
  titulo: string;
  estaConcluida: boolean;
  prioridade: string;
  criadaEm: string;
  dataAlteracao: string; 
}

export type FiltroStatus = 'todas' | 'pendentes' | 'concluidas';