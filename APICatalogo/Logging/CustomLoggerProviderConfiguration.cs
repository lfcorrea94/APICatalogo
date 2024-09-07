namespace APICatalogo.Logging
{
    public class CustomLoggerProviderConfiguration
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Warning; // Nível mínimo de log a ser registrado.
        public int EventId { get; set; } = 0;
    }
}
