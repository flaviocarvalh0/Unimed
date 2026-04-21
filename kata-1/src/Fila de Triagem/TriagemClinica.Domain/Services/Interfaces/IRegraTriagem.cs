using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriagemClinica.Domain.Entities;
using TriagemClinica.Domain.Enums;

namespace TriagemClinica.Domain.Services.Interfaces
{
    public interface IRegraTriagem
    {
        NivelUrgencia Calcular(Paciente paciente, NivelUrgencia urgenciaAtual);
    }
}
