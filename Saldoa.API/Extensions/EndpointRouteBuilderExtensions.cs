using Saldoa.API.Endpoints.Auth;
using Saldoa.API.Endpoints.Categories;
using Saldoa.API.Endpoints.CategoryBudgets;
using Saldoa.API.Endpoints.Transactions;

namespace Saldoa.API.Extensions;

public static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// Organiza e configura endpoints da API
    /// </summary>
    public static void MapEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var api = endpoints.MapGroup("/api");
        var v1 = api.MapGroup("/v1");

        v1.MapAuthEndpoints();
        v1.MapCategoryEndpoints();
        v1.MapTransactionEndpoints();
        v1.MapCategoryBudgetEndpoints();
    }
}