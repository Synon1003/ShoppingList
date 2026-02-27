var builder = DistributedApplication.CreateBuilder(args);

var mongo = builder.AddMongoDB("mongo", 59437);
var mongodb = mongo.AddDatabase("mongodb");

var redis = builder.AddRedis("redis")
                    .WithRedisInsight();

var apiService = builder.AddProject<Projects.ShoppingList_Service>("apiservice")
    .WithReference(redis)
    .WaitFor(redis)
    .WaitFor(mongodb)
    .WithReference(mongodb);

var proxyService = builder.AddProject<Projects.ShoppingList_ServiceProxy>("proxyservice")
    .WithReference(apiService)
    .WaitFor(apiService);

var webApp = builder.AddViteApp("webapp", "../ShoppingList.Client", "pnpm")
    .WithPnpmPackageInstallation()
    .WaitFor(apiService)
    .WithEnvironment("VITE_API_URL", apiService.GetEndpoint("http"));

builder.Build().Run();
