using FastFoodProduction.Repositories;
using FastFoodProduction.SqsQueues;
using FastFoodProduction.UseCases;

namespace FastFoodProduction.Endpoints;

public static class ProductionEndpoints
{
    public static void RegistryProductionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/GetNextOrder", async (SqsLogger logger, SqsProduction sqsProduction, ProductionRepository productionRepository) =>
        {
            var getNextOrder = new GetNextOrderUseCase();
            return await getNextOrder.GetNextOrder(logger, sqsProduction, productionRepository);
        });

        endpoints.MapGet("/GetAllPendingOrders", async (SqsLogger logger, ProductionRepository productionRepository) =>
        {
            var getAllPendingOrdersUseCase = new GetAllPendingOrdersUseCase();
            return await getAllPendingOrdersUseCase.GetAllPendingOrders(logger, productionRepository);
        });

        endpoints.MapPatch("/ChangeStatus/{in_store_order_id}/{newStatus}", async (string in_store_order_id, string newStatus, SqsLogger logger, ProductionRepository productionRepository) =>
        {
            var changeStatusUseCase = new ChangeStatusUseCase();
            return await changeStatusUseCase.ChangeStatus(in_store_order_id, logger, productionRepository, newStatus);
        });

        endpoints.MapPatch("/CancelOrderProduction/{in_store_order_id}", async (string in_store_order_id, SqsLogger logger, ProductionRepository productionRepository, HttpContext httpContext) =>
        {
            var accessToken = httpContext.GetAuthorizationToken();
            var changeStatusUseCase = new CancelOrderUseCase();
            return await changeStatusUseCase.CancelOrder(in_store_order_id, logger, productionRepository, accessToken);
        });
    }

    public static string GetAuthorizationToken(this HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("Authorization", out var token))
            return token.ToString().Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase).Trim();

        return null;
    }
}
