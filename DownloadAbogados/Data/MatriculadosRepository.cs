using Dapper;

namespace DownloadAbogados.Data;

public class MatriculadosRepository(MySqlConnectionFactory connectionFactory) : IMatriculadosRepository
{
	private readonly Dictionary<string, Matriculado> _matriculados = [];
	private readonly MySqlConnectionFactory _connectionFactory = connectionFactory;

	public void PushAsync(Matriculado matriculado)
	{
		if (matriculado.Matricula != null && !_matriculados.ContainsKey(matriculado.Matricula))
		{
			_matriculados[matriculado.Matricula] = matriculado;
		}
	}

	public void ClearCacheAsync()
	{
		_matriculados.Clear();
	}

	public async Task<bool> SaveAsync()
	{
		try
		{
			foreach (var matriculado in _matriculados.Values)
			{
				using var connection = _connectionFactory.CreateConnection();
				var query = @"INSERT IGNORE INTO rivendel.Patrocinante(nombre, nroMatricula, domicilio, localidad, nroCasillero)
					VALUES(@Nombre, @Matricula, @Domicilio, null, null);";
				await connection.ExecuteAsync(query, matriculado);
			}
			return true;
		}
		catch (Exception ex)
		{
			Console.WriteLine("Error al guardar los datos en la base de datos.", ex);
			return false;
		}
	}
}
