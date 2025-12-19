using System;
using DownloadAbogados.Data;
using HtmlAgilityPack;
using DownloadAbogados.Services;

namespace DownloadAbogados.UseCases;

public class ParserService
{
	private readonly IMatriculadosRepository _matriculadosRepository;

	public ParserService(IMatriculadosRepository matriculadosRepository)
	{
		_matriculadosRepository = matriculadosRepository;
	}

	public async Task ProcesarAsync(IEnumerable<Matriculado> matriculados)
	{
		foreach (var m in matriculados)
		{
			await _matriculadosRepository.InsertAsync(m);
		}
	}

	public Matriculado HtmlNodeToMatriculado(HtmlNodeCollection parrafos)
	{
		var result = new Matriculado();
		foreach (var p in parrafos)
		{
			var texto = p.InnerText.Trim();

			if (texto.StartsWith("Nombre:"))
				result.Nombre = texto.Replace("Nombre:", "").Trim();

			else if (texto.StartsWith("Matricula:"))
			{
				// Matricula: 5229 - Fecha: 10/07/2025
				var partes = texto.Replace("Matricula:", "").Split(" - ");
				result.Matricula = partes[0].Trim();
				result.Fecha = partes.Length > 1
						? partes[1].Replace("Fecha:", "").Trim()
						: null;
			}
			else if (texto.StartsWith("Domicilio Legal:"))
				result.Domicilio = texto.Replace("Domicilio Legal:", "").Trim();

			else if (texto.StartsWith("Telefono:"))
				result.Telefono = texto.Replace("Telefono:", "").Trim();

			else if (texto.StartsWith("Correo:"))
				result.Correo = texto.Replace("Correo:", "").Trim();
		}

		return result;
	}

	public async void  SomeOtherMethod()
	{
		var dict = new Dictionary<string, Matriculado>();
		var generator = new QueryCriteriaGenerator();
		while (generator.GetNextQueryParameter() is string parameter)
		{
			try
			{
				
				var response = await CustomHttpClient.PostWithRetryAsync(parameter);

				if (response.IsSuccessStatusCode)
				{
						string responseBody = await response.Content.ReadAsStringAsync();

						var htmlDoc = new HtmlDocument();
						htmlDoc.LoadHtml(responseBody);

						var xpath = "//main//article//div[1]";
						var node = htmlDoc.DocumentNode.SelectSingleNode(xpath);
						while (node != null)
						{
							var m = HtmlNodeToMatriculado(node.SelectNodes(".//p"));
							if (m.Matricula != null && !dict.ContainsKey(m.Matricula))
							{
								dict[m.Matricula] = m;
								//Console.WriteLine(m);
							}
							node = node.NextSibling;
						}
				}
				else
				{
						Console.WriteLine($"Error: {response.StatusCode}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Excepción al procesar el parámetro: {parameter} - {ex.Message}");
			}
		}
		Console.WriteLine($"Total Matriculados: {dict.Count}");

	}
}
