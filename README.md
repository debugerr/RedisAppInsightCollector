# RedisAppInsightCollector
Sample for collecting Redis Stats and expose them as dependency data to Azure Application Insight

## Configuration
In your startup code, configure the dependency injection to setup a scoped collector like this:

````
// setup the redis ConnectionMultiplexer, so you can re-use it and register it as a singleton
var connection = ConnectionMultiplexer.Connect("localhost:6379");
services.AddSingleton<ConnectionMultiplexer>(connection);

// when registering the IDatabase interface, you get an instance of the collector and
// Register it as a profiler with redis
services.AddScoped<IDatabase>(sp =>
            {
                var collector = sp.GetService<RedisAppInsightsSessionCollector>();
                connection.RegisterProfiler(() => collector.GetCurrentProfilingSession());
                return connection.GetDatabase();
            });
````