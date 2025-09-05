using MediatR;
using Microsoft.EntityFrameworkCore;
using OmniSuite.Application.DigitalProduct.Commands;
using OmniSuite.Application.DigitalProduct.Queries;
using OmniSuite.Application.DigitalProduct.Responses;
using OmniSuite.Application.Generic.Responses;
using OmniSuite.Domain.Entities;
using OmniSuite.Domain.Enums;
using OmniSuite.Domain.Utils;
using OmniSuite.Persistence;
using System.Security.Cryptography;
using System.Text;

namespace OmniSuite.Application.DigitalProduct
{
    public class DigitalProductHandler :
        IRequestHandler<CreateDigitalProductCommand, Response<DigitalProductResponse>>,
        IRequestHandler<UpdateDigitalProductCommand, Response<DigitalProductResponse>>,
        IRequestHandler<DeleteDigitalProductCommand, Response<bool>>,
        IRequestHandler<PurchaseDigitalProductCommand, Response<PurchaseDigitalProductResponse>>,
        IRequestHandler<GetDigitalProductByIdQuery, Response<DigitalProductResponse>>,
        IRequestHandler<GetDigitalProductsQuery, Response<PaginatedResult<DigitalProductResponse>>>,
        IRequestHandler<GetUserPurchasedProductsQuery, Response<PaginatedResult<DigitalProductPurchaseResponse>>>,
        IRequestHandler<GetDigitalProductsByCategoryQuery, Response<PaginatedResult<DigitalProductResponse>>>
    {
        private readonly ApplicationDbContext _context;

        public DigitalProductHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<DigitalProductResponse>> Handle(CreateDigitalProductCommand request, CancellationToken cancellationToken)
        {
            var userId = UserClaimsHelper.GetUserId();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
                return Response<DigitalProductResponse>.Fail("Usuário não encontrado");

            var digitalProduct = new Domain.Entities.DigitalProduct
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                ShortDescription = request.ShortDescription,
                Price = request.Price,
                ImageUrl = request.ImageUrl,
                ThumbnailUrl = request.ThumbnailUrl,
                Type = request.Type,
                Status = DigitalProductStatusEnum.Active,
                DownloadUrl = request.DownloadUrl,
                AccessInstructions = request.AccessInstructions,
                DownloadLimit = request.DownloadLimit,
                ExpirationDate = request.ExpirationDate,
                IsFeatured = request.IsFeatured,
                IsDigitalDelivery = request.IsDigitalDelivery,
                Category = request.Category,
                Tags = request.Tags,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.DigitalProducts.Add(digitalProduct);
            await _context.SaveChangesAsync(cancellationToken);

            var response = MapToResponse(digitalProduct, user.Name);
            return Response<DigitalProductResponse>.Ok(response);
        }

        public async Task<Response<DigitalProductResponse>> Handle(UpdateDigitalProductCommand request, CancellationToken cancellationToken)
        {
            var userId = UserClaimsHelper.GetUserId();
            var digitalProduct = await _context.DigitalProducts
                .Include(dp => dp.CreatedByUser)
                .FirstOrDefaultAsync(dp => dp.Id == request.Id, cancellationToken);

            if (digitalProduct == null)
                return Response<DigitalProductResponse>.Fail("Produto digital não encontrado");

            // Verificar se o usuário é o criador do produto ou admin
            if (digitalProduct.CreatedBy != userId)
                return Response<DigitalProductResponse>.Fail("Você não tem permissão para editar este produto");

            digitalProduct.Name = request.Name;
            digitalProduct.Description = request.Description;
            digitalProduct.ShortDescription = request.ShortDescription;
            digitalProduct.Price = request.Price;
            digitalProduct.ImageUrl = request.ImageUrl;
            digitalProduct.ThumbnailUrl = request.ThumbnailUrl;
            digitalProduct.Type = request.Type;
            digitalProduct.Status = request.Status;
            digitalProduct.DownloadUrl = request.DownloadUrl;
            digitalProduct.AccessInstructions = request.AccessInstructions;
            digitalProduct.DownloadLimit = request.DownloadLimit;
            digitalProduct.ExpirationDate = request.ExpirationDate;
            digitalProduct.IsFeatured = request.IsFeatured;
            digitalProduct.IsDigitalDelivery = request.IsDigitalDelivery;
            digitalProduct.Category = request.Category;
            digitalProduct.Tags = request.Tags;
            digitalProduct.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            var response = MapToResponse(digitalProduct, digitalProduct.CreatedByUser?.Name);
            return Response<DigitalProductResponse>.Ok(response);
        }

        public async Task<Response<bool>> Handle(DeleteDigitalProductCommand request, CancellationToken cancellationToken)
        {
            var userId = UserClaimsHelper.GetUserId();
            var digitalProduct = await _context.DigitalProducts
                .FirstOrDefaultAsync(dp => dp.Id == request.Id, cancellationToken);

            if (digitalProduct == null)
                return Response<bool>.Fail("Produto digital não encontrado");

            // Verificar se o usuário é o criador do produto ou admin
            if (digitalProduct.CreatedBy != userId)
                return Response<bool>.Fail("Você não tem permissão para excluir este produto");

            // Verificar se há compras associadas
            var hasPurchases = await _context.DigitalProductPurchases
                .AnyAsync(dpp => dpp.DigitalProductId == request.Id, cancellationToken);

            if (hasPurchases)
                return Response<bool>.Fail("Não é possível excluir produto com compras associadas");

            _context.DigitalProducts.Remove(digitalProduct);
            await _context.SaveChangesAsync(cancellationToken);

            return Response<bool>.Ok(true);
        }

        public async Task<Response<PurchaseDigitalProductResponse>> Handle(PurchaseDigitalProductCommand request, CancellationToken cancellationToken)
        {
            var userId = UserClaimsHelper.GetUserId();
            var user = await _context.Users
                .Include(u => u.UserBalance)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
                return Response<PurchaseDigitalProductResponse>.Fail("Usuário não encontrado");

            var digitalProduct = await _context.DigitalProducts
                .FirstOrDefaultAsync(dp => dp.Id == request.DigitalProductId, cancellationToken);

            if (digitalProduct == null)
                return Response<PurchaseDigitalProductResponse>.Fail("Produto digital não encontrado");

            if (digitalProduct.Status != DigitalProductStatusEnum.Active)
                return Response<PurchaseDigitalProductResponse>.Fail("Produto não está disponível para compra");

            if (digitalProduct.ExpirationDate.HasValue && digitalProduct.ExpirationDate < DateTime.UtcNow)
                return Response<PurchaseDigitalProductResponse>.Fail("Produto expirado");

            // Verificar se já foi comprado
            var existingPurchase = await _context.DigitalProductPurchases
                .FirstOrDefaultAsync(dpp => dpp.UserId == userId && dpp.DigitalProductId == request.DigitalProductId, cancellationToken);

            if (existingPurchase != null)
                return Response<PurchaseDigitalProductResponse>.Fail("Você já possui este produto");

            // Verificar saldo do usuário
            if (user.UserBalance?.TotalAmount < (long)(digitalProduct.Price * 100)) // Convertendo para centavos
                return Response<PurchaseDigitalProductResponse>.Fail("Saldo insuficiente");

            // Criar token de download
            var downloadToken = GenerateDownloadToken();
            var tokenExpiresAt = DateTime.UtcNow.AddDays(30); // Token válido por 30 dias

            var purchase = new DigitalProductPurchase
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                DigitalProductId = request.DigitalProductId,
                Amount = digitalProduct.Price,
                PurchaseDate = DateTime.UtcNow,
                Status = DigitalProductPurchaseStatusEnum.Paid,
                DownloadToken = downloadToken,
                DownloadTokenExpiresAt = tokenExpiresAt,
                DownloadCount = 0
            };

            _context.DigitalProductPurchases.Add(purchase);

            // Atualizar saldo do usuário
            if (user.UserBalance != null)
            {
                user.UserBalance.TotalAmount -= (long)(digitalProduct.Price * 100);
                user.UserBalance.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);

            var response = new PurchaseDigitalProductResponse
            {
                PurchaseId = purchase.Id,
                DigitalProductId = digitalProduct.Id,
                DigitalProductName = digitalProduct.Name,
                Amount = digitalProduct.Price,
                Status = DigitalProductPurchaseStatusEnum.Paid,
                DownloadToken = downloadToken,
                DownloadTokenExpiresAt = tokenExpiresAt,
                DownloadUrl = digitalProduct.DownloadUrl,
                AccessInstructions = digitalProduct.AccessInstructions,
                PurchaseDate = purchase.PurchaseDate
            };

            return Response<PurchaseDigitalProductResponse>.Ok(response);
        }

        public async Task<Response<DigitalProductResponse>> Handle(GetDigitalProductByIdQuery request, CancellationToken cancellationToken)
        {
            var digitalProduct = await _context.DigitalProducts
                .Include(dp => dp.CreatedByUser)
                .FirstOrDefaultAsync(dp => dp.Id == request.Id, cancellationToken);

            if (digitalProduct == null)
                return Response<DigitalProductResponse>.Fail("Produto digital não encontrado");

            var response = MapToResponse(digitalProduct, digitalProduct.CreatedByUser?.Name);
            return Response<DigitalProductResponse>.Ok(response);
        }

        public async Task<Response<PaginatedResult<DigitalProductResponse>>> Handle(GetDigitalProductsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.DigitalProducts
                .Include(dp => dp.CreatedByUser)
                .AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(dp => dp.Name.Contains(request.Search) || 
                                        dp.Description.Contains(request.Search) ||
                                        (dp.Tags != null && dp.Tags.Contains(request.Search)));
            }

            if (request.Type.HasValue)
                query = query.Where(dp => dp.Type == request.Type.Value);

            if (request.Status.HasValue)
                query = query.Where(dp => dp.Status == request.Status.Value);

            if (!string.IsNullOrEmpty(request.Category))
                query = query.Where(dp => dp.Category == request.Category);

            if (request.IsFeatured.HasValue)
                query = query.Where(dp => dp.IsFeatured == request.IsFeatured.Value);

            if (request.MinPrice.HasValue)
                query = query.Where(dp => dp.Price >= request.MinPrice.Value);

            if (request.MaxPrice.HasValue)
                query = query.Where(dp => dp.Price <= request.MaxPrice.Value);

            // Aplicar paginação
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(dp => dp.IsFeatured)
                .ThenByDescending(dp => dp.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(dp => MapToResponse(dp, dp.CreatedByUser != null ? dp.CreatedByUser.Name : null))
                .ToListAsync(cancellationToken);

            var result = new PaginatedResult<DigitalProductResponse>
            {
                Items = items,
                TotalItems = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,

            };

            return Response<PaginatedResult<DigitalProductResponse>>.Ok(result);
        }

        public async Task<Response<PaginatedResult<DigitalProductPurchaseResponse>>> Handle(GetUserPurchasedProductsQuery request, CancellationToken cancellationToken)
        {
            var userId = UserClaimsHelper.GetUserId();

            var query = _context.DigitalProductPurchases
                .Include(dpp => dpp.DigitalProduct)
                .Where(dpp => dpp.UserId == userId);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(dpp => dpp.PurchaseDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(dpp => new DigitalProductPurchaseResponse
                {
                    Id = dpp.Id,
                    UserId = dpp.UserId,
                    DigitalProductId = dpp.DigitalProductId,
                    DigitalProductName = dpp.DigitalProduct.Name,
                    Amount = dpp.Amount,
                    PurchaseDate = dpp.PurchaseDate,
                    Status = dpp.Status,
                    DownloadToken = dpp.DownloadToken,
                    DownloadTokenExpiresAt = dpp.DownloadTokenExpiresAt,
                    DownloadCount = dpp.DownloadCount,
                    DownloadUrl = dpp.DigitalProduct.DownloadUrl,
                    AccessInstructions = dpp.DigitalProduct.AccessInstructions
                })
                .ToListAsync(cancellationToken);

            var result = new PaginatedResult<DigitalProductPurchaseResponse>
            {
                Items = items,
                TotalItems = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,

            };

            return Response<PaginatedResult<DigitalProductPurchaseResponse>>.Ok(result);
        }

        public async Task<Response<PaginatedResult<DigitalProductResponse>>> Handle(GetDigitalProductsByCategoryQuery request, CancellationToken cancellationToken)
        {
            var query = _context.DigitalProducts
                .Include(dp => dp.CreatedByUser)
                .Where(dp => dp.Category == request.Category && dp.Status == DigitalProductStatusEnum.Active);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(dp => dp.IsFeatured)
                .ThenByDescending(dp => dp.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(dp => MapToResponse(dp, dp.CreatedByUser != null ? dp.CreatedByUser.Name : null))
                .ToListAsync(cancellationToken);

            var result = new PaginatedResult<DigitalProductResponse>
            {
                Items = items,
                TotalItems = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,

            };

            return Response<PaginatedResult<DigitalProductResponse>>.Ok(result);
        }

        private static DigitalProductResponse MapToResponse(Domain.Entities.DigitalProduct digitalProduct, string? createdByName)
        {
            return new DigitalProductResponse
            {
                Id = digitalProduct.Id,
                Name = digitalProduct.Name,
                Description = digitalProduct.Description,
                ShortDescription = digitalProduct.ShortDescription,
                Price = digitalProduct.Price,
                ImageUrl = digitalProduct.ImageUrl,
                ThumbnailUrl = digitalProduct.ThumbnailUrl,
                Type = digitalProduct.Type,
                Status = digitalProduct.Status,
                DownloadUrl = digitalProduct.DownloadUrl,
                AccessInstructions = digitalProduct.AccessInstructions,
                DownloadLimit = digitalProduct.DownloadLimit,
                ExpirationDate = digitalProduct.ExpirationDate,
                IsFeatured = digitalProduct.IsFeatured,
                IsDigitalDelivery = digitalProduct.IsDigitalDelivery,
                Category = digitalProduct.Category,
                Tags = digitalProduct.Tags,
                CreatedAt = digitalProduct.CreatedAt,
                UpdatedAt = digitalProduct.UpdatedAt,
                CreatedBy = digitalProduct.CreatedBy,
                CreatedByName = createdByName
            };
        }

        private static string GenerateDownloadToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
