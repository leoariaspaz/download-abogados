using HtmlAgilityPack;
using DownloadAbogados;

var content = new FormUrlEncodedContent(new[]
{
		new KeyValuePair<string, string>("b", "diaz"),
});
HttpResponseMessage response = await CustomHttpClient.SharedClient.PostAsync("", content);

if (response.IsSuccessStatusCode)
{
		string responseBody = await response.Content.ReadAsStringAsync();
		//Console.WriteLine(responseBody);
		//File.WriteAllText("./response.html", responseBody);

		var htmlDoc = new HtmlDocument();
		htmlDoc.LoadHtml(responseBody);

		//var xpath = "/html/body/main/section[2]/div/div/div[1]/article/div[1]";
		var xpath = "//main//article//div[1]";
		var node = htmlDoc.DocumentNode.SelectSingleNode(xpath);
		while (node != null)
		{
			// Console.WriteLine("Node found:");
			// Console.WriteLine(node.InnerHtml);

			Console.WriteLine(new ParserService().Execute(node.SelectNodes(".//p")));
			node = node.NextSibling;
		}		
}
else
{
		Console.WriteLine($"Error: {response.StatusCode}");
}