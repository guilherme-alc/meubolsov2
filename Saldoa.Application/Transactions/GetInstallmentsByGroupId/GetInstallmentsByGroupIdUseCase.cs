using Saldoa.Application.Common.Pagination;
using Saldoa.Application.Common.Results;
using Saldoa.Application.Transactions.Abstractions;
using Saldoa.Application.Transactions.Common;

namespace Saldoa.Application.Transactions.GetInstallmentsByGroupId
{
    public class GetInstallmentsByGroupIdUseCase
    {
        private readonly ITransactionRepository _transactionRepository;
        public GetInstallmentsByGroupIdUseCase(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<Result<GetInstallmentsByGroupIdResponse>> ExecuteAsync(string userId, Guid installmentGroupId, GetInstallmentsByGroupIdRequest request, CancellationToken ct)
        {
            var headerResult = await _transactionRepository.GetInstallmentGroupHeaderAsync(installmentGroupId, userId, ct);

            if (headerResult is null)
            {
                return Result<GetInstallmentsByGroupIdResponse>
                    .Failure(TransactionErrors.InstallmentNotFound);
            }

            var data = await _transactionRepository.GetInstallmentsByGroupIdAsync(
                installmentGroupId, 
                userId, 
                request.PageNumber, 
                request.PageSize, 
                ct
            );

            var installments = data.Items
                .Select(i => new InstallmentResponse(
                    i.Id, 
                    i.InstallmentInfo.InstallmentNumber, 
                    i.PaidOrReceivedAt, 
                    i.Amount))
                .ToList();

            var pagedResult = new PagedResult<InstallmentResponse>(installments, data.TotalCount, data.PageNumber, data.PageSize);

            var result = new GetInstallmentsByGroupIdResponse(
                headerResult.InstallmentGroupId,
                headerResult.Title,
                headerResult.Description,
                headerResult.Type,
                headerResult.TotalAmount,
                headerResult.TotalInstallments,
                headerResult.Category,
                pagedResult
            );

            return Result<GetInstallmentsByGroupIdResponse>.Success(result);
        }
    }
}
