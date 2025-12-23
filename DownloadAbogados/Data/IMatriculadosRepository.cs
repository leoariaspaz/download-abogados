using DownloadAbogados.Data.DTOs;

namespace DownloadAbogados.Data;

public interface IMatriculadosRepository
{
	void ClearCache();

	bool Push(MatriculadoDTO matriculado);

	Task<bool> SaveAsync();
}
