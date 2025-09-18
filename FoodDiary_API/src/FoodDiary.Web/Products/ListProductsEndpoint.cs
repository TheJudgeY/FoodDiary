using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using FoodDiary.UseCases.Products;
using FoodDiary.Core.ProductAggregate;

namespace FoodDiary.Web.Products;

[Authorize]
public class ListProductsEndpoint : EndpointWithoutRequest<ListProductsResponse>
{
    private readonly IMediator _mediator;

    public ListProductsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/products");
        Summary(s =>
        {
            s.Summary = "List products";
            s.Description = "Retrieves a paginated list of products with optional filtering by category and search term.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var page = Query<int?>("page", isRequired: false) ?? 1;
        var pageSize = Query<int?>("pageSize", isRequired: false) ?? 20;
        var search = Query<string>("search", isRequired: false);
        var category = Query<string>("category", isRequired: false);

        var command = new ListProductsCommand
        {
            Page = page,
            PageSize = pageSize,
            SearchTerm = search?.Trim(),
            Category = !string.IsNullOrEmpty(category) ? Enum.Parse<ProductCategory>(category) : null
        };

        var result = await _mediator.Send(command, ct);

        if (result.IsSuccess)
        {
            await SendAsync(result.Value, 200, ct);
        }
        else
        {
            await SendErrorsAsync(500, ct);
        }
    }
}

 