using DownloadAbogados.Controllers;
using DownloadAbogados.UseCases;
using Moq;

namespace DownloadAbogados.Tests.Controllers;

public class ScraperControllerTests
{
	[Fact]
	public async Task StartScraping_ExecutesParser()
	{
		var parser = new Mock<IParser>();
		var controller = new ScraperController(parser.Object);

		await controller.StartScraping();

		parser.Verify(item => item.ExecuteAsync(), Times.Once);
	}
}