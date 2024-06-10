
using Microsoft.AspNetCore.Http.HttpResults;

namespace Web.Features.GetTrackedFood;

public class GetTrackedFoodEndpoint : IEndpoint
{
    public static readonly string RouteName = "tracker";
    public void Configure(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/", Handle)
            .WithName(RouteName);

    }

    public static Task<IResult> Handle()
    {
        return Task.FromResult<IResult>(new RazorComponentResult<GetTrackedFoodView>());
    }
}
