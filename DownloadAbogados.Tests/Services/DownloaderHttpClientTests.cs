using System.Net;
using DownloadAbogados.Services;
using DownloadAbogados.Tests.Support;

namespace DownloadAbogados.Tests.Services;

public class DownloaderHttpClientTests
{
	[Fact]
	public async Task GetMatriculadosListAsync_ParsesAllFieldsFromHtml()
	{
		using var client = CreateClient((_, _) => Task.FromResult(HtmlResponse(
			"<main><article><div><p>Nombre: Ada Lovelace</p><p>Matricula: 5229 - Fecha: 10/07/2025</p><p>Domicilio Legal: Calle 1</p><p>Telefono: 123</p><p>Correo: ada@example.test</p></div></article></main>")));
		var downloader = new DownloaderHttpClient(client);

		var result = await downloader.GetMatriculadosListAsync("lov");

		var matriculado = Assert.Single(result);
		Assert.Equal("Ada Lovelace", matriculado.Nombre);
		Assert.Equal(5229, matriculado.Matricula);
		Assert.Equal("10/07/2025", matriculado.Fecha);
		Assert.Equal("Calle 1", matriculado.Domicilio);
		Assert.Equal("123", matriculado.Telefono);
		Assert.Equal("ada@example.test", matriculado.Correo);
	}

	[Fact]
	public async Task GetMatriculadosListAsync_AllowsRegistrationWithoutDate()
	{
		using var client = CreateClient((_, _) => Task.FromResult(HtmlResponse(
			"<main><article><div><p>Matricula: 18</p></div></article></main>")));
		var downloader = new DownloaderHttpClient(client, _ => Task.CompletedTask);

		var matriculado = Assert.Single(await downloader.GetMatriculadosListAsync("abc"));

		Assert.Equal(18, matriculado.Matricula);
		Assert.Null(matriculado.Fecha);
	}

	[Fact]
	public async Task GetMatriculadosListAsync_ReturnsEmptyForNonSuccessStatus()
	{
		using var client = CreateClient((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)));
		var downloader = new DownloaderHttpClient(client, _ => Task.CompletedTask);

		var result = await downloader.GetMatriculadosListAsync("abc");

		Assert.Empty(result);
	}

	[Fact]
	public async Task GetMatriculadosListAsync_ReturnsEmptyWhenHtmlContainsInvalidRegistration()
	{
		using var client = CreateClient((_, _) => Task.FromResult(HtmlResponse(
			"<main><article><div><p>Matricula: inválida</p></div></article></main>")));
		var downloader = new DownloaderHttpClient(client, _ => Task.CompletedTask);

		var result = await downloader.GetMatriculadosListAsync("abc");

		Assert.Empty(result);
	}

	[Fact]
	public async Task GetMatriculadosListAsync_RetriesHttpRequestFailures()
	{
		var attempts = 0;
		var delays = new List<TimeSpan>();
		using var client = CreateClient((_, _) =>
		{
			attempts++;
			return attempts < 3
				? Task.FromException<HttpResponseMessage>(new HttpRequestException())
				: Task.FromResult(HtmlResponse("<main></main>"));
		});
		var downloader = new DownloaderHttpClient(client, delay =>
		{
			delays.Add(delay);
			return Task.CompletedTask;
		});

		var result = await downloader.GetMatriculadosListAsync("abc");

		Assert.Empty(result);
		Assert.Equal(3, attempts);
		Assert.Equal(new[] { TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4) }, delays);
	}

	[Theory]
	[InlineData("timeout")]
	[InlineData("http")]
	[InlineData("unexpected")]
	public async Task GetMatriculadosListAsync_ReturnsEmptyAfterRetryExhaustion(string failureKind)
	{
		var attempts = 0;
		var delays = new List<TimeSpan>();
		using var client = CreateClient((_, _) =>
		{
			attempts++;
			Exception exception = failureKind switch
			{
				"timeout" => new TaskCanceledException(),
				"http" => new HttpRequestException(),
				_ => new InvalidOperationException()
			};
			return Task.FromException<HttpResponseMessage>(exception);
		});
		var downloader = new DownloaderHttpClient(client, delay =>
		{
			delays.Add(delay);
			return Task.CompletedTask;
		});

		var result = await downloader.GetMatriculadosListAsync("abc");

		Assert.Empty(result);
		Assert.Equal(3, attempts);
		Assert.Equal(new[] { TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4) }, delays);
	}

	[Fact]
	public async Task GetMatriculadosListAsync_RetriesUnexpectedExceptions()
	{
		var attempts = 0;
		var delays = 0;
		using var client = CreateClient((_, _) =>
		{
			attempts++;
			return attempts == 1
				? Task.FromException<HttpResponseMessage>(new InvalidOperationException("temporary"))
				: Task.FromResult(HtmlResponse("<main></main>"));
		});
		var downloader = new DownloaderHttpClient(client, _ =>
		{
			delays++;
			return Task.CompletedTask;
		});

		var result = await downloader.GetMatriculadosListAsync("abc");

		Assert.Empty(result);
		Assert.Equal(2, attempts);
		Assert.Equal(1, delays);
	}

	private static HttpClient CreateClient(
		Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> sendAsync)
	{
		return new HttpClient(new CallbackHttpMessageHandler(sendAsync))
		{
			BaseAddress = new Uri("https://example.test/")
		};
	}

	private static HttpResponseMessage HtmlResponse(string html)
	{
		return new HttpResponseMessage(HttpStatusCode.OK)
		{
			Content = new StringContent(html)
		};
	}
}