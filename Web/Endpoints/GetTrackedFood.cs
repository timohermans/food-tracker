
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

    public static async Task<IResult> Handle() {

        return new RazorComponentResult<Tracker>();
    }
}
