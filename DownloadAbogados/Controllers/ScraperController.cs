using DownloadAbogados.UseCases;

namespace DownloadAbogados.Controllers;

public class ScraperController(IParser parser)
{
	private readonly IParser _parser = parser;

	public async Task StartScraping()
	{
		await _parser.ExecuteAsync();
	}
}
