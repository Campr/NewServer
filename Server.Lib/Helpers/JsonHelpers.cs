using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Server.Lib.Infrastructure;
using Server.Lib.Services;

namespace Server.Lib.Helpers
{
    class JsonHelpers : IJsonHelpers, ITraceWriter
    {
        public JsonHelpers(
            ILoggingService loggingService,
            ITextHelpers textHelpers,
            IConfiguration configuration)
        {
            Ensure.Argument.IsNotNull(loggingService, nameof(loggingService));
            Ensure.Argument.IsNotNull(textHelpers, nameof(textHelpers));
            Ensure.Argument.IsNotNull(configuration, nameof(configuration));

            this.configuration = configuration;
            this.loggingService = loggingService;

            this.defaultFormatting = Formatting.None;
            this.defaultSettings = new JsonSerializerSettings
            {
                TraceWriter = this,
                ContractResolver = new BaseContractResolver(textHelpers)
            };
        }

        private readonly ILoggingService loggingService;
        private readonly IConfiguration configuration;
        private readonly Formatting defaultFormatting;
        private readonly JsonSerializerSettings defaultSettings;

        public string ToJsonString(object content)
        {
            return JsonConvert.SerializeObject(content, this.defaultFormatting, this.defaultSettings);
        }

        public TObject FromJsonString<TObject>(string source)
        {
            return JsonConvert.DeserializeObject<TObject>(source, this.defaultSettings);
        }

        public void Trace(TraceLevel level, string message, Exception ex)
        {
            // If an exception was provided, log it.
            if (ex != null)
            {
                this.loggingService.Exception(ex, "[Json] {0}", message);
                return;
            }

            // Otherwise, it depends on the log level.
            switch (level)
            {
                case TraceLevel.Error:
                    this.loggingService.Error("[Json] {0}", message);
                    break;
                default:
                    this.loggingService.Info("[Json] {0}", message);
                    break;
            }
        }

        public TraceLevel LevelFilter => this.configuration.JsonDebug
            ? TraceLevel.Verbose
            : TraceLevel.Off;
    }
}