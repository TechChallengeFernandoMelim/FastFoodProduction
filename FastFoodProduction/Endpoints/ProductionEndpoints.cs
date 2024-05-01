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
            catch (Exception ex)
            {
                await logger.Log(ex.StackTrace, ex.Message, ex.ToString());
                return Results.BadRequest();
            }
        });

        endpoints.MapGet("/GetAllPendingOrders", async (SqsLogger logger, ProductionRepository productionRepository) =>
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
        });

        endpoints.MapPatch("/ChangeToPreparing/{in_store_order_id}", async (string in_store_order_id, SqsLogger logger, ProductionRepository productionRepository) =>
        {
            try
            {
                var order = await productionRepository.GetOrderByPk(in_store_order_id);

                if (order is null)
                    return Results.BadRequest("Pedido não foi encontrado na base de dados.");

                if (order.OrderStatus == "Paid")
                {
                    var changed = await productionRepository.ChangeOrderToStatus(in_store_order_id, "Preparing");
                    if (!changed)
                        throw new Exception("Ocorreu algum erro ao atualizaro o status do pedido.");
                }
                else
                    return Results.BadRequest("Esse pedido não pode ser movido para o estado Preparing.");


                return Results.Ok();
            }
            catch (Exception ex)
            {
                await logger.Log(ex.StackTrace, ex.Message, ex.ToString());
                return Results.BadRequest();
            }
        });

        endpoints.MapPatch("/ChangeToReady/{in_store_order_id}", async (string in_store_order_id, SqsLogger logger, ProductionRepository productionRepository) =>
        {
            try
            {
                var order = await productionRepository.GetOrderByPk(in_store_order_id);

                if (order is null)
                    return Results.BadRequest("Pedido não foi encontrado na base de dados.");

                if (order.OrderStatus == "Preparing")
                {
                    var changed = await productionRepository.ChangeOrderToStatus(in_store_order_id, "Ready");
                    if (!changed)
                        throw new Exception("Ocorreu algum erro ao atualizaro o status do pedido.");
                }
                else
                    return Results.BadRequest("Esse pedido não pode ser movido para o estado Ready.");


                return Results.Ok();
            }
            catch (Exception ex)
            {
                await logger.Log(ex.StackTrace, ex.Message, ex.ToString());
                return Results.BadRequest();
            }
        });

        endpoints.MapPatch("/ChangeToFinished/{in_store_order_id}", async (string in_store_order_id, SqsLogger logger, ProductionRepository productionRepository) =>
        {
            try
            {
                var order = await productionRepository.GetOrderByPk(in_store_order_id);

                if (order is null)
                    return Results.BadRequest("Pedido não foi encontrado na base de dados.");

                if (order.OrderStatus == "Ready")
                {
                    var changed = await productionRepository.ChangeOrderToStatus(in_store_order_id, "Finished");
                    if (!changed)
                        throw new Exception("Ocorreu algum erro ao atualizaro o status do pedido.");
                }
                else
                    return Results.BadRequest("Esse pedido não pode ser movido para o estado Finished.");


                return Results.Ok();
            }
            catch (Exception ex)
            {
                await logger.Log(ex.StackTrace, ex.Message, ex.ToString());
                return Results.BadRequest();
            }
        });
    }
}
