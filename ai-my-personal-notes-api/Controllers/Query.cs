using HotChocolate.Authorization;

public class Query
{
    public string Unauthorized()
    {
        return "unauthorized";
    }

    [Authorize(Policy = "hr")]
    public List<string> Authorized([Service] IHttpContextAccessor contextAccessor)
    {
        var res = contextAccessor
            .HttpContext.User.Claims.Select(x => $"{x.Type} : {x.Value}")
            .ToList();

        return res;
    }

    [Authorize(Policy = "hr")]
    public List<string> AuthorizedBetterWay([GlobalState("currentUser")] CurrentUser user)
    {
        return user.Claims;
    }

    [Authorize(Roles = new[] { "leader" })]
    public List<string> AuthorizedLeader([GlobalState("currentUser")] CurrentUser user)
    {
        return user.Claims;
    }

    [Authorize(Roles = new[] { "dev" })]
    public List<string> AuthorizedDev([GlobalState("currentUser")] CurrentUser user)
    {
        return user.Claims;
    }

    [Authorize(Policy = "DevDepartment")]
    public List<string> AuthorizedDevDepartment([GlobalState("currentUser")] CurrentUser user)
    {
        return user.Claims;
    }

    public Task<List<Book>> GetBooks([Service] Repository repository) => repository.GetBooksAsync();

    public Task<Author?> GetAuthor(GetAuthorInput input, [Service] Repository r) =>
        r.GetAuthor(input.authorId);
}
