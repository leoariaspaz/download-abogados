using DownloadAbogados.Services;

namespace DownloadAbogados.Tests.Services;

public class QueryCriteriaGeneratorTests
{
	[Fact]
	public void GetNextQueryParameter_GeneratesEveryCombinationAndThenStops()
	{
		var generator = new QueryCriteriaGenerator();
		string? first = null;
		string? last = null;

		for (var index = 0; index < 26 * 26 * 26; index++)
		{
			var parameter = generator.GetNextQueryParameter();
			first ??= parameter;
			last = parameter;
		}

		Assert.Equal("aaa", first);
		Assert.Equal("zzz", last);
		Assert.Null(generator.GetNextQueryParameter());
		Assert.Null(generator.GetNextQueryParameter());
	}

	[Fact]
	public void GetNextLetter_RejectsInvalidInput()
	{
		Assert.Throws<ArgumentException>(() => QueryCriteriaGenerator.GetNextLetter("A"));
		Assert.Throws<ArgumentException>(() => QueryCriteriaGenerator.GetNextLetter("ab"));
	}
}