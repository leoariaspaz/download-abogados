using System;

namespace DownloadAbogados.Domain;

public class ScrapingResult
{
	public string? HtmlContent { get; set; }
	public bool IsSuccess { get; set; }
}
