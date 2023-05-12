using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BITPayService
{
    public delegate void TimeReachedEventHandler(DateTime Time);

   public class DailyTimer:IDisposable
    {
        private Timer myTimer = null;

        private int dayHour = 9;
        private int dayMinute = 0;
        public event TimeReachedEventHandler TimeReached;

        public DailyTimer(int dayHour, int dayMinute)
        {
            if ((dayHour < 0) || (dayHour > 23))
                throw new ArgumentException("Day hour is less than 1 or more than 23!");

            if ((dayMinute < 0) || (dayMinute > 59))
                throw new ArgumentException("Day minuets is less than 0 or more than 60!");

            this.dayHour = dayHour;
            this.dayMinute = dayMinute;
        }

        public void ScheduleRun()
        {
            //---- Subtract the current time, from timer time (tomorrow).
            DateTime nextRun = this.GetTimerTime();
            TimeSpan ts = nextRun.Subtract(DateTime.Now);

            Util.LogError("DailyTimer_NextRun", new Exception(nextRun.ToString("dd/MM/yyyy HH:mm:ss")), false);

            //==================================
            myTimer = new Timer(TimerCallback, null, (int)ts.TotalMilliseconds, Timeout.Infinite);
            //=================================
        }

        private DateTime GetTimerTime()
        {
            DateTime currentTime = DateTime.Now;
            DateTime runTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, dayHour, dayMinute, 0);
            if (runTime < currentTime)
                runTime = currentTime.AddDays(1);

            //---- 24hours
            return new DateTime(runTime.Year, runTime.Month, runTime.Day, dayHour, dayMinute, 0);
        }

        public void TimerCallback(Object stateInfo)
        {
            TimeReached?.Invoke(DateTime.Now);
        }

        public void Dispose()
        {
            myTimer.Dispose();
        }
    }
}
