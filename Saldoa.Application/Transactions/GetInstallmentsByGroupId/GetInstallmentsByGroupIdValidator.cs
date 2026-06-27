using FluentValidation;
using Saldoa.Application.Common.Pagination;

namespace Saldoa.Application.Transactions.GetInstallmentsByGroupId
{
    public class GetInstallmentsByGroupIdValidator : AbstractValidator<GetInstallmentsByGroupIdRequest>
    {
        public GetInstallmentsByGroupIdValidator()
        {
            this.AddPaginationRules();
        }
    }
}
