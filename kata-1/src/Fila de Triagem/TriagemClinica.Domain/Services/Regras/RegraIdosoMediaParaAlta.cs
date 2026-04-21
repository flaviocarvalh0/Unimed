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
    public class RegraIdosoMediaParaAlta : IRegraTriagem
    {
        public NivelUrgencia Calcular(Paciente paciente, NivelUrgencia urgenciaAtual)
        {
            if (paciente.Idade >= 60 && urgenciaAtual == NivelUrgencia.Media)
            {
                return NivelUrgencia.Alta;
            }
            return urgenciaAtual;
        }
    }
}
