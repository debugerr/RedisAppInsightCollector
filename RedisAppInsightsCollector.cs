using Microsoft.ApplicationInsights;
using StackExchange.Redis.Profiling;
using System;

namespace Samples.Redis.AppInsightCollectors
{
    public sealed class RedisAppInsightsSessionCollector : IDisposable
    {
        private readonly TelemetryClient telemetryClient;
        private ProfilingSession currentSession;
        private string dependencyTypeName = "Redis";
        private string dependencyName = "Redis West Europe";

        public RedisAppInsightsSessionCollector(TelemetryClient telemetryClient)
        {
            this.telemetryClient = telemetryClient;
        }

        public ProfilingSession GetCurrentProfilingSession(string dependencyTypeName = null, string dependencyName = null)
        {
            if (dependencyName != null) this.dependencyName = dependencyName;
            if (dependencyTypeName != null) this.dependencyTypeName = dependencyTypeName;

            lock (this.telemetryClient)
            {
                if (this.currentSession == null)
                {
                    this.currentSession = new ProfilingSession(telemetryClient);
                }
                return this.currentSession;
            }
        }

        public void Dispose()
        {
            foreach (var itm in currentSession.FinishProfiling())
            {
                this.telemetryClient.TrackDependency(this.dependencyTypeName, this.dependencyName, itm.Command, itm.CommandCreated, itm.ElapsedTime, true);
            }
        }
    }
}
