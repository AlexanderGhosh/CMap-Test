using CMapTest.Data;
using CMapTest.Models;
using CMapTest.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CMapTest.Reports
{
    public class DownloadableFile
    {
        public string? FilePath { get; set; }
        public required string DownloadName { get; set; }
        public byte[]? Data { get; set; }
        public string ContentType { get; set; } = "application/text";
        [MemberNotNullWhen(true, nameof(FilePath), nameof(Data))]
        public bool IsValid => FilePath is not null || Data is not null;
    }
    public class ReportGenerator(IUserDataLayer _users, IEntriesDataLayer _entries) : IReportGenerator
    {
        public async Task<DownloadableFile> GenerateUserReport(int userId, DateRange range, CancellationToken cancellationToken)
        {
            User user = await _users.GetUserFromId(userId, cancellationToken);
            IEnumerable<Entry> entriesRaw = await _entries.EntrySearch(new()
            {
                UserId = userId,
                DateStart = range.Min,
                DateEnd = range.Max
            }, cancellationToken);
            IEnumerable<EntryPretty> entries = entriesRaw.Select(e => _entries.GetPretty(e.Id, default).Result);
            var projectEntries = entries.GroupBy(e => e.ProjectId);

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("Report for User: {0} In Range: {1}", user.FullName, range).AppendLine();
            if (!projectEntries.Any())
            {
                builder.AppendFormat("{0} has not done any work in the selected period", user.PreferredName);
            }
            foreach (IGrouping<int, EntryPretty> group in projectEntries)
            {
                bool first = true;
                foreach (EntryPretty entry in group)
                {
                    if (first)
                    {
                        builder.AppendFormat("Project: {0}", entry.ProjectName).AppendLine().AppendLine(new string('-', 100));
                        first = true;
                    }
                    builder.AppendFormat("Date: {0} | Time worked: {1} | Description: {2}", entry.Date, entry.WorkingPeriod, entry.Description ?? "N/A").AppendLine();
                }
                builder.AppendFormat("Total time worked: {0}", group.Select(e => e.WorkingPeriodRaw).Aggregate((a, b) => a + b));
            }

            using MemoryStream stream = new();
            using StreamWriter writer = new(stream);
            writer.Write(builder);
            writer.Flush();
            string safeUserName = getSafeName(user.FullName);
            string safeDateRange = getSafeName(range.ToString());
            return new DownloadableFile()
            {
                Data = stream.ToArray(),
                DownloadName = $"Timesheet {safeUserName} Range {safeDateRange}.txt"
            };
        }

        private string getSafeName(string raw) => raw;
    }
}
