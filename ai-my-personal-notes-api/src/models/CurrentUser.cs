﻿public class CurrentUser
{
    public Guid UserId { get; }
    public List<string> Claims { get; }

    public CurrentUser(Guid userId, List<string> claims)
    {
        UserId = userId;
        Claims = claims;
    }
}

public class CurrentUserGlobalState : GlobalStateAttribute
{
    public CurrentUserGlobalState()
        : base("currentUser") { }
}
