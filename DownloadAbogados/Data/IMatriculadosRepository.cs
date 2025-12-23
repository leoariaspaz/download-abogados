namespace DownloadAbogados.Data;

public interface IMatriculadosRepository
{
	void ClearCacheAsync();

	void PushAsync(Matriculado matriculado);

	Task<bool> SaveAsync();
}
