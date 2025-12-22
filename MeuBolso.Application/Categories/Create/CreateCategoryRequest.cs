using MeuBolso.Application.Common.Requests;

namespace MeuBolso.Application.Categories.Create;

public class CreateCategoryRequest : AuthenticatedRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
}