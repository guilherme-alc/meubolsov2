using MeuBolso.Application.Categories.Abstractions;
using MeuBolso.Application.Common.Results;
using MeuBolso.Domain.Entities;

namespace MeuBolso.Application.Categories.Create;

public class CreateCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<CreateCategoryResponse>> ExecuteAsync(CreateCategoryRequest request)
    {
        if (await _categoryRepository.ExistsAsync(request.UserId, request.Name))
            return Result<CreateCategoryResponse>.Failure($"A Categoria {request.Name} já existe");;

        var category = new Category
        {
            Name = request.Name,
            Description = request.Description,
            Color = request.Color,
            UserId = request.UserId
        };
    
        await _categoryRepository.AddAsync(category);
        
        return Result<CreateCategoryResponse>.Success(
            new CreateCategoryResponse(category.Id, category.Name)
        );
    }
}