internal static class CustomHttpClient
{
	internal static readonly HttpClient SharedClient = new()
	{
		BaseAddress = new Uri("https://www.colegioabogadossde.org.ar/index.php?id=matriculados"),
		Timeout = TimeSpan.FromSeconds(10)
	};

	internal static async Task<HttpResponseMessage> PostWithRetryAsync(HttpContent content)
	{
		int retries = 3;

		for (int i = 1; i <= retries; i++)
		{
			try
			{
				return await SharedClient.PostAsync("", content);
			}
			catch (TaskCanceledException) when (i < retries)
			{
				Console.WriteLine("Timeout occurred, retrying...");
				await Task.Delay(TimeSpan.FromSeconds(2 * i));
			}
			catch (HttpRequestException) when (i < retries)
			{
				Console.WriteLine("HTTP request failed, retrying...");
				await Task.Delay(TimeSpan.FromSeconds(2 * i));
			}
			catch (Exception ex) when (i < retries)
			{
				Console.WriteLine($"Unexpected error: {ex.Message}, retrying...");
				await Task.Delay(TimeSpan.FromSeconds(2 * i));
			}
		}

		throw new Exception("Falló luego de varios reintentos");
	}
}