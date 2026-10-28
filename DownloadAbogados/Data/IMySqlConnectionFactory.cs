using System.Data;

namespace DownloadAbogados.Data;

public interface IMySqlConnectionFactory
{
	IDbConnection CreateConnection();
}