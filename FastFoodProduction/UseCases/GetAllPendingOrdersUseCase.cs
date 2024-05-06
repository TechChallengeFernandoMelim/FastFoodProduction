using FastFoodProduction.Repositories;
using FastFoodProduction.SqsQueues;

namespace FastFoodProduction.UseCases;

public class GetAllPendingOrdersUseCase()
{
    public async Task<IResult> GetAllPendingOrders(SqsLogger logger, ProductionRepository productionRepository)
    {
        try
        {
            var orders = await productionRepository.GetPendingOrders();

            return Results.Ok(orders);
        }
        catch (Exception ex)
        {
            await logger.Log(ex.StackTrace, ex.Message, ex.ToString());
            return Results.BadRequest();
        }
    }
}
