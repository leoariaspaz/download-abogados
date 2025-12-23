using DownloadAbogados.Data;
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
				foreach (var m in matriculados)
				{
					_matriculadosRepository.PushAsync(m);
				}
				count += matriculados.Count;
				await _matriculadosRepository.SaveAsync();
				_matriculadosRepository.ClearCacheAsync();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Excepción al procesar el parámetro: {parameter} - {ex.Message}");
			}
		}
		Console.WriteLine($"Total Matriculados: {count}");
	}
}
