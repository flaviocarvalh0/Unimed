import React, { useState, useEffect } from 'react';
import { TarefaService } from './services/api';
import type { FiltroStatus, Tarefa } from './types/Tarefas';


function App() {
  const [tarefas, setTarefas] = useState<Tarefa[]>([]);
  const [novoTitulo, setNovoTitulo] = useState('');
  const [novaPrioridade, setNovaPrioridade] = useState('Média');
  const [filtro, setFiltro] = useState<FiltroStatus>('todas');

  // 1. Estado do Toast (Notificação Flutuante)
  const [toast, setToast] = useState<{ mensagem: string; tipo: 'sucesso' | 'erro' } | null>(null);

  // 2. Estado do Modal de Confirmação
  const [modalExclusao, setModalExclusao] = useState<{ aberto: boolean; tarefaId: number | null }>({
    aberto: false,
    tarefaId: null,
  });

  useEffect(() => {
    carregarTarefas();
  }, [filtro]);

  // Função para exibir o Toast e fazê-lo sumir após 3 segundos
  const exibirToast = (mensagem: string, tipo: 'sucesso' | 'erro' = 'sucesso') => {
    setToast({ mensagem, tipo });
    setTimeout(() => {
      setToast(null);
    }, 3000);
  };

  const carregarTarefas = async () => {
    try {
      let status: boolean | undefined = undefined;
      if (filtro === 'pendentes') status = false;
      if (filtro === 'concluidas') status = true;

      const dados = await TarefaService.listar(status);
      setTarefas(dados);
    } catch (e) {
      exibirToast('Falha ao conectar com o servidor.', 'erro');
    }
  };

  const handleCriar = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (!novoTitulo.trim()) return;

    try {
      await TarefaService.criar(novoTitulo, novaPrioridade);
      setNovoTitulo('');
      setNovaPrioridade('Média');
      carregarTarefas();
      exibirToast('Tarefa criada com sucesso!'); // Toast de sucesso!
    } catch (e) {
      exibirToast('Falha ao criar tarefa.', 'erro');
    }
  };

  const handleAlternarStatus = async (tarefa: Tarefa) => {
    const novoStatus = !tarefa.estaConcluida;
    try {
      await TarefaService.atualizarStatus(tarefa, novoStatus);
      carregarTarefas();
      exibirToast(novoStatus ? 'Tarefa concluída!' : 'Tarefa reaberta!', 'sucesso');
    } catch (e: any) {
      exibirToast(e.message || 'Erro ao alterar status.', 'erro');
    }
  };

  // Prepara a exclusão (Abre o Modal ou bloqueia se estiver concluída)
  const solicitarExclusao = (id: number, estaConcluida: boolean) => {
    if (estaConcluida) {
      exibirToast('Ação bloqueada: Tarefas concluídas não podem ser excluídas.', 'erro');
      return;
    }
    // Se passou, abre o modal guardando o ID
    setModalExclusao({ aberto: true, tarefaId: id });
  };

  // Confirmação final (Quando clica em "Sim" no Modal)
  const confirmarExclusao = async () => {
    if (modalExclusao.tarefaId === null) return;

    try {
      await TarefaService.excluir(modalExclusao.tarefaId);
      carregarTarefas();
      exibirToast('Tarefa excluída definitivamente.', 'sucesso');
    } catch (e: any) {
      exibirToast(e.message || 'Falha ao excluir tarefa.', 'erro');
    } finally {
      // Fecha o modal independentemente de dar erro ou sucesso
      setModalExclusao({ aberto: false, tarefaId: null });
    }
  };

  const getCorPrioridade = (prioridade: string) => {
    switch (prioridade) {
      case 'Baixa': return { bg: '#dcfce7', text: '#166534' };
      case 'Média': return { bg: '#dbeafe', text: '#1e40af' };
      case 'Alta': return { bg: '#ffedd5', text: '#9a3412' };
      case 'Urgente': return { bg: '#fee2e2', text: '#991b1b' };
      default: return { bg: '#e5e7eb', text: '#374151' };
    }
  };

  const formatarData = (dataIso: string) => {
    if (!dataIso) return '';
    const dataComFuso = dataIso.endsWith('Z') ? dataIso : `${dataIso}Z`;
    const data = new Date(dataComFuso);
    return new Intl.DateTimeFormat('pt-BR', {
      day: '2-digit', month: '2-digit', year: 'numeric',
      hour: '2-digit', minute: '2-digit'
    }).format(data);
  };

  return (
    <>
      {/* TOAST NOTIFICATION (Fica flutuando no topo direito) */}
      {toast && (
        <div style={{
          position: 'fixed',
          top: '20px',
          right: '20px',
          backgroundColor: toast.tipo === 'sucesso' ? '#00995D' : '#ef4444',
          color: 'white',
          padding: '1rem 1.5rem',
          borderRadius: '8px',
          boxShadow: '0 4px 12px rgba(0,0,0,0.15)',
          zIndex: 9999,
          fontWeight: 'bold',
          transition: 'all 0.3s ease-in-out'
        }}>
          {toast.tipo === 'sucesso' ? '✅ ' : '❌ '}
          {toast.mensagem}
        </div>
      )}

      {/* MODAL DE CONFIRMAÇÃO (Fundo escuro + Caixa branca) */}
      {modalExclusao.aberto && (
        <div style={{
          position: 'fixed',
          top: 0, left: 0, right: 0, bottom: 0,
          backgroundColor: 'rgba(0, 0, 0, 0.5)', // Overlay escuro
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
          zIndex: 1000
        }}>
          <div style={{
            backgroundColor: 'white',
            padding: '2rem',
            borderRadius: '12px',
            maxWidth: '400px',
            width: '90%',
            boxShadow: '0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)'
          }}>
            <h2 style={{ marginTop: 0, color: '#111827', fontSize: '1.25rem' }}>Excluir Tarefa?</h2>
            <p style={{ color: '#4b5563', margin: '1rem 0 2rem 0' }}>
              Tem certeza que deseja remover esta tarefa da sua lista? Esta ação não pode ser desfeita.
            </p>
            <div style={{ display: 'flex', gap: '1rem', justifyContent: 'flex-end' }}>
              <button 
                onClick={() => setModalExclusao({ aberto: false, tarefaId: null })}
                style={{ padding: '0.5rem 1rem', background: '#e5e7eb', color: '#374151', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: 'bold' }}
              >
                Cancelar
              </button>
              <button 
                onClick={confirmarExclusao}
                style={{ padding: '0.5rem 1rem', background: '#ef4444', color: 'white', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: 'bold' }}
              >
                Sim, Excluir
              </button>
            </div>
          </div>
        </div>
      )}

      {/* CONTEÚDO PRINCIPAL (O CONTAINER) */}
      <div className="container">
        <h1 style={{ marginBottom: '2rem', color: '#00995D' }}>Painel de Tarefas</h1>

        <form onSubmit={handleCriar} style={{ display: 'flex', gap: '0.5rem', marginBottom: '2rem' }}>
          <input
            type="text"
            placeholder="O que precisa ser feito?"
            value={novoTitulo}
            onChange={(e) => setNovoTitulo(e.target.value)}
            style={{ flex: 1, padding: '0.75rem', borderRadius: '4px', border: '1px solid #ccc' }}
          />

          <select
            value={novaPrioridade}
            onChange={(e) => setNovaPrioridade(e.target.value)}
            style={{ padding: '0.75rem', borderRadius: '4px', border: '1px solid #ccc', backgroundColor: 'white', cursor: 'pointer' }}
          >
            <option value="Baixa">Baixa</option>
            <option value="Média">Média</option>
            <option value="Alta">Alta</option>
            <option value="Urgente">Urgente</option>
          </select>

          <button type="submit" style={{ padding: '0.75rem 1.5rem', background: '#00995D', color: 'white', border: 'none', borderRadius: '4px', cursor: 'pointer', fontWeight: 'bold' }}>
            Adicionar
          </button>
        </form>

        <div style={{ display: 'flex', gap: '1rem', marginBottom: '1.5rem' }}>
          <label style={{ cursor: 'pointer' }}>
            <input type="radio" checked={filtro === 'todas'} onChange={() => setFiltro('todas')} /> Todas
          </label>
          <label style={{ cursor: 'pointer' }}>
            <input type="radio" checked={filtro === 'pendentes'} onChange={() => setFiltro('pendentes')} /> Pendentes
          </label>
          <label style={{ cursor: 'pointer' }}>
            <input type="radio" checked={filtro === 'concluidas'} onChange={() => setFiltro('concluidas')} /> Concluídas
          </label>
        </div>

        <ul style={{ listStyle: 'none', padding: 0 }}>
          {tarefas.length === 0 && <p style={{ color: '#6b7280', textAlign: 'center', marginTop: '2rem' }}>Nenhuma tarefa encontrada.</p>}

          {tarefas.map(tarefa => {
            const cores = getCorPrioridade(tarefa.prioridade);

            return (
              <li key={tarefa.id} style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '1rem', background: '#f9fafb', border: '1px solid #e5e7eb', marginBottom: '0.5rem', borderRadius: '4px' }}>

                <div style={{ display: 'flex', gap: '1rem', alignItems: 'flex-start', flex: 1 }}>
                  <input
                    type="checkbox"
                    checked={tarefa.estaConcluida}
                    onChange={() => handleAlternarStatus(tarefa)}
                    style={{ width: '1.2rem', height: '1.2rem', cursor: 'pointer', marginTop: '0.2rem' }}
                  />

                  <div style={{ display: 'flex', flexDirection: 'column', gap: '0.3rem' }}>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '0.8rem' }}>
                      <span style={{ textDecoration: tarefa.estaConcluida ? 'line-through' : 'none', color: tarefa.estaConcluida ? '#9ca3af' : '#111827', fontWeight: 600, fontSize: '1.05rem' }}>
                        {tarefa.titulo}
                      </span>
                      <span style={{ fontSize: '0.7rem', backgroundColor: cores.bg, color: cores.text, padding: '0.2rem 0.6rem', borderRadius: '12px', fontWeight: 'bold' }}>
                        {tarefa.prioridade}
                      </span>
                    </div>

                    <div style={{ display: 'flex', gap: '1rem', fontSize: '0.75rem', color: '#6b7280' }}>
                      <span>📅 Criado em: {formatarData(tarefa.criadaEm)}</span>
                      {tarefa.dataAlteracao !== tarefa.criadaEm && (
                        <span>✏️ Atualizado em: {formatarData(tarefa.dataAlteracao)}</span>
                      )}
                    </div>
                  </div>
                </div>

                <button
                  onClick={() => solicitarExclusao(tarefa.id, tarefa.estaConcluida)} // Agora chama a função de preparar exclusão
                  style={{
                    background: 'transparent',
                    color: tarefa.estaConcluida ? '#d1d5db' : '#ef4444',
                    border: 'none',
                    cursor: tarefa.estaConcluida ? 'not-allowed' : 'pointer',
                    fontWeight: 'bold',
                    marginLeft: '1rem'
                  }}
                  title={tarefa.estaConcluida ? "Não é possível excluir tarefas concluídas" : "Excluir tarefa"}
                >
                  Excluir
                </button>
              </li>
            );
          })}
        </ul>
      </div>
    </>
  );
}

export default App;