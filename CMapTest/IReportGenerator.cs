using CMapTest.Utils;

namespace CMapTest
{
    public interface IReportGenerator
    {
        Task<DownloadableFile> GenerateUserReport(int userId, DateRange range, CancellationToken cancellationToken);
    }
}