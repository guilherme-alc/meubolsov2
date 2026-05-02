using Saldoa.Application.Categories.Abstractions;
using Saldoa.Application.Categories.Common;
using Saldoa.Application.Common.Results;

namespace Saldoa.Application.Categories.GetById;

public class GetCategoryByIdUseCase
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryByIdUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<CategoryResponse>> ExecuteAsync(long id, string userId, CancellationToken ct)
    {
        var category = await _categoryRepository.GetByIdAsync(id, userId, ct);
        
        if (category is null)
        {
            var error = CategoryErrors.NotFound;
            return Result<CategoryResponse>.Failure(error);
        }
        
        var response = new CategoryResponse(category.Id, category.Name, category.Description, category.Color);
        
        return Result<CategoryResponse>.Success(response);
    }
}