using Saldoa.Application.Categories.Abstractions;
using Saldoa.Application.Categories.Common;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Common.Results;
using Saldoa.Application.Transactions.Abstractions;
using Saldoa.Application.Transactions.Common;
using Saldoa.Domain.Enums;

namespace Saldoa.Application.Transactions.Update;

public class UpdateTransactionUseCase
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unit;
    private readonly TransactionBudgetAnalyzer _transactionBudgetAnalyzer;

    public UpdateTransactionUseCase(
        ITransactionRepository transactionRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unit,
        TransactionBudgetAnalyzer transactionBudgetAnalyzer)
    {
        _transactionRepository = transactionRepository;
        _categoryRepository = categoryRepository;
        _unit = unit;
        _transactionBudgetAnalyzer = transactionBudgetAnalyzer;
    }

    public async Task<Result<UpdateTransactionResponse>> ExecuteAsync(
        long id,
        UpdateTransactionRequest request,
        string userId,
        CancellationToken ct)
    {
        var transaction = await _transactionRepository.GetByIdForUpdateAsync(id, userId, ct);
        if (transaction is null)
        {
            var error = TransactionErrors.NotFound;
            return Result<UpdateTransactionResponse>.Failure(error);
        }

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, userId, ct);
        if (category is null)
        {
            var error = CategoryErrors.NotFound;
            return Result<UpdateTransactionResponse>.Failure(error);
        }

        List<BudgetAlert> budgetAlerts = [];

        if (transaction.Type == TransactionType.Income)
        {
            transaction.SetTitle(request.Title);
            transaction.SetDescription(request.Description);
            transaction.SetCategoryId(request.CategoryId);
            transaction.SetAmount(request.Amount);
            transaction.SetPaidOrReceivedAt(request.PaidOrReceivedAt);
        }
        else if (transaction.InstallmentInfo.IsInstallment &&
            transaction.InstallmentInfo.InstallmentGroupId is not null)
        {
            var updateScope = request.UpdateScope ?? TransactionUpdateScope.Single;

            var affectedTransactions = updateScope == TransactionUpdateScope.All
                ? await _transactionRepository.GetInstallmentsForUpdateAsync(
                    transaction.InstallmentInfo.InstallmentGroupId.Value,
                    userId,
                    ct)
                : [transaction];

            if (affectedTransactions is null || affectedTransactions.Count == 0)
            {
                var error = TransactionErrors.InstallmentNotFound;
                return Result<UpdateTransactionResponse>.Failure(error);
            }

            foreach (var affectedTransaction in affectedTransactions)
            {
                affectedTransaction.SetTitle(request.Title);
                affectedTransaction.SetDescription(request.Description);
                affectedTransaction.SetCategoryId(request.CategoryId);
                affectedTransaction.SetAmount(request.Amount);
            }

            var installmentDrafts = affectedTransactions
                .Select(t => new InstallmentDraft(request.Amount, t.PaidOrReceivedAt, t.InstallmentInfo))
                .ToList();

            var affectedTransactionIds = affectedTransactions
                .Select(t => t.Id)
                .ToList();

            budgetAlerts = await _transactionBudgetAnalyzer.AnalyzeAsync(
                userId,
                request.CategoryId,
                installmentDrafts,
                affectedTransactionIds,
                ct
            );
        }
        else
        {
            transaction.SetTitle(request.Title);
            transaction.SetDescription(request.Description);
            transaction.SetCategoryId(request.CategoryId);
            transaction.SetAmount(request.Amount);
            transaction.SetPaidOrReceivedAt(request.PaidOrReceivedAt);

            var installmentDrafts = new List<InstallmentDraft>
            {
                new(request.Amount, request.PaidOrReceivedAt, transaction.InstallmentInfo)
            };

            budgetAlerts = await _transactionBudgetAnalyzer.AnalyzeAsync(
                userId,
                request.CategoryId,
                installmentDrafts,
                [transaction.Id],
                ct
            );
        }

        await _unit.SaveChangesAsync(ct);

        return Result<UpdateTransactionResponse>.Success(
            new UpdateTransactionResponse(budgetAlerts.Count == 0 ? null : budgetAlerts));
    }
}
