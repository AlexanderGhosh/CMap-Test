namespace CMapTest.Utils
{
    /// <summary>
    /// Inclusive, Null means ignore bound
    /// </summary>
    // i know i could have extended Range but I felt the functionality that it added might cause a future developer to assume that it abided by functionality that it infact doesn't
    public class Range<T>(T? min = null, T? max = null) where T : struct, IComparable<T>
    {
        public T? Min => min;
        public T? Max => max;
        public bool InRange(T x) => (Min is null || x.CompareTo(Min.Value) >= 0) && (Max is null || x.CompareTo(Max.Value) <= 0);
        // I could do [, ( for the bounds but i felt that showing int max/min might be concerning for a non technical user
        public override string ToString() => $"Min: {(Min.HasValue ? Min.Value : "Unbound")} Min: {(Max.HasValue ? Max.Value : "Unbound")}";
    }

    public class IntRange(int? min = null, int? max = null) : Range<int>(min, max);
    public class DateRange(DateOnly? min = null, DateOnly? max = null) : Range<DateOnly>(min, max);
}
