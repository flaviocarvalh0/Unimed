using TriagemClinica.Domain.Entities;
using TriagemClinica.Domain.Enums;
using TriagemClinica.Domain.Services.Interfaces;

namespace TriagemClinica.Domain.Services.Regras
{
    /// <summary>
    /// Regra 6 (Exemplo de Extensibilidade): Pacientes gestantes ganham +1 nível de prioridade.
    /// </summary>
    public class RegraGestante : IRegraTriagem
    {
        public NivelUrgencia Calcular(Paciente paciente, NivelUrgencia urgenciaAtual)
        {
            if (paciente.IsGestante && urgenciaAtual < NivelUrgencia.Critica)
            {
                return (NivelUrgencia)((int)urgenciaAtual + 1);
            }
            return urgenciaAtual;
        }
    }
}