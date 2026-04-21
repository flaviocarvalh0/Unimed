using System;
using System.Collections.Generic;
using System.Linq;
using TriagemClinica.Domain.Entities;
using TriagemClinica.Domain.Enums;
using TriagemClinica.Domain.Services;
using TriagemClinica.Domain.Services.Interfaces;
using TriagemClinica.Domain.Services.Regras;
using Xunit;

namespace TriagemClinica.Tests
{
    public class FilaServiceTests
    {
        // Injetando as 3 regras agora (Idoso, Menor e Gestante)
        private readonly FilaService _service = new(new List<IRegraTriagem>
        {
            new RegraIdosoMediaParaAlta(),
            new RegraMenorDeIdade(),
            new RegraGestante()
        });

        [Fact]
        public void Deve_Validar_Todos_Os_Cenarios_De_Prioridade_Simultaneamente()
        {
            // Arrange - Criando um cenário com todos os tipos de regras aplicadas
            var p1 = new Paciente("Normal Baixa", 30, NivelUrgencia.Baixa, DateTime.Parse("08:00"));

            // Regra 4: Idoso Média -> Alta (Chega às 09:00)
            var p2 = new Paciente("Idoso Média", 65, NivelUrgencia.Media, DateTime.Parse("09:00"));

            // Regra 5: Menor Média -> Alta (Chega às 08:30)
            var p3 = new Paciente("Menor Média", 15, NivelUrgencia.Media, DateTime.Parse("08:30"));

            // Regra 6: Gestante Baixa -> Média (Chega às 07:00)
            var p4 = new Paciente("Gestante Baixa", 25, NivelUrgencia.Baixa, DateTime.Parse("07:00"), isGestante: true);

            // Crítica Pura (Chega por último mas deve ir primeiro)
            var p5 = new Paciente("Adulto Crítica", 40, NivelUrgencia.Critica, DateTime.Parse("10:00"));

            var fila = new[] { p1, p2, p3, p4, p5 };

            // Act
            var resultado = _service.OrdenarFila(fila).ToList();

            // Assert
            // 1º: Adulto Crítica (Urgência máxima ganha de todos)
            Assert.Equal("Adulto Crítica", resultado[0].Nome);

            // 2º: Menor Média (Subiu para Alta e chegou às 08:30)
            Assert.Equal("Menor Média", resultado[1].Nome);

            // 3º: Idoso Média (Subiu para Alta e chegou às 09:00 - perde para o menor pelo horário)
            Assert.Equal("Idoso Média", resultado[2].Nome);

            // 4º: Gestante Baixa (Subiu para Média - ganha do Normal Baixa)
            Assert.Equal("Gestante Baixa", resultado[3].Nome);

            // 5º: Normal Baixa (Não teve promoção)
            Assert.Equal("Normal Baixa", resultado[4].Nome);
        }

        [Fact]
        public void Regra6_Gestante_Deve_Subir_Um_Nivel()
        {
            // Arrange
            var gestanteBaixa = new Paciente("Ana Gestante", 28, NivelUrgencia.Baixa, DateTime.Parse("10:00"), isGestante: true);
            var comumBaixa = new Paciente("Carlos Comum", 30, NivelUrgencia.Baixa, DateTime.Parse("09:00"));

            var fila = new[] { comumBaixa, gestanteBaixa };

            // Act
            var resultado = _service.OrdenarFila(fila).ToList();

            // Assert
            // Ana era Baixa mas virou Média. Carlos continuou Baixa. Ana passa na frente mesmo chegando depois.
            Assert.Equal("Ana Gestante", resultado[0].Nome);
        }
    }
}