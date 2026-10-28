internal sealed class CustomHttpClient
{
	internal static readonly HttpClient SharedClient = new()
	{
		BaseAddress = new Uri("https://www.colegioabogadossde.org.ar/index.php?id=matriculados"),
		Timeout = TimeSpan.FromSeconds(10)
	};

	private readonly HttpClient _client;
	private readonly Func<TimeSpan, Task> _delayAsync;

	internal CustomHttpClient(HttpClient client, Func<TimeSpan, Task>? delayAsync = null)
	{
		_client = client;
		_delayAsync = delayAsync ?? Task.Delay;
	}

	internal async Task<HttpResponseMessage> PostWithRetryAsync(HttpContent content)
	{
		int retries = 3;

		for (int i = 1; i <= retries; i++)
		{
			try
			{
				return await _client.PostAsync("", content);
			}
			catch (TaskCanceledException)
			{
				if (i == retries) break;
				Console.WriteLine("Timeout occurred, retrying...");
				await _delayAsync(TimeSpan.FromSeconds(2 * i));
			}
			catch (HttpRequestException)
			{
				if (i == retries) break;
				Console.WriteLine("HTTP request failed, retrying...");
				await _delayAsync(TimeSpan.FromSeconds(2 * i));
			}
			catch (Exception ex)
			{
				if (i == retries) break;
				Console.WriteLine($"Unexpected error: {ex.Message}, retrying...");
				await _delayAsync(TimeSpan.FromSeconds(2 * i));
			}
		}

		throw new Exception("Falló luego de varios reintentos");
	}
}