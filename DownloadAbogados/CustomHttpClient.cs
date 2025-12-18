internal static class CustomHttpClient
{
	 // Create a single, static HttpClient instance to be reused throughout the application's lifetime
		internal static readonly HttpClient SharedClient = new()
		{
				//BaseAddress = new Uri("https://jsonplaceholder.typicode.com/"),
				BaseAddress = new Uri("https://www.colegioabogadossde.org.ar/index.php?id=matriculados"),
		};

}