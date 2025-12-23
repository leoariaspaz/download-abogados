using Dapper;
using DownloadAbogados.Data.DTOs;

namespace DownloadAbogados.Data;

public class MatriculadosRepository(MySqlConnectionFactory connectionFactory) : IMatriculadosRepository
{
	private readonly Dictionary<int, MatriculadoDTO> _matriculados = [];
	private readonly MySqlConnectionFactory _connectionFactory = connectionFactory;

	public bool Push(MatriculadoDTO matriculado)
	{
		if (!_matriculados.ContainsKey(matriculado.Matricula))
		{
			_matriculados[matriculado.Matricula] = matriculado;
			return true;
		}
		return false;
	}

	public void ClearCache()
	{
		_matriculados.Clear();
	}

	public async Task<bool> SaveAsync()
	{
		try
		{
			var count = 0;
			foreach (var matriculado in _matriculados.Values.Where(m => !m.Saved))
			{
				using var connection = _connectionFactory.CreateConnection();
				var query = @"INSERT IGNORE INTO rivendel.Patrocinante(nombre, nroMatricula, domicilio, localidad, nroCasillero)
					VALUES(@Nombre, @Matricula, @Domicilio, null, null);";
				var result = await connection.ExecuteAsync(query, matriculado);
				matriculado.Saved = true;
				if (result > 0)
				{
					count++;					
				}
			}
			if (count > 0)
			{
				Console.WriteLine($"{count} nuevos matriculados guardados en la base de datos.");
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
