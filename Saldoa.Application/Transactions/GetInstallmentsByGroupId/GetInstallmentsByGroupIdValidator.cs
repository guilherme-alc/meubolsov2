using FluentValidation;

namespace Saldoa.Application.Transactions.GetInstallmentsByGroupId
{
    public class GetInstallmentsByGroupIdValidator : AbstractValidator<GetInstallmentsByGroupIdRequest>
    {
        public GetInstallmentsByGroupIdValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(100);
        }
    }
}
