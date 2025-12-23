using DownloadAbogados.Data;
using DownloadAbogados.Data.DTOs;
using DownloadAbogados.Services;

namespace DownloadAbogados.UseCases;

public class MatriculadosParser(IQueryCriteriaGenerator generator,
	IMatriculadosProvider provider,
	IMatriculadosRepository repository) : IParser
{
	private readonly IQueryCriteriaGenerator _queryCriteriaGenerator = generator;
	private readonly IMatriculadosProvider _matriculadosProvider = provider;
	private readonly IMatriculadosRepository _matriculadosRepository = repository;

	public async Task ExecuteAsync()
	{
		int count = 0;
		while (_queryCriteriaGenerator.GetNextQueryParameter() is string parameter)
		{
			try
			{
				var matriculados = await _matriculadosProvider.GetMatriculadosListAsync(parameter);
				var lastCount = count;
				foreach (var m in matriculados)
				{
					if (_matriculadosRepository.Push(new MatriculadoDTO(m)))
					{
						count++;
					}					
				}
				if (parameter.EndsWith("a") && lastCount != count)
				{
					Console.WriteLine($"Último parámetro procesado: {parameter} - Total acumulado: {count}");
				}
				await _matriculadosRepository.SaveAsync();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Excepción al procesar el parámetro: {parameter} - {ex.Message}");
			}
		}
		Console.WriteLine($"Total Matriculados: {count}");
	}
}
