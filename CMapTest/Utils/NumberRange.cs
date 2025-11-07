namespace CMapTest.Utils
{
    /// <summary>
    /// Inclusive, Null means ignore bound
    /// </summary>
    // i know i could have extended Range but I felt the functionality that it added might cause a future developer to assume that it abided by functionality that it infact doesn't
    public struct NumberRange(int? min = null, int? max = null)
    {
        public readonly int? Min => min;
        public readonly int? Max => max;
        public readonly bool InRange(int x) => (Min is null || x >= Min) && (Max is null || x <= Max);
        // I could do [, ( for the bounds but i felt that showing int max/min might be concerning for a non technical user
        public readonly override string ToString() => $"Min: {(Min.HasValue ? Min.Value : "Unbound")} Min: {(Max.HasValue ? Max.Value : "Unbound")}";
    }
}
