using System.Collections;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace DownloadAbogados.Tests.Support;

internal sealed class RecordingDbConnection(int affectedRows) : DbConnection
{
	private ConnectionState _state = ConnectionState.Open;

	[AllowNull]
	public override string ConnectionString { get; set; } = string.Empty;
	public override string Database => "test";
	public override string DataSource => "test";
	public override string ServerVersion => "1";
	public override ConnectionState State => _state;
	public int OpenCalls { get; private set; }

	public override void ChangeDatabase(string databaseName)
	{
	}

	public override void Close() => _state = ConnectionState.Closed;

	public override void Open()
	{
		OpenCalls++;
		_state = ConnectionState.Open;
	}

	protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
	{
		throw new NotSupportedException();
	}

	protected override DbCommand CreateDbCommand() => new RecordingDbCommand(this, affectedRows);
}

internal sealed class RecordingDbCommand(DbConnection connection, int affectedRows) : DbCommand
{
	private readonly RecordingDbParameterCollection _parameters = new();

	[AllowNull]
	public override string CommandText { get; set; } = string.Empty;
	public override int CommandTimeout { get; set; }
	public override CommandType CommandType { get; set; }
	public override bool DesignTimeVisible { get; set; }
	public override UpdateRowSource UpdatedRowSource { get; set; }
	protected override DbConnection? DbConnection { get; set; } = connection;
	protected override DbParameterCollection DbParameterCollection => _parameters;
	protected override DbTransaction? DbTransaction { get; set; }

	public override void Cancel()
	{
	}

	public override int ExecuteNonQuery() => affectedRows;

	public override Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken) => Task.FromResult(affectedRows);

	public override object? ExecuteScalar() => affectedRows;

	public override void Prepare()
	{
	}

	protected override DbParameter CreateDbParameter() => new RecordingDbParameter();

	protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
	{
		throw new NotSupportedException();
	}
}

internal sealed class RecordingDbParameter : DbParameter
{
	public override DbType DbType { get; set; }
	public override ParameterDirection Direction { get; set; }
	public override bool IsNullable { get; set; }
	[AllowNull]
	public override string ParameterName { get; set; } = string.Empty;
	[AllowNull]
	public override string SourceColumn { get; set; } = string.Empty;
	public override object? Value { get; set; }
	public override bool SourceColumnNullMapping { get; set; }
	public override int Size { get; set; }

	public override void ResetDbType()
	{
	}
}

internal sealed class RecordingDbParameterCollection : DbParameterCollection
{
	private readonly List<DbParameter> _items = [];

	public override int Count => _items.Count;
	public override object SyncRoot => ((ICollection)_items).SyncRoot;

	public override int Add(object value)
	{
		_items.Add((DbParameter)value);
		return _items.Count - 1;
	}

	public override void AddRange(Array values)
	{
		foreach (var value in values)
		{
			Add(value!);
		}
	}

	public override void Clear() => _items.Clear();

	public override bool Contains(object value) => _items.Contains((DbParameter)value);

	public override bool Contains(string value) => IndexOf(value) >= 0;

	public override void CopyTo(Array array, int index) => ((ICollection)_items).CopyTo(array, index);

	public override IEnumerator GetEnumerator() => _items.GetEnumerator();

	public override int IndexOf(object value) => _items.IndexOf((DbParameter)value);

	public override int IndexOf(string parameterName) => _items.FindIndex(item => item.ParameterName == parameterName);

	public override void Insert(int index, object value) => _items.Insert(index, (DbParameter)value);

	public override void Remove(object value) => _items.Remove((DbParameter)value);

	public override void RemoveAt(int index) => _items.RemoveAt(index);

	public override void RemoveAt(string parameterName) => _items.RemoveAt(IndexOf(parameterName));

	protected override DbParameter GetParameter(int index) => _items[index];

	protected override DbParameter GetParameter(string parameterName) => _items[IndexOf(parameterName)];

	protected override void SetParameter(int index, DbParameter value) => _items[index] = value;

	protected override void SetParameter(string parameterName, DbParameter value)
	{
		var index = IndexOf(parameterName);
		if (index < 0)
		{
			_items.Add(value);
		}
		else
		{
			_items[index] = value;
		}
	}
}