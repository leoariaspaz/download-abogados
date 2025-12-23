using DownloadAbogados.Domain;

namespace DownloadAbogados.Services;

public interface IMatriculadosProvider
{
	public Task<List<Matriculado>> GetMatriculadosListAsync(string queryCriteria);
}
