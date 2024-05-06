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
    }
}
