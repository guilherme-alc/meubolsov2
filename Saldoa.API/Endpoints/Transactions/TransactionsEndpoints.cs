namespace Saldoa.API.Endpoints.Transactions;

internal static class TransactionsEndpoints
{
    internal static void MapTransactionEndpoints(this IEndpointRouteBuilder app)
    {
        var transactionsGroup = app.MapGroup("/transactions")
            .WithTags("Transactions");

        CreateTransactionEndpoints.Map(transactionsGroup);
        GetTransactionByIdEndpoint.Map(transactionsGroup);
        UpdateTransactionEndpoint.Map(transactionsGroup);
        DeleteTransactionEndpoint.Map(transactionsGroup);
        ListTransactionsByPeriodEndpoint.Map(transactionsGroup);
        GetInstallmentsByGroupIdEndpoint.Map(transactionsGroup);
    }
}