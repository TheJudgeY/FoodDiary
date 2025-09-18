using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using FoodDiary.UseCases.Products;

namespace FoodDiary.Web.Products;

[Authorize]
public class DeleteProductEndpoint : Endpoint<DeleteProductRequest, DeleteProductResponse>
{
    private readonly IMediator _mediator;

    public DeleteProductEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Delete("/api/products/{id}");
        Summary(s =>
        {
            s.Summary = "Delete a product";
            s.Description = "Deletes a product from the system. Returns detailed information about why deletion might fail.";
        });
    }

    public override async Task HandleAsync(DeleteProductRequest req, CancellationToken ct)
    {
        var command = new DeleteProductCommand
        {
            ProductId = req.Id
        };

        var result = await _mediator.Send(command, ct);

        if (!result.IsSuccess)
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        var response = result.Value;

        if (!response.CanDelete)
        {
            await SendAsync(response, 400, cancellation: ct);
            return;
        }

        await SendAsync(response, cancellation: ct);
    }
}

public class DeleteProductRequest
{
    public Guid Id { get; set; }
} 