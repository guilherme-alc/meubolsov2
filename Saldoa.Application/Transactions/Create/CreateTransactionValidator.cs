using FluentValidation;
using Saldoa.Domain.Enums;

namespace Saldoa.Application.Transactions.Create;

public class CreateTransactionValidator : AbstractValidator<CreateTransactionRequest>
{
    public CreateTransactionValidator()
    {
        RuleFor(x => x.Title)
            .Must(n => !string.IsNullOrWhiteSpace(n))
            .WithMessage("O título da transação é obrigatório")
            .MaximumLength(100)
            .WithMessage("O título não pode ultrapassar 100 caracteres");
        
        RuleFor(x => x.Description)
            .MaximumLength(255)
            .WithMessage("A descrição não pode ultrapassar 255 caracteres");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Tipo da transação inválido.");

        RuleFor(x => x.TotalAmount)
            .GreaterThan(0)
            .WithMessage("O valor da transação deve ser maior que 0");
        
        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("O id da categoria é obrigatório");
        
        var min = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-3));
        var max = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(2));

        RuleFor(x => x.PaidOrReceivedAt)
            .InclusiveBetween(min, max)
            .WithMessage($"A data deve estar entre {min:dd/MM/yyyy} e {max:dd/MM/yyyy}.");

        When(x => x.TotalInstallments > 1, () =>
        {
            RuleFor(x => x.Type)
                .Equal(TransactionType.Expense)
                .WithMessage("Parcelamentos só são permitidos para transações do tipo despesa");

            RuleFor(x => x.TotalInstallments)
                .LessThanOrEqualTo(120)
                .WithMessage("O número máximo de parcelas permitido é 120");
        });
    }
}