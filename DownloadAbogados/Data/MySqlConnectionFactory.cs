using System.Data;
using MySql.Data.MySqlClient;

namespace DownloadAbogados.Data;

public class MySqlConnectionFactory
{
	private readonly string _connectionString;

	public MySqlConnectionFactory(string connectionString)
	{
		_connectionString = connectionString;
	}

	public IDbConnection CreateConnection()
	{
		var conn = new MySqlConnection(_connectionString);
		conn.Open();
		return conn;
	}

}
