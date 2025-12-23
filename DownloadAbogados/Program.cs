var connectionString = File.ReadAllText("./settings.txt").Trim();
var connectionFactory = new DownloadAbogados.Data.MySqlConnectionFactory(connectionString);
var repository = new DownloadAbogados.Data.MatriculadosRepository(connectionFactory);
var criteriaGenerator = new DownloadAbogados.Services.QueryCriteriaGenerator();
var provider = new DownloadAbogados.Services.DownloaderHttpClient();
var useCase = new DownloadAbogados.UseCases.MatriculadosParser(criteriaGenerator, provider, repository);
var controller = new DownloadAbogados.Controllers.ScraperController(useCase);
await controller.StartScraping();