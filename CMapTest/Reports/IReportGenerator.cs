using CMapTest.Utils;

namespace CMapTest.Reports
{
    public interface IReportGenerator
    {
        Task<DownloadableFile> GenerateUserReport(int userId, DateTimeRange range, CancellationToken cancellationToken);
    }
}