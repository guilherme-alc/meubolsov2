namespace Saldoa.API.Endpoints.Transactions;

public static class TransactionsEndpoints
{
    public static void MapTransactionsEndpoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/transactions")
            .WithTags("Transactions");

        CreateTransactionEndpoints.Map(group);
        GetTransactionByIdEndpoint.Map(group);
        UpdateTransactionEndpoint.Map(group);
        DeleteTransactionEndpoint.Map(group);
        ListTransactionsByPeriodEndpoint.Map(group);
        ListTransactionsByMonthEndpoint.Map(group);
        GetInstallmentsByGroupIdEndpoint.Map(group);
    }
}