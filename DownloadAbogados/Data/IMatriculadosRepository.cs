using System;

namespace DownloadAbogados.Data;

public interface IMatriculadosRepository
{
	Task ClearCacheAsync();

	Task PushAsync(Matriculado matriculado);

	Task<bool> SaveAsync();
}
