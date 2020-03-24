using System;

namespace FinancialAccountService.Models.DataTransferObjects
{
    public class HeartBeatDto
    {
        public string ServiceName { get; set; }
        public string Details { get;set; }
        public TimeSpan RunningElapsedTime { get; set; }
        public TimeSpan TotalElapsedTime { get; set; }
        public string FriendlyDisplayElapsedTime => $"{TotalElapsedTime.Hours:00}:{TotalElapsedTime.Minutes:00}:{TotalElapsedTime.Seconds:00}.{TotalElapsedTime.Milliseconds / 10:00}";
        public string Status { get; set; }
        public HeartBeatDto ChildHeartBeat { get; set; }

        public void SetStatus(int greenThreshold, int redThreshold)
        {
            if (RunningElapsedTime.TotalSeconds < greenThreshold)
            {
                Status = "GREEN";
                return;
            }

            Status = RunningElapsedTime.TotalSeconds > redThreshold ? "RED" : "AMBER";
        }
    }
}