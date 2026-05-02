using FluentValidation;

namespace Saldoa.Application.Transactions.Update;

public class UpdateTransactionValidator : AbstractValidator<UpdateTransactionRequest>
{
    public UpdateTransactionValidator()
    {
        RuleFor(x => x.Title!)
            .Must(n => !string.IsNullOrWhiteSpace(n))
            .WithMessage("O título da transação precisa ser fornecido")
            .MaximumLength(100)
            .WithMessage("O título não pode ultrapassar 100 caracteres");

        RuleFor(x => x.Description!)
            .MaximumLength(255)
            .WithMessage("A descrição não pode ultrapassar 255 caracteres");

        RuleFor(x => x.Amount!)
            .GreaterThan(0)
            .WithMessage("O valor da transação deve ser maior que 0");

        RuleFor(x => x.CategoryId!)
            .GreaterThan(0)
            .WithMessage("O id da categoria é obrigatório");

        var min = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-3));
        var max = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(2));


        RuleFor(x => x.PaidOrReceivedAt)
            .InclusiveBetween(min, max)
            .WithMessage($"A data deve estar entre {min:dd/MM/yyyy} e {max:dd/MM/yyyy}.");
    }
}