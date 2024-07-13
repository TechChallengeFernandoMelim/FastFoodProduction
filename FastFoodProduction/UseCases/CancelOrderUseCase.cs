using FastFoodProduction.Repositories;
using FastFoodProduction.SqsQueues;

namespace FastFoodProduction.UseCases;

public class CancelOrderUseCase
{
    public async Task<IResult> CancelOrder(string in_store_order_id, SqsLogger logger, ProductionRepository productionRepository, string accessToken)
    {
        try
        {
            if (await productionRepository.ChangeOrderToStatus(in_store_order_id, "Canceled"))
            {
                await CancelOrderPaymentService(accessToken, in_store_order_id);
                return Results.Ok();
            }

            return Results.BadRequest();
        }
        catch (Exception ex)
        {
            await logger.Log(ex.StackTrace, ex.Message, ex.ToString());
            return Results.BadRequest();
        }
    }

    private async Task<bool> CancelOrderPaymentService(string accessToken, string in_store_order_id)
    {
        var httpRequest = new HttpClient();

        httpRequest.BaseAddress = new Uri(Environment.GetEnvironmentVariable("PAYMENT_SERVICE_URL"));
        httpRequest.DefaultRequestHeaders.Clear();

        if(!string.IsNullOrWhiteSpace(in_store_order_id))
            httpRequest.DefaultRequestHeaders.Add("Authorization", $"{accessToken}");

        var result = await httpRequest.PatchAsync($"/ApiGatewayStage/CancelOrderPayment/{in_store_order_id}", new StringContent(in_store_order_id));

        return result.IsSuccessStatusCode;
    }
}
