using Saldoa.Application.Categories.Abstractions;
using Saldoa.Application.Categories.Common;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Common.Results;
using Saldoa.Application.Transactions.Abstractions;
using Saldoa.Application.Transactions.Common;
using Saldoa.Domain.Entities;
using Saldoa.Domain.Enums;

namespace Saldoa.Application.Transactions.Create;

public class CreateTransactionUseCase
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unit;
    private readonly TransactionBudgetAnalyzer _transactionBudgetAnalyzer;

    public CreateTransactionUseCase(ITransactionRepository transactionRepository, 
        IUnitOfWork unit, 
        ICategoryRepository categoryRepository,
        TransactionBudgetAnalyzer transactionBudgetAnalyzer)
    {
        _transactionRepository = transactionRepository;
        _unit = unit;
        _categoryRepository = categoryRepository;
        _transactionBudgetAnalyzer = transactionBudgetAnalyzer;
    }

    public async Task<Result<TransactionsResponse>> ExecuteAsync(CreateTransactionRequest request, string userId, CancellationToken ct)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, userId, ct);
        
        if(category == null)
        {
            var error = CategoryErrors.NotFound;
            return Result<TransactionsResponse>.Failure(error);
        }

        List<BudgetAlert> budgetAlerts = [];

        var installments = InstallmentPlanFactory.Create(request.TotalAmount, request.PaidOrReceivedAt, request.TotalInstallments);

        if (request.Type == TransactionType.Expense)
        {
            budgetAlerts = await _transactionBudgetAnalyzer.AnalyzeAsync(
                userId,
                request.CategoryId,
                installments,
                ct);
        }

        var transactions = installments.Select(i =>
            new Transaction(
                userId,
                request.Title,
                request.Description,
                request.Type,
                i.Amount,
                i.Date,
                category.Id,
                i.InstallmentInfo
            )
        ).ToList();

        if (transactions.Count > 1)
            await _transactionRepository.AddRangeAsync(transactions, ct);
        else
            await _transactionRepository.AddAsync(transactions[0], ct);

        await _unit.SaveChangesAsync(ct);

        var response = transactions.Select(t =>
            new TransactionResponse(
                t.Id,
                t.Title,
                t.Description,
                t.Type,
                t.Amount,
                t.PaidOrReceivedAt,
                new CategorySummaryResponse(category.Id, category.Name, category.Color),
                t.InstallmentInfo.IsInstallment ? t.InstallmentInfo : null
            )
        ).ToList();

        return Result<TransactionsResponse>.Success(new TransactionsResponse(
            response,
            budgetAlerts.Count == 0 ? null : budgetAlerts
        ));
    }
}