using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriagemClinica.Domain.Entities;

namespace TriagemClinica.Domain.Services.Interfaces
{
    public interface IFilaService
    {
        IEnumerable<Paciente> OrdenarFila(IEnumerable<Paciente> pacientes);
    }
}
