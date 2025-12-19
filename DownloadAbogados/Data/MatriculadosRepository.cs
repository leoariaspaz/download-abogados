using System;
using Dapper;

namespace DownloadAbogados.Data;

public class MatriculadosRepository : IMatriculadosRepository
{
	private readonly Dictionary<string, Matriculado> _matriculados;
	private readonly MySqlConnectionFactory _connectionFactory;

	public MatriculadosRepository(MySqlConnectionFactory connectionFactory)
	{
		_connectionFactory = connectionFactory;
		_matriculados = new Dictionary<string, Matriculado>();
	}

	public async Task PushAsync(Matriculado matriculado)
	{
		if (matriculado.Matricula != null && !_matriculados.ContainsKey(matriculado.Matricula))
		{
			_matriculados[matriculado.Matricula] = matriculado;
		}
	}

	public async Task ClearCacheAsync()
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
		}catch (Exception)
		{
			return false;
		}
	}
}
