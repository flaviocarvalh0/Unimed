using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriagemClinica.Domain.Entities;
using TriagemClinica.Domain.Enums;
using TriagemClinica.Domain.Services.Interfaces;

namespace TriagemClinica.Domain.Services.Regras
{
    public class RegraMenorDeIdade : IRegraTriagem
    {
        public NivelUrgencia Calcular(Paciente paciente, NivelUrgencia urgenciaAtual)
        {
            if (paciente.Idade < 18 && urgenciaAtual < NivelUrgencia.Critica)
            {
                return (NivelUrgencia)((int)urgenciaAtual + 1);
            }
            return urgenciaAtual;
        }
    }
}
