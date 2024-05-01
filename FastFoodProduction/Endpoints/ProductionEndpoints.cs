using FastFoodProduction.Repositories;
using FastFoodProduction.SqsQueues;

namespace FastFoodProduction.Endpoints;

public static class ProductionEndpoints
{
    public static void RegistryProductionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/GetNextOrder", async (SqsLogger logger, SqsProduction sqsProduction, ProductionRepository productionRepository) =>
        {
            try
            {
                var order = await sqsProduction.GetNextOrder();

                if (order == null)
                    return Results.NoContent();

                await productionRepository.CreateOrder(order);

                await sqsProduction.DeleteMessage(order.ReceiptHandlerMessage);

                return Results.Ok(order);
            }
            catch(Exception ex)
            {
                await logger.Log(ex.StackTrace, ex.Message, ex.ToString());
                return Results.BadRequest();
            }
        });
    }
}
