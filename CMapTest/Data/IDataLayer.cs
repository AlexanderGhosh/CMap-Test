namespace CMapTest.Data
{
    // this is async because future implementations might be better served by it
    public interface IDataLayer
    {
        void Seed() { }
    }
}
