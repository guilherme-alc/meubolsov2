using Saldoa.Application.Categories.Abstractions;
using Saldoa.Application.Categories.Common;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Common.Results;
using Saldoa.Domain.Entities;

namespace Saldoa.Application.Categories.Create;

public class CreateCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unit;

    public CreateCategoryUseCase(ICategoryRepository categoryRepository, IUnitOfWork unit)
    {
        _categoryRepository = categoryRepository;
        _unit = unit;
    }

    public async Task<Result<CreateCategoryResponse>> ExecuteAsync(CreateCategoryRequest request, string userId, CancellationToken ct)
    {
        if (await _categoryRepository.ExistsAsync(userId, request.Name, ct))
        {
            var error = CategoryErrors.AlreadyExists(request.Name);
            return Result<CreateCategoryResponse>.Failure(error);
        }

        var category = new Category(userId, request.Name, request.Description, request.Color);
    
        await _categoryRepository.AddAsync(category, ct);
        await _unit.SaveChangesAsync(ct);
        return Result<CreateCategoryResponse>.Success(
            new CreateCategoryResponse(category.Id, category.Name)
        );
    }
}