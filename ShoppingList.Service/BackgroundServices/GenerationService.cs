using System.Threading.Channels;
using System.Collections.Concurrent;
using ShoppingList.Core.Domain.Entities;

namespace ShoppingList.Service.BackgroundServices;

public class GenerationService(
    ILogger<GenerationService> logger,
    Channel<GenerationJob> channel,
    ConcurrentDictionary<string, GenerationStatus> statusDictionary
): BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach(var job in channel.Reader.ReadAllAsync())
        {
            try
            {
                await ProcessJobAsync(job);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing generation job");
            }
        }
    }

    private async Task ProcessJobAsync(GenerationJob job)
    {
        statusDictionary[job.Id] = GenerationStatus.Processing;

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(15));
            statusDictionary[job.Id] = GenerationStatus.Completed;
        }
        catch (Exception ex)
        {
            statusDictionary[job.Id] = GenerationStatus.Failed;
            throw;
        }
    }
}