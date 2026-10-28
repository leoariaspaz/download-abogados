using System.Data;
using DownloadAbogados.Data;
using DownloadAbogados.Data.DTOs;
using DownloadAbogados.Domain;
using DownloadAbogados.Tests.Support;
using Moq;

namespace DownloadAbogados.Tests.Data;

public class MatriculadosRepositoryTests
{
	[Fact]
	public void MySqlConnectionFactory_RejectsInvalidConnectionString()
	{
		var factory = new MySqlConnectionFactory("invalid");

		Assert.ThrowsAny<Exception>(() => factory.CreateConnection());
	}

	[Fact]
	public void MySqlConnectionFactory_OpensAndReturnsCreatedConnection()
	{
		using var connection = new RecordingDbConnection(0);
		var factory = new MySqlConnectionFactory("test", _ => connection);

		var result = factory.CreateConnection();

		Assert.Same(connection, result);
		Assert.Equal(1, connection.OpenCalls);
	}

	[Fact]
	public void MySqlConnectionFactory_PropagatesOpenFailure()
	{
		var connection = new Mock<IDbConnection>();
		connection.Setup(item => item.Open()).Throws(new InvalidOperationException("offline"));
		var factory = new MySqlConnectionFactory("test", _ => connection.Object);

		Assert.Throws<InvalidOperationException>(() => factory.CreateConnection());
	}

	[Fact]
	public void Push_RejectsDuplicateRegistrationNumbers()
	{
		var connectionFactory = new Mock<IMySqlConnectionFactory>();
		var repository = new MatriculadosRepository(connectionFactory.Object);

		Assert.True(repository.Push(new MatriculadoDTO(new Matriculado { Matricula = 11 })));
		Assert.False(repository.Push(new MatriculadoDTO(new Matriculado { Matricula = 11 })));
	}

	[Fact]
	public async Task SaveAsync_ReturnsTrueWhenThereAreNoPendingRecords()
	{
		var connectionFactory = new Mock<IMySqlConnectionFactory>();
		var repository = new MatriculadosRepository(connectionFactory.Object);

		var result = await repository.SaveAsync();

		Assert.True(result);
		connectionFactory.Verify(item => item.CreateConnection(), Times.Never);
	}

	[Fact]
	public async Task SaveAsync_SkipsRecordsAlreadyMarkedSaved()
	{
		var connectionFactory = new Mock<IMySqlConnectionFactory>();
		var repository = new MatriculadosRepository(connectionFactory.Object);
		var record = new MatriculadoDTO(new Matriculado { Matricula = 12 }) { Saved = true };
		repository.Push(record);

		var result = await repository.SaveAsync();

		Assert.True(result);
		connectionFactory.Verify(item => item.CreateConnection(), Times.Never);
	}

	[Fact]
	public async Task SaveAsync_ReturnsFalseWhenConnectionFails()
	{
		var connectionFactory = new Mock<IMySqlConnectionFactory>();
		connectionFactory.Setup(item => item.CreateConnection()).Throws(new InvalidOperationException("offline"));
		var repository = new MatriculadosRepository(connectionFactory.Object);
		var record = new MatriculadoDTO(new Matriculado { Matricula = 13 });
		repository.Push(record);

		var result = await repository.SaveAsync();

		Assert.False(result);
		Assert.False(record.Saved);
		connectionFactory.Verify(item => item.CreateConnection(), Times.Once);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	public async Task SaveAsync_ExecutesInsertAndMarksRecordSaved(int affectedRows)
	{
		using var connection = new RecordingDbConnection(affectedRows);
		var connectionFactory = new Mock<IMySqlConnectionFactory>();
		connectionFactory.Setup(item => item.CreateConnection()).Returns(connection);
		var repository = new MatriculadosRepository(connectionFactory.Object);
		var record = new MatriculadoDTO(new Matriculado { Nombre = "Ada", Matricula = 15 });
		repository.Push(record);

		var result = await repository.SaveAsync();

		Assert.True(result);
		Assert.True(record.Saved);
		connectionFactory.Verify(item => item.CreateConnection(), Times.Once);
	}

	[Fact]
	public void ClearCache_AllowsRegistrationToBePushedAgain()
	{
		var connectionFactory = new Mock<IMySqlConnectionFactory>();
		var repository = new MatriculadosRepository(connectionFactory.Object);
		var record = new MatriculadoDTO(new Matriculado { Matricula = 14 });
		repository.Push(record);

		repository.ClearCache();

		Assert.True(repository.Push(new MatriculadoDTO(new Matriculado { Matricula = 14 })));
	}
}