
using Microsoft.AspNetCore.Http.HttpResults;
using Web.Views;

namespace Web.Endpoints;

public class GetTrackedFood : IEndpoint
{
    public static readonly string RouteName = "tracker";
    public void Configure(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/", Handle)
            .WithName(RouteName);

    }

    public static Task<IResult> Handle()
    {
        return Task.FromResult<IResult>(new RazorComponentResult<Tracker>());
    }
}
