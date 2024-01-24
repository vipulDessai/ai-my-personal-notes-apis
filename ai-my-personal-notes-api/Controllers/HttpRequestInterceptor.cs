using HotChocolate.AspNetCore;
using HotChocolate.Execution;

public class HttpRequestInterceptor : DefaultHttpRequestInterceptor
{
    public override ValueTask OnCreateAsync(
        HttpContext context,
        IRequestExecutor requestExecutor,
        IQueryRequestBuilder requestBuilder,
        CancellationToken cancellationToken
    )
    {
        var user = context.User;

        return base.OnCreateAsync(context, requestExecutor, requestBuilder, cancellationToken);
    }
}
