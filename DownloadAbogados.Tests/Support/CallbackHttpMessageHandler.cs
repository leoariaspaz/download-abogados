namespace DownloadAbogados.Tests.Support;

internal sealed class CallbackHttpMessageHandler(
	Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> sendAsync)
	: HttpMessageHandler
{
	protected override Task<HttpResponseMessage> SendAsync(
		HttpRequestMessage request,
		CancellationToken cancellationToken)
	{
		return sendAsync(request, cancellationToken);
	}
}