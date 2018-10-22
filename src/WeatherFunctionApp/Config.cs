using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherFunctionApp
{
    public static class Config
    {
        public static readonly string HttpProcessorUrl = Environment.GetEnvironmentVariable(nameof(HttpProcessorUrl)) ?? throw new ArgumentNullException(nameof(HttpProcessorUrl));
        public static readonly string BatchSize = Environment.GetEnvironmentVariable(nameof(BatchSize));
        public static readonly string MaxRecordCount = Environment.GetEnvironmentVariable(nameof(MaxRecordCount));
    }
}
