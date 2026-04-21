using System;
using TriagemClinica.Domain.Enums;

namespace TriagemClinica.Domain.Entities
{
    public class Paciente
    {
        public string Nome { get; private set; }
        public int Idade { get; private set; }
        public NivelUrgencia Urgencia { get; private set; }
        public DateTime HorarioChegada { get; private set; }
        public bool IsGestante { get; set; } = false;

        public Paciente(string nome, int idade, NivelUrgencia urgencia, DateTime horarioChegada, bool isGestante = false)
        {
            Nome = nome;
            Idade = idade;
            Urgencia = urgencia;
            HorarioChegada = horarioChegada;
            IsGestante = isGestante;
        }
    }
}