using System.Collections.Generic;
using System.Linq;
using TriagemClinica.Domain.Entities;
using TriagemClinica.Domain.Enums;
using TriagemClinica.Domain.Services.Interfaces;
using TriagemClinica.Domain.Services.Regras;

namespace TriagemClinica.Domain.Services
{
    public class FilaService
    {
        private readonly IEnumerable<IRegraTriagem> _regras;

        public FilaService(IEnumerable<IRegraTriagem> regras)
        {
            _regras = regras;
        }

        public IEnumerable<Paciente> OrdenarFila(IEnumerable<Paciente> pacientes)
        {
            return pacientes
                .OrderByDescending(CalcularUrgenciaEfetiva)
                .ThenBy(p => p.HorarioChegada)
                .ToList();
        }

        public NivelUrgencia CalcularUrgenciaEfetiva(Paciente paciente)
        {
            var urgenciaAtual = paciente.Urgencia;

            foreach (var regra in _regras)
            {
                urgenciaAtual = regra.Calcular(paciente, urgenciaAtual);
            }

            return urgenciaAtual;
        }
    }
}