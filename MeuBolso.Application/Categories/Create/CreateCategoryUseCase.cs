using MeuBolso.Application.Categories.Abstractions;
using MeuBolso.Application.Common.Abstractions;
using MeuBolso.Application.Common.Results;
using MeuBolso.Domain.Entities;

namespace MeuBolso.Application.Categories.Create;

public class CreateCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unit;

    public CreateCategoryUseCase(ICategoryRepository categoryRepository, IUnitOfWork unit)
    {
        _categoryRepository = categoryRepository;
        _unit = unit;
    }

    public async Task<Result<CreateCategoryResponse>> ExecuteAsync(CreateCategoryRequest request, CancellationToken ct)
    {
        if (await _categoryRepository.ExistsAsync(request.UserId, request.Name, ct))
            return Result<CreateCategoryResponse>.Failure($"A Categoria {request.Name} já existe");

        var category = new Category(request.UserId, request.Name, request.Description, request.Color);
    
        await _categoryRepository.AddAsync(category, ct);
        await _unit.SaveChangesAsync(ct);
        return Result<CreateCategoryResponse>.Success(
            new CreateCategoryResponse(category.Id, category.Name)
        );
    }
}