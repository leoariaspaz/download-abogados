using DownloadAbogados.UseCases;

namespace DownloadAbogados.Controllers;

public class ScraperController(IParser parser)
{
	private readonly IParser _parser = parser;

	public async void StartScraping()
	{
		await _parser.ExecuteAsync();
	}
}
