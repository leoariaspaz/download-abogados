using HtmlAgilityPack;
using DownloadAbogados;

var connectionString = "Server=localhost;Database=rivendel;User=root;Password=admin;";
var connectionFactory = new DownloadAbogados.Data.MySqlConnectionFactory(connectionString);
var matriculadosRepository = new DownloadAbogados.Data.MatriculadosRepository(connectionFactory);
var consultaService = new DownloadAbogados.Services.QueryCriteriaGenerator(matriculadosRepository);
consultaService.ProcesarAsync(await GetMatriculadosAsync()).Wait();

