using System.Data;
using MySql.Data.MySqlClient;

namespace DownloadAbogados.Data;

public class MySqlConnectionFactory : IMySqlConnectionFactory
{
	private readonly string _connectionString;
	private readonly Func<string, IDbConnection> _createConnection;

	public MySqlConnectionFactory(string connectionString)
		: this(connectionString, value => new MySqlConnection(value))
	{
	}

	internal MySqlConnectionFactory(string connectionString, Func<string, IDbConnection> createConnection)
	{
		_connectionString = connectionString;
		_createConnection = createConnection;
	}

	public IDbConnection CreateConnection()
	{
		var conn = _createConnection(_connectionString);
		conn.Open();
		return conn;
	}
}
