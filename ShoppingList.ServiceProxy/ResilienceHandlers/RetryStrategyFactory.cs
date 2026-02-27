using ShoppingList.ServiceProxy.Client;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace ShoppingList.ServiceProxy.ResilienceHandlers;

public static class RetryStrategyFactory
{
    public static RetryStrategyOptions<HttpResponseMessage> GetDefaultRetryOptions()
    {
        return new RetryStrategyOptions<HttpResponseMessage>
        {
            MaxRetryAttempts = 5,
            BackoffType = DelayBackoffType.Exponential,
            UseJitter = true,
            Delay = TimeSpan.FromSeconds(2),
            
            ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                .HandleResult(response => (int)response.StatusCode >= 500)
                .Handle<ApiException>()
                .Handle<BrokenCircuitException>(),

            OnRetry = static args =>
            {
                Console.WriteLine("OnRetry, Attempt: {0}, Waiting {1} ms", 
                    args.AttemptNumber + 1, 
                    args.RetryDelay.TotalMilliseconds);
                return default;
            }
        };
    }
}