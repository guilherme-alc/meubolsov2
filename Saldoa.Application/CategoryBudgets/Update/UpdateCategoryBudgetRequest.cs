namespace Saldoa.Application.CategoryBudgets.Update;

public sealed record UpdateCategoryBudgetRequest(DateOnly PeriodStart, DateOnly PeriodEnd, decimal LimitAmount);