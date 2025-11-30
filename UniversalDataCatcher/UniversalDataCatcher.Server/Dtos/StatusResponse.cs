namespace UniversalDataCatcher.Server.Dtos
{
    public class StatusResponse
    {
        public string ServiceName { get; set; }
        public string ServiceLabelName { get; set; }
        public bool IsRunning { get; set; }
        public int Progress { get; set; }
        public int RepeatEvery { get; set; }
        public DateTime? SleepTime { get; set; }
        public DateTime? StartTime { get { return SleepTime?.AddMinutes(RepeatEvery); } }
    }
}
