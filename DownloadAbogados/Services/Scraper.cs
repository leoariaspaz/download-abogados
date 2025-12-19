internal static class CustomHttpClient
{
	internal static readonly HttpClient SharedClient = new()
	{
			BaseAddress = new Uri("https://www.colegioabogadossde.org.ar/index.php?id=matriculados"),
			Timeout = TimeSpan.FromSeconds(10)
	};

	internal static async Task<HttpResponseMessage> PostWithRetryAsync(string query)
	{
			int retries = 3;

			for (int i = 1; i <= retries; i++)
			{
					try
					{
							var content = new FormUrlEncodedContent(
							[
								new KeyValuePair<string, string>("b", query)
							]);
							return await SharedClient.PostAsync("", content);
					}
					catch (TaskCanceledException) when (i < retries)
					{
							await Task.Delay(TimeSpan.FromSeconds(2 * i));
					}
					catch (HttpRequestException) when (i < retries)
					{
							await Task.Delay(TimeSpan.FromSeconds(2 * i));
					}
			}

			throw new Exception("Falló luego de varios reintentos");
	}
}