using System.Security.Claims;
using HotChocolate.AspNetCore;
using HotChocolate.Execution;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

public class HttpRequestInterceptor : DefaultHttpRequestInterceptor
{
    private readonly IPolicyEvaluator _policyEvaluator;
    private readonly IAuthorizationPolicyProvider _policyProvider;

    public HttpRequestInterceptor(
        IPolicyEvaluator policyEvaluator,
        IAuthorizationPolicyProvider policyProvider
    )
    {
        _policyEvaluator = policyEvaluator;
        _policyProvider = policyProvider;
    }

    public override async ValueTask OnCreateAsync(
        HttpContext context,
        IRequestExecutor requestExecutor,
        IQueryRequestBuilder requestBuilder,
        CancellationToken cancellationToken
    )
    {
        if (context.User.Identity.IsAuthenticated)
        {
            requestBuilder.SetGlobalState(
                "currentUser",
                new CurrentUser(
                    Guid.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)),
                    context.User.Claims.Select(x => $"{x.Type} : {x.Value}").ToList()
                )
            );
        }

        await _policyEvaluator.AuthenticateAsync(
            await _policyProvider.GetDefaultPolicyAsync(),
            context
        );
        await base.OnCreateAsync(context, requestExecutor, requestBuilder, cancellationToken);
    }
}
