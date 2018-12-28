using System;

namespace MMudObjects
{
    public class TrackedValue<T>
    {
        public T Value { get; set; }
        public DateTime TimeStamp { get; set; }

        public TrackedValueDelta<T> Delta { get; set; }

        public TrackedValue(T value)
        {
            this.Value = value;
            this.TimeStamp = DateTime.Now;
        }

        public TrackedValue(T value, TrackedValue<T> previous)
        {
            this.Value = value;
            this.TimeStamp = DateTime.Now;
            this.Delta = new TrackedValueDelta<T>(this, previous);
        }
    }

    public class TrackedValueDelta<T>
    {
        public T Value { get; set; }
        public TimeSpan timeSpan { get; set; }
        public string Rate { get; set; }

        public TrackedValueDelta(TrackedValue<T> now, TrackedValue<T> previous)
        {
            this.timeSpan = now.TimeStamp - previous.TimeStamp;
            this.Value = Difference(now.Value, previous.Value);

            double seconds = this.timeSpan.TotalSeconds;
            double minutes = seconds / 60;
            double hours = minutes / 60;

            this.Rate = this.Value + " per " + seconds + " seconds";
        }

        public T Difference(T x, T y)
        {
            dynamic a = (dynamic)x;
            dynamic b = (dynamic)y;
            dynamic result;

            try
            {
                result = a - b;
            }
            catch (Exception ex)
            {
                result = -1;
            }

            return result;
        }
    }
}
