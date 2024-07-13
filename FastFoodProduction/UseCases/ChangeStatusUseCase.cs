using FastFoodProduction.Repositories;
using FastFoodProduction.SqsQueues;

namespace FastFoodProduction.UseCases;

public class ChangeStatusUseCase()
{
    public async Task<IResult> ChangeStatus(string in_store_order_id, SqsLogger logger, ProductionRepository productionRepository, string newStatus)
    {
        try
        {
            var order = await productionRepository.GetOrderByPk(in_store_order_id);

            if (order is null)
                return Results.BadRequest("Pedido não foi encontrado na base de dados.");

            if (ValidateNextStatus(order.OrderStatus, newStatus))
            {
                var changed = await productionRepository.ChangeOrderToStatus(in_store_order_id, newStatus);
                if (!changed)
                    throw new Exception("Ocorreu algum erro ao atualizar o status do pedido.");
            }
            else
            {
                if (order.OrderStatus == "Canceled")
                    return Results.BadRequest($"O pedido {in_store_order_id} já foi cancelado.");
                else
                    return Results.BadRequest("Status inválido para o pedido selecionado.");
            }

            return Results.Ok();
        }
        catch (Exception ex)
        {
            await logger.Log(ex.StackTrace, ex.Message, ex.ToString());
            return Results.BadRequest();
        }
    }

    private bool ValidateNextStatus(string oldStatus, string nextStatus)
    {
        if (oldStatus == "Paid" && nextStatus == "Preparing")
            return true;

        if (oldStatus == "Preparing" && nextStatus == "Ready")
            return true;

        if (oldStatus == "Ready" && nextStatus == "Finished")
            return true;

        return false;
    }
}
