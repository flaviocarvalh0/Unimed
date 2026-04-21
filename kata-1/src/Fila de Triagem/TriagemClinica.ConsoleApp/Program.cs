using System;
using System.Collections.Generic;
using TriagemClinica.Domain.Entities;
using TriagemClinica.Domain.Enums;
using TriagemClinica.Domain.Services;
using TriagemClinica.Domain.Services.Interfaces;
using TriagemClinica.Domain.Services.Regras;

var regras = new List<IRegraTriagem>
{
    new RegraIdosoMediaParaAlta(), // Regra 4
    new RegraMenorDeIdade(),       // Regra 5
    new RegraGestante()            // Regra 6 (Extra/Extensibilidade)
};

var service = new FilaService(regras);

var pacientes = new List<Paciente>
{
    // Caso 1: Urgência Crítica pura (Sempre 1º)
    new Paciente("Marcos (Crítico)", 45, NivelUrgencia.Critica, DateTime.Parse("11:00")),

    // Caso 2: Idoso com Urgência Média (Sobe para Alta - Regra 4)
    new Paciente("Sr. Zenon (Idoso Média -> Alta)", 70, NivelUrgencia.Media, DateTime.Parse("09:00")),

    // Caso 3: Menor de Idade com Urgência Média (Sobe para Alta - Regra 5)
    new Paciente("Lucas (Menor Média -> Alta)", 12, NivelUrgencia.Media, DateTime.Parse("08:30")),

    // Caso 4: Gestante com Urgência Baixa (Sobe para Média - Regra 6)
    new Paciente("Clara (Gestante Baixa -> Média)", 28, NivelUrgencia.Baixa, DateTime.Parse("07:00"), isGestante: true),

    // Caso 5: Idoso com Urgência Baixa (NÃO SOBE - Regra 4 só vale para Média)
    new Paciente("Sr. João (Idoso Baixa)", 65, NivelUrgencia.Baixa, DateTime.Parse("06:00")),

    // Caso 6: Adulto Comum Média (Sem promoção, deve perder para os que subiram para Alta)
    new Paciente("Roberto (Adulto Média)", 35, NivelUrgencia.Media, DateTime.Parse("08:00"))
};

var filaOrdenada = service.OrdenarFila(pacientes);

Console.WriteLine("==============================================================================");
Console.WriteLine("FILA DE ATENDIMENTO TRIADA - UNIMED CARUARU");
Console.WriteLine("==============================================================================");
Console.WriteLine(string.Format("{0,-30} | {1,-5} | {2,-8} | {3,-15}", "Nome", "Idade", "Chegada", "Urgência Final"));
Console.WriteLine("------------------------------------------------------------------------------");

foreach (var p in filaOrdenada)
{
    var urgenciaFinal = service.CalcularUrgenciaEfetiva(p);
    Console.WriteLine(string.Format("{0,-30} | {1,-5} | {2,-8} | {3,-15}",
        p.Nome, p.Idade, p.HorarioChegada.ToString("HH:mm"), urgenciaFinal));
}

Console.WriteLine("==============================================================================");