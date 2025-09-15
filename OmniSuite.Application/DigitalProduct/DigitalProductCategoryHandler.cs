using MediatR;
using Microsoft.EntityFrameworkCore;
using OmniSuite.Application.DigitalProduct.Commands;
using OmniSuite.Application.DigitalProduct.Queries;
using OmniSuite.Application.DigitalProduct.Responses;
using OmniSuite.Application.Generic.Responses;
using OmniSuite.Domain.Entities;
using OmniSuite.Domain.Utils;
using OmniSuite.Persistence;

namespace OmniSuite.Application.DigitalProduct
{
    public class DigitalProductCategoryHandler :
        IRequestHandler<CreateDigitalProductCategoryCommand, Response<DigitalProductCategoryResponse>>,
        IRequestHandler<UpdateDigitalProductCategoryCommand, Response<DigitalProductCategoryResponse>>,
        IRequestHandler<DeleteDigitalProductCategoryCommand, Response<bool>>,
        IRequestHandler<GetDigitalProductCategoryByIdQuery, Response<DigitalProductCategoryResponse>>,
        IRequestHandler<GetDigitalProductCategoriesQuery, Response<PaginatedResult<DigitalProductCategoryResponse>>>
    {
        private readonly ApplicationDbContext _context;

        public DigitalProductCategoryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<DigitalProductCategoryResponse>> Handle(CreateDigitalProductCategoryCommand request, CancellationToken cancellationToken)
        {
            var userId = UserClaimsHelper.GetUserId();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
                return Response<DigitalProductCategoryResponse>.Fail("Usuário não encontrado");

            // Verificar se já existe uma categoria com o mesmo nome
            var existingCategory = await _context.DigitalProductCategories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == request.Name.ToLower(), cancellationToken);

            if (existingCategory != null)
                return Response<DigitalProductCategoryResponse>.Fail("Já existe uma categoria com este nome");

            var category = new DigitalProductCategory
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                IconUrl = request.IconUrl,
                Color = request.Color,
                IsActive = true,
                SortOrder = request.SortOrder,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.DigitalProductCategories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);

            var response = MapToResponse(category, user.Name);
            return Response<DigitalProductCategoryResponse>.Ok(response);
        }

        public async Task<Response<DigitalProductCategoryResponse>> Handle(UpdateDigitalProductCategoryCommand request, CancellationToken cancellationToken)
        {
            var userId = UserClaimsHelper.GetUserId();
            var category = await _context.DigitalProductCategories
                .Include(c => c.CreatedByUser)
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (category == null)
                return Response<DigitalProductCategoryResponse>.Fail("Categoria não encontrada");

            // Verificar se o usuário é o criador da categoria ou admin
            if (category.CreatedBy != userId)
                return Response<DigitalProductCategoryResponse>.Fail("Você não tem permissão para editar esta categoria");

            // Verificar se já existe outra categoria com o mesmo nome
            var existingCategory = await _context.DigitalProductCategories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == request.Name.ToLower() && c.Id != request.Id, cancellationToken);

            if (existingCategory != null)
                return Response<DigitalProductCategoryResponse>.Fail("Já existe uma categoria com este nome");

            category.Name = request.Name;
            category.Description = request.Description;
            category.IconUrl = request.IconUrl;
            category.Color = request.Color;
            category.IsActive = request.IsActive;
            category.SortOrder = request.SortOrder;
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            var response = MapToResponse(category, category.CreatedByUser?.Name);
            return Response<DigitalProductCategoryResponse>.Ok(response);
        }

        public async Task<Response<bool>> Handle(DeleteDigitalProductCategoryCommand request, CancellationToken cancellationToken)
        {
            var userId = UserClaimsHelper.GetUserId();
            var category = await _context.DigitalProductCategories
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (category == null)
                return Response<bool>.Fail("Categoria não encontrada");

            // Verificar se o usuário é o criador da categoria ou admin
            if (category.CreatedBy != userId)
                return Response<bool>.Fail("Você não tem permissão para excluir esta categoria");

            // Verificar se há produtos associados
            var hasProducts = await _context.DigitalProducts
                .AnyAsync(dp => dp.CategoryId == request.Id, cancellationToken);

            if (hasProducts)
                return Response<bool>.Fail("Não é possível excluir categoria com produtos associados");

            _context.DigitalProductCategories.Remove(category);
            await _context.SaveChangesAsync(cancellationToken);

            return Response<bool>.Ok(true);
        }

        public async Task<Response<DigitalProductCategoryResponse>> Handle(GetDigitalProductCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _context.DigitalProductCategories
                .Include(c => c.CreatedByUser)
                .Include(c => c.DigitalProducts)
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (category == null)
                return Response<DigitalProductCategoryResponse>.Fail("Categoria não encontrada");

            var response = MapToResponse(category, category.CreatedByUser?.Name);
            response.ProductCount = category.DigitalProducts.Count;
            return Response<DigitalProductCategoryResponse>.Ok(response);
        }

        public async Task<Response<PaginatedResult<DigitalProductCategoryResponse>>> Handle(GetDigitalProductCategoriesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.DigitalProductCategories
                .Include(c => c.CreatedByUser)
                .Include(c => c.DigitalProducts)
                .AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(c => c.Name.Contains(request.Search) || 
                                       (c.Description != null && c.Description.Contains(request.Search)));
            }

            if (request.IsActive.HasValue)
                query = query.Where(c => c.IsActive == request.IsActive.Value);

            // Aplicar paginação
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(c => c.SortOrder)
                .ThenBy(c => c.Name)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new DigitalProductCategoryResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    IconUrl = c.IconUrl,
                    Color = c.Color,
                    IsActive = c.IsActive,
                    SortOrder = c.SortOrder,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    CreatedBy = c.CreatedBy,
                    CreatedByName = c.CreatedByUser != null ? c.CreatedByUser.Name : null,
                    ProductCount = c.DigitalProducts.Count
                })
                .ToListAsync(cancellationToken);

            var result = new PaginatedResult<DigitalProductCategoryResponse>
            {
                Items = items,
                TotalItems = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
            };

            return Response<PaginatedResult<DigitalProductCategoryResponse>>.Ok(result);
        }

        private static DigitalProductCategoryResponse MapToResponse(DigitalProductCategory category, string? createdByName)
        {
            return new DigitalProductCategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IconUrl = category.IconUrl,
                Color = category.Color,
                IsActive = category.IsActive,
                SortOrder = category.SortOrder,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt,
                CreatedBy = category.CreatedBy,
                CreatedByName = createdByName,
                ProductCount = 0 // Será preenchido quando necessário
            };
        }
    }
}
