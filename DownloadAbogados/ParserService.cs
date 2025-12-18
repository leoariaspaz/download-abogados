using System;
using HtmlAgilityPack;

namespace DownloadAbogados;

public class ParserService
{
	public Matriculado Execute(HtmlNodeCollection parrafos)
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
}
