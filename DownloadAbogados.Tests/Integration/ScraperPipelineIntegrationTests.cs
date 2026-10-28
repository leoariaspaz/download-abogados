using System.Net;
using DownloadAbogados.Data;
using DownloadAbogados.Data.DTOs;
using DownloadAbogados.Services;
using DownloadAbogados.Tests.Support;
using DownloadAbogados.UseCases;
using Moq;

namespace DownloadAbogados.Tests.Integration;

public class ScraperPipelineIntegrationTests
{
	[Fact]
	public async Task ParserAndDownloader_ConvertResponseHtmlIntoRepositoryRecord()
	{
		string? submittedForm = null;
		using var client = new HttpClient(new CallbackHttpMessageHandler(async (request, _) =>
		{
			submittedForm = await request.Content!.ReadAsStringAsync();
			return new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent(
					"<main><article><div><p>Nombre: Grace Hopper</p><p>Matricula: 42 - Fecha: 01/02/2025</p><p>Domicilio Legal: Calle 9</p><p>Telefono: 555</p><p>Correo: grace@example.test</p></div></article></main>")
			};
		}))
		{
			BaseAddress = new Uri("https://example.test/")
		};
		var repository = new Mock<IMatriculadosRepository>();
		MatriculadoDTO? savedRecord = null;
		repository.Setup(item => item.Push(It.IsAny<MatriculadoDTO>()))
			.Callback<MatriculadoDTO>(record => savedRecord = record)
			.Returns(true);
		repository.Setup(item => item.SaveAsync()).ReturnsAsync(true);
		var downloader = new DownloaderHttpClient(client, _ => Task.CompletedTask);
		var generator = new Mock<IQueryCriteriaGenerator>();
		generator.SetupSequence(item => item.GetNextQueryParameter())
			.Returns("gra")
			.Returns((string?)null);
		var parser = new MatriculadosParser(generator.Object, downloader, repository.Object);

		await parser.ExecuteAsync();

		Assert.Equal("b=gra", submittedForm);
		Assert.NotNull(savedRecord);
		Assert.Equal("Grace Hopper", savedRecord.Nombre);
		Assert.Equal(42, savedRecord.Matricula);
		Assert.Equal("01/02/2025", savedRecord.Fecha);
		Assert.Equal("Calle 9", savedRecord.Domicilio);
		Assert.Equal("555", savedRecord.Telefono);
		Assert.Equal("grace@example.test", savedRecord.Correo);
		repository.Verify(item => item.SaveAsync(), Times.Once);
	}
}