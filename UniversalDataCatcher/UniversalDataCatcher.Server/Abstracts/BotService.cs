namespace UniversalDataCatcher.Server.Abstracts
{
    public class BotService
    {
        public int RepeatEvery { get; set; }
        public DateTime? SleepTime { get; set; }
        public DateTime? StartTime { get { return SleepTime?.AddMinutes(RepeatEvery); } }
        public bool IsRunning { get; set; }
        public int Progress { get; set; }
        public CancellationTokenSource? CancellationTokenSource { get; set; }
    }
}
