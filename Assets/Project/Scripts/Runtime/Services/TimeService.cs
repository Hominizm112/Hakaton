using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;



public class TimeService : Service
{

    [Serializable]
    public class GameDateTime
    {

        [SerializeField] private int _year;
        [SerializeField] private int _month;
        [SerializeField] private int _day;
        [SerializeField] private int _hour;
        [SerializeField] private int _minute;
        [SerializeField] private int _second;

        public int year
        {
            get => _year;
            private set => _year = value;
        }

        public int month
        {
            get => _month;
            private set
            {
                _month = value;
                if (_month > 12)
                {
                    year += (_month - 1) / 12;
                    _month = ((_month - 1) % 12) + 1;
                }
            }
        }

        public int day
        {
            get => _day;
            private set
            {
                _day = value;
                while (_day > 30)
                {
                    _day -= 30;
                    month++;
                }
            }
        }

        public int hour
        {
            get => _hour;
            private set
            {
                _hour = value;
                while (_hour >= 24)
                {
                    _hour -= 24;
                    day++;
                }
            }
        }

        public int minute
        {
            get => _minute;
            private set
            {
                _minute = value;
                while (_minute >= 60)
                {
                    _minute -= 60;
                    hour++;
                }
            }
        }

        public int second
        {
            get => _second;
            private set
            {
                _second = value;
                while (_second >= 60)
                {
                    _second -= 60;
                    minute++;
                }
            }
        }

        public void Update(int years = 0, int months = 0, int days = 0, int hours = 0, int minutes = 0, int seconds = 0)
        {
            this.year += years;
            this.month += months;
            this.day += days;
            this.hour += hours;
            this.minute += minutes;
            this.second += seconds;
        }

        public GameDateTime(int years = 1, int months = 1, int days = 1, int hours = 0, int minutes = 0, int seconds = 0)
        {
            this.year = years;
            this.month = months;
            this.day = days;
            this.hour = hours;
            this.minute = minutes;
            this.second = seconds;
        }

        public GameDateTime(GameDateTime dateTime)
        {
            this.year = dateTime.year;
            this.month = dateTime.month;
            this.day = dateTime.day;
            this.hour = dateTime.hour;
            this.minute = dateTime.minute;
            this.second = dateTime.second;
        }

        public bool Compare(GameDateTime dateTime)
        {
            return year == dateTime.year && month == dateTime.month && day == dateTime.day && hour == dateTime.hour && minute == dateTime.minute && second == dateTime.second;
        }

        public string ToString(string format = "")
        {
            switch (format)
            {
                case "hms":
                    return $"{hour:00}:{minute:00}:{second:00}";

                default:
                    return $"Year {year}, Month {month}, Day {day}, {hour:00}:{minute:00}:{second:00}";
            }

        }



    }

    [SerializeField] public GameDateTime currentTime = new();
    [SerializeField] private int timeScale = 60; // 1 секунда в жизни = n игровых секунд

    public Action OnTrackComplete;
    public Action<GameDateTime> OnTrackUpdate;

    private CancellationTokenSource _timeTrackingCts;
    private bool _isTracking;

    public override void Initialize()
    {
        base.Initialize();

        GlobalEventBus.Subscribe<TimeTrackStartEvent>(StartTrackEventHandler);
        GlobalEventBus.Subscribe<TimeTrackStopEvent>(StopTrackEventHandler);
    }



    #region Event Handlers
    private void StartTrackEventHandler(TimeTrackStartEvent @event)
    {
        StartTrack(@event.Seconds, @event.Minutes);
    }

    private void StopTrackEventHandler(TimeTrackStopEvent @event)
    {
        StopTracking();
    }

    #endregion

    private void Start()
    {
        OnTrackComplete += () => ColorfulDebug.LogBlue("Time track completed");
    }

    public void StartTrackMinutes(int minutes = 0)
    {
        StartTrack(minutes: minutes);
    }

    public void StartTrackSeconds(int seconds = 0)
    {
        StartTrack(seconds: seconds);
    }

    public void StartTrack(int seconds = 0, int minutes = 0)
    {
        if (seconds == 0 && minutes == 0)
        {
            TimeTrackCompleteHandler();
            return;
        }

        StopTracking();

        GameDateTime desiredTime = new(currentTime);
        desiredTime.Update(seconds: seconds, minutes: minutes);

        _timeTrackingCts = new CancellationTokenSource();
        _isTracking = true;

        TimeTrackingTask(desiredTime, _timeTrackingCts.Token).Forget();
    }

    public void StopTracking()
    {
        if (_isTracking)
        {
            _timeTrackingCts?.Cancel();
            _timeTrackingCts?.Dispose();
            _timeTrackingCts = null;
            _isTracking = false;
            TimeTrackCompleteHandler();
        }
    }

    public void ForceStopTracking()
    {
        if (_isTracking)
        {
            _timeTrackingCts?.Cancel();
            _timeTrackingCts?.Dispose();
            _timeTrackingCts = null;
            _isTracking = false;
        }
    }

    private async UniTask TimeTrackingTask(GameDateTime desiredDateTime, CancellationToken cancellationToken)
    {

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1), ignoreTimeScale: true, cancellationToken: cancellationToken);

                TimeTrackUpdateHandler();

                if (currentTime.Compare(desiredDateTime))
                {
                    StopTracking();
                    break;
                }
            }
        }
        catch (Exception e)
        {
            throw new Exception($"Time tracking error: {e}");
        }
        finally
        {
            _isTracking = false;
        }
    }


    public void TimeTrackCompleteHandler()
    {
        OnTrackComplete?.Invoke();
        GlobalEventBus.Publish<TimeTrackCompletedEvent>(new());
    }

    public void TimeTrackUpdateHandler()
    {
        currentTime.Update(seconds: timeScale);
        OnTrackUpdate?.Invoke(currentTime);
    }

    public override void Dispose()
    {
        StopTracking();
        OnTrackComplete = null;
        OnTrackUpdate = null;
    }


}
