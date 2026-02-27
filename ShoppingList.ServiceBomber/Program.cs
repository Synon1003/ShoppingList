
using NBomber.CSharp;
using System.Net.Http;
using NBomber.Http.CSharp;
using NBomber.Http;
using ShoppingList.ServiceBomber;
using Bogus;
using NBomber.Sinks.Timescale;
using NBomber.Contracts;

var baseUrl = "https://localhost:7133/";
var httpClient = Http.CreateDefaultClient();
/*var config = new TimescaleDbSinkConfig(connectionString: "Host=localhost;Port=5432;Username=timescaledb;Password=timescaledb;Database=nb_studio_db;Pooling=true;");
var timescaleDb = new TimescaleDbSink(config);*/
var faker = new Faker();

// fill the database
var createItemsScenario = Scenario.Create("CreateItems", async context =>
{
    var item = new CreateItemDto
    (
        Name: faker.Commerce.ProductName(),
        Price: faker.Random.Int(1, 100000),
        Description: faker.Commerce.ProductDescription()
    );
    var request = Http.CreateRequest("POST",$"{baseUrl}items").WithJsonBody(item);
    var response = await Http.Send<ItemDto>(httpClient, request);

    return response;
})
.WithLoadSimulations(Simulation.Inject(rate: 10, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(5)));


var getItemsScenario = Scenario.Create("GetItems", async context =>
{

    var request = Http.CreateRequest("GET", $"{baseUrl}items");
    var response = await Http.Send<ItemsPageDto>(httpClient, request);

    return response;
})
.WithThresholds(
    Threshold.Create(stats => stats.Fail.Request.Percent < 10), // error rate 10%
    Threshold.Create(stats => stats.Ok.Latency.Percent99 < 1000) // 99% of 1000ms
)
.WithWarmUpDuration(TimeSpan.FromSeconds(3))
.WithLoadSimulations(
    Simulation.Inject(rate: 5, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(20)), // 5 RPS for 20 seconds
    Simulation.RampingInject(rate: 10, interval: TimeSpan.FromSeconds(1),  during: TimeSpan.FromSeconds(10)), // Ramp up to 10 RPS for 10 seconds
    Simulation.Inject(rate: 10, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(15)), // Hold 10 RPS for 15 seconds
    Simulation.RampingInject(rate: 2, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(5))  // Ramp down for 2 RPS for 5 seconds
);


var result = NBomberRunner.RegisterScenarios(
    createItemsScenario,
    getItemsScenario
)
.WithTestName("ItemsTest")
.WithTestSuite("LoadTestSuite")
//.WithReportingSinks(timescaleDb)  
.WithReportingInterval(TimeSpan.FromSeconds(10))
.WithReportFileName($"ItemsTest-{DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss")}")
.Run();

var stats = result.ScenarioStats.Get("GetItems");
bool tooManyErrors = stats.Fail.Request.Percent > 5; // More than 5% error rate
bool tooSlow = stats.Ok.Latency.Percent99 > 500; // P99 latency above 500ms

if (tooManyErrors || tooSlow)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("[ASSERTION FAILED]: The API is too slow or has too many errors!");
    Console.ResetColor();
    
    Environment.Exit(-1);
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("[ASSERTION PASSED]: API performance is within limits.");
Console.ResetColor();





