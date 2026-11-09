using System.Xml.Linq;
using XmlArchiveApp.Models;

namespace XmlArchiveApp.Strategies;

public class LinqToXmlStrategy : IAnalysisStrategy
{
    public List<ArchiveItem> Analyze(string path, SearchCriteria c)
    {
        IEnumerable<XElement> query = XDocument.Load(path).Root!.Elements("material");

        if (!string.IsNullOrEmpty(c.MaterialType))
            query = query.Where(m => (string?)m.Attribute("type") == c.MaterialType);
        if (!string.IsNullOrEmpty(c.CreatedDate))
            query = query.Where(m => (string?)m.Attribute("created") == c.CreatedDate);
        if (!string.IsNullOrEmpty(c.Author))
            query = query.Where(m => (string?)m.Element("author")?.Attribute("name") == c.Author);
        if (!string.IsNullOrEmpty(c.Faculty))
            query = query.Where(m => (string?)m.Element("author")?.Attribute("faculty") == c.Faculty);
        if (!string.IsNullOrEmpty(c.Department))
            query = query.Where(m => (string?)m.Element("author")?.Attribute("department") == c.Department);

        if (!string.IsNullOrWhiteSpace(c.Keyword))
        {
            string k = c.Keyword.Trim();
            query = query.Where(m =>
                ((string?)m.Element("title") ?? "").Contains(k, StringComparison.OrdinalIgnoreCase) ||
                ((string?)m.Element("author")?.Attribute("name") ?? "").Contains(k, StringComparison.OrdinalIgnoreCase));
        }

        return query.Select(ToItem).ToList();
    }

    public Dictionary<string, List<string>> GetAttributeValues(string path) =>
        ArchiveValues.From(XDocument.Load(path).Root!.Elements("material").Select(ToItem));

    private static ArchiveItem ToItem(XElement m) => new()
    {
        Id = (int?)m.Attribute("id") ?? 0,
        Title = (string?)m.Element("title") ?? "",
        Author = (string?)m.Element("author")?.Attribute("name") ?? "",
        Faculty = (string?)m.Element("author")?.Attribute("faculty") ?? "",
        Department = (string?)m.Element("author")?.Attribute("department") ?? "",
        MaterialType = (string?)m.Attribute("type") ?? "",
        Size = (string?)m.Attribute("size") ?? "",
        CreatedDate = (string?)m.Attribute("created") ?? ""
    };
}