using DownloadAbogados.Data;
using DownloadAbogados.Data.DTOs;
using DownloadAbogados.Domain;
using DownloadAbogados.Services;
using DownloadAbogados.UseCases;
using Moq;

namespace DownloadAbogados.Tests.UseCases;

public class MatriculadosParserTests
{
	[Fact]
	public async Task ExecuteAsync_ProcessesRecordsAndSavesEachQuery()
	{
		var generator = new Mock<IQueryCriteriaGenerator>();
		generator.SetupSequence(item => item.GetNextQueryParameter())
			.Returns("ada")
			.Returns("abb")
			.Returns((string?)null);
		var provider = new Mock<IMatriculadosProvider>();
		provider.Setup(item => item.GetMatriculadosListAsync("ada"))
			.ReturnsAsync([new Matriculado { Nombre = "Ada", Matricula = 1 }]);
		provider.Setup(item => item.GetMatriculadosListAsync("abb"))
			.ReturnsAsync([new Matriculado { Nombre = "Augusta", Matricula = 2 }]);
		var repository = new Mock<IMatriculadosRepository>();
		repository.Setup(item => item.Push(It.IsAny<MatriculadoDTO>())).Returns(true);
		repository.Setup(item => item.SaveAsync()).ReturnsAsync(true);
		var parser = new MatriculadosParser(generator.Object, provider.Object, repository.Object);

		await parser.ExecuteAsync();

		provider.Verify(item => item.GetMatriculadosListAsync("ada"), Times.Once);
		provider.Verify(item => item.GetMatriculadosListAsync("abb"), Times.Once);
		repository.Verify(item => item.Push(It.Is<MatriculadoDTO>(record => record.Nombre == "Ada")), Times.Once);
		repository.Verify(item => item.SaveAsync(), Times.Exactly(2));
	}

	[Fact]
	public async Task ExecuteAsync_ContinuesAfterProviderFailure()
	{
		var generator = new Mock<IQueryCriteriaGenerator>();
		generator.SetupSequence(item => item.GetNextQueryParameter())
			.Returns("err")
			.Returns((string?)null);
		var provider = new Mock<IMatriculadosProvider>();
		provider.Setup(item => item.GetMatriculadosListAsync("err"))
			.ThrowsAsync(new HttpRequestException("offline"));
		var repository = new Mock<IMatriculadosRepository>();
		var parser = new MatriculadosParser(generator.Object, provider.Object, repository.Object);

		await parser.ExecuteAsync();

		repository.Verify(item => item.SaveAsync(), Times.Never);
	}

	[Fact]
	public async Task ExecuteAsync_OnlyCountsNewRecordsAndHandlesSaveFailure()
	{
		var generator = new Mock<IQueryCriteriaGenerator>();
		generator.SetupSequence(item => item.GetNextQueryParameter())
			.Returns("aba")
			.Returns("abc")
			.Returns("cda")
			.Returns((string?)null);
		var provider = new Mock<IMatriculadosProvider>();
		provider.Setup(item => item.GetMatriculadosListAsync(It.IsAny<string>()))
			.ReturnsAsync([new Matriculado { Nombre = "Same", Matricula = 7 }]);
		var repository = new Mock<IMatriculadosRepository>();
		repository.SetupSequence(item => item.Push(It.IsAny<MatriculadoDTO>()))
			.Returns(true)
			.Returns(false)
			.Returns(false);
		repository.SetupSequence(item => item.SaveAsync())
			.ReturnsAsync(false)
			.ReturnsAsync(true)
			.ReturnsAsync(true);
		var parser = new MatriculadosParser(generator.Object, provider.Object, repository.Object);

		await parser.ExecuteAsync();

		repository.Verify(item => item.Push(It.IsAny<MatriculadoDTO>()), Times.Exactly(3));
		repository.Verify(item => item.SaveAsync(), Times.Exactly(3));
	}
}