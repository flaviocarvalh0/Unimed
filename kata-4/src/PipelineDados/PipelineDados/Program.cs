using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PipelineDados;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("Iniciando Pipeline de Transformação de Dados...\n");

        var pathPedidos = "dados/pedidos.csv";
        var pathClientes = "dados/clientes.csv";
        var pathEntregas = "dados/entregas.csv";

        var clientes = LerCsv(pathClientes).Select(c => new
        {
            Id = c[0],
            Nome = c[1],
            Cidade = NormalizarCidade(c[2]),
            Estado = c[3]
        }).ToList();

        var pedidos = LerCsv(pathPedidos)
            .Where(p => !string.IsNullOrWhiteSpace(p[0]))
            .Select(p => new
            {
                IdPedido = p[0],
                DataPedido = ParseData(p[1]),
                IdCliente = p[2],
                ValorTotal = ParseDecimal(p[3]),
                Status = p[4]
            }).ToList();

        var entregas = LerCsv(pathEntregas).Select(e => new
        {
            IdPedido = e[1],
            DataPrevista = ParseData(e[2]),
            DataRealizada = ParseData(e[3]),
            StatusEntrega = e[4]
        }).ToList();

        var dadosConsolidados = (from p in pedidos
                                 join c in clientes on p.IdCliente equals c.Id into clienteJoin
                                 from c in clienteJoin.DefaultIfEmpty()
                                 join e in entregas on p.IdPedido equals e.IdPedido into entregaJoin
                                 from e in entregaJoin.DefaultIfEmpty()
                                 select new
                                 {
                                     IdPedido = p.IdPedido,
                                     NomeCliente = c?.Nome ?? "CLIENTE DESCONHECIDO",
                                     Cidade = c?.Cidade ?? "N/A",
                                     Estado = c?.Estado ?? "N/A",
                                     ValorTotal = p.ValorTotal,
                                     StatusPedido = p.Status,
                                     DataPedido = p.DataPedido,
                                     DataPrevista = e?.DataPrevista,
                                     DataRealizada = e?.DataRealizada,
                                     StatusEntrega = e?.StatusEntrega ?? "PENDENTE",
                                     AtrasoDias = (e?.DataRealizada.HasValue == true && e?.DataPrevista.HasValue == true)
                                         ? (int)(e.DataRealizada.Value - e.DataPrevista.Value).TotalDays
                                         : (int?)null
                                 }).ToList();

        // Salvar arquivo consolidado
        var linhasCsv = new List<string> { "id_pedido;nome_cliente;cidade_normalizada;estado;valor_total;status_pedido;data_pedido;data_prevista_entrega;data_realizada_entrega;atraso_dias;status_entrega" };
        linhasCsv.AddRange(dadosConsolidados.Select(d => $"{d.IdPedido};{d.NomeCliente};{d.Cidade};{d.Estado};{d.ValorTotal.ToString("F2", CultureInfo.InvariantCulture)};{d.StatusPedido};{d.DataPedido:yyyy-MM-dd};{d.DataPrevista:yyyy-MM-dd};{d.DataRealizada:yyyy-MM-dd};{d.AtrasoDias};{d.StatusEntrega}"));
        File.WriteAllLines("dados/consolidado.csv", linhasCsv);
        Console.WriteLine("✅ Arquivo consolidado.csv gerado com sucesso!\n");

        // 3. INDICADORES (Parte B)
        Console.WriteLine("📊 RELATÓRIO DE INDICADORES CONSOLIDADOS\n");

        Console.WriteLine("--- Total de Pedidos por Status ---");
        var pedidosPorStatus = dadosConsolidados.GroupBy(x => x.StatusPedido).Select(g => new { Status = g.Key, Total = g.Count() });
        foreach (var item in pedidosPorStatus) Console.WriteLine($"{item.Status}: {item.Total}");

        Console.WriteLine("\n--- Ticket Médio por Estado ---");
        var ticketPorEstado = dadosConsolidados.Where(x => x.Estado != "N/A").GroupBy(x => x.Estado).Select(g => new { Estado = g.Key, Media = g.Average(x => x.ValorTotal) });
        foreach (var item in ticketPorEstado) Console.WriteLine($"{item.Estado}: R$ {item.Media:F2}");

        Console.WriteLine("\n--- Performance de Entregas ---");
        var totalEntregues = dadosConsolidados.Count(x => x.DataRealizada.HasValue);
        var noPrazo = dadosConsolidados.Count(x => x.AtrasoDias <= 0);
        var atrasados = dadosConsolidados.Count(x => x.AtrasoDias > 0);
        if (totalEntregues > 0)
        {
            Console.WriteLine($"No Prazo / Antecipado: {(noPrazo * 100.0 / totalEntregues):F1}%");
            Console.WriteLine($"Com Atraso: {(atrasados * 100.0 / totalEntregues):F1}%");
        }

        Console.WriteLine("\n--- Top 3 Cidades (Volume de Pedidos) ---");
        var topCidades = dadosConsolidados.Where(x => x.Cidade != "N/A").GroupBy(x => x.Cidade).OrderByDescending(g => g.Count()).Take(3);
        foreach (var item in topCidades) Console.WriteLine($"{item.Key}: {item.Count()} pedidos");

        Console.WriteLine("\n--- Média de Atraso ---");
        var mediaAtraso = dadosConsolidados.Where(x => x.AtrasoDias > 0).Average(x => x.AtrasoDias);
        Console.WriteLine($"Tempo médio de atraso: {mediaAtraso:F1} dias");
    }

    static string[][] LerCsv(string caminho) => File.ReadAllLines(caminho)
        .Skip(1)
        .Where(l => !string.IsNullOrWhiteSpace(l)) 
        .Select(l => l.Split(';'))
        .Where(colunas => colunas.Length >= 4)    
        .ToArray();

    static string NormalizarCidade(string cidade)
    {
        if (string.IsNullOrWhiteSpace(cidade)) return "N/A";
        var textoNormalizado = cidade.Normalize(NormalizationForm.FormD);
        var textoSemAcento = Regex.Replace(textoNormalizado, @"[^a-zA-Z0-9 ]+", "");
        return textoSemAcento.ToUpper().Trim();
    }

    static decimal ParseDecimal(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor)) return 0;
        valor = valor.Replace(",", ".");
        return decimal.TryParse(valor, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : 0;
    }

    static DateTime? ParseData(string data)
    {
        if (string.IsNullOrWhiteSpace(data)) return null;
        if (DateTime.TryParse(data, new CultureInfo("pt-BR"), DateTimeStyles.None, out var dtPtBr)) return dtPtBr;
        if (DateTime.TryParse(data, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dtInv)) return dtInv;
        if (long.TryParse(data, out var unixTime)) return DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;
        return null;
    }
}