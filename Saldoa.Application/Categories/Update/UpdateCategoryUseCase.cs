using Saldoa.Application.Categories.Abstractions;
using Saldoa.Application.Categories.Common;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Common.Results;

namespace Saldoa.Application.Categories.Update;

public class UpdateCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unit;

    public UpdateCategoryUseCase(ICategoryRepository categoryRepository, IUnitOfWork unit)
    {
         _categoryRepository = categoryRepository;
         _unit = unit;
    }

    public async Task<Result> ExecuteAsync(long id, UpdateCategoryRequest request, string userId, CancellationToken ct)
    {
        var category = await _categoryRepository.GetByIdForUpdateAsync(id, userId, ct);

        if (category == null)
        {
            var error = CategoryErrors.NotFound;
            return Result.Failure(error);
        }

        if (request.Name is not null)
        {
            var newNormalized = request.Name.Trim().ToUpperInvariant();
            if (newNormalized != category.NormalizedName 
                && await _categoryRepository.ExistsAsync(userId, request.Name, ct))
            {
                var error = CategoryErrors.AlreadyExists(request.Name);
                return Result.Failure(error);
            }
            
            category.SetName(request.Name);
        }

        if(request.Description is not null)
            category.SetDescription(request.Description);
        
        if(request.Color is not null)
            category.SetColor(request.Color);
        
        await _unit.SaveChangesAsync(ct);
        return Result.Success();
    }
}