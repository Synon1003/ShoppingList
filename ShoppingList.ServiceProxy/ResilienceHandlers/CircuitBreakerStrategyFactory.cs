using ShoppingList.ServiceProxy.Client;
using Polly;
using Polly.CircuitBreaker;

namespace ShoppingList.ServiceProxy.ResilienceHandlers;

public static class CircuitBreakerStrategyFactory
{
    public static CircuitBreakerStrategyOptions<HttpResponseMessage> GetDefaultCircuitBreakerOptions()
    {
        return new CircuitBreakerStrategyOptions<HttpResponseMessage>
        {
            ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                .Handle<ApiException>()
                .HandleResult(response => (int)response.StatusCode >= 500),
            
            BreakDuration = TimeSpan.FromSeconds(10),
            FailureRatio = 0.5,
            SamplingDuration = TimeSpan.FromSeconds(22),
            MinimumThroughput = 2,
            
            OnOpened = args =>
            {
                Console.WriteLine($"OnCircuitOpened, BreakDuration: {args.BreakDuration}");
                return ValueTask.CompletedTask;
            },
            OnClosed = args =>
            {
                Console.WriteLine("OnCircuitClosed");
                return ValueTask.CompletedTask;
            },
            OnHalfOpened = args =>
            {
                Console.WriteLine("OnCircuitHalfOpened - Testing the waters...");
                return ValueTask.CompletedTask;
            }
        };
    }
}