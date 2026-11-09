using System.Xml;
using XmlArchiveApp.Models;

namespace XmlArchiveApp.Strategies;

public class DomStrategy : IAnalysisStrategy
{
    private const string Upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZАБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯ";
    private const string Lower = "abcdefghijklmnopqrstuvwxyzабвгґдеєжзиіїйклмнопрстуфхцчшщьюя";

    public List<ArchiveItem> Analyze(string path, SearchCriteria criteria)
    {
        XmlDocument doc = Load(path);
        string xpath = BuildXPath(criteria);

        var items = new List<ArchiveItem>();
        foreach (XmlNode material in doc.SelectNodes(xpath)!)
            items.Add(ToItem(material));
        return items;
    }

    public Dictionary<string, List<string>> GetAttributeValues(string path)
    {
        XmlDocument doc = Load(path);
        var items = new List<ArchiveItem>();
        foreach (XmlNode material in doc.SelectNodes("/archive/material")!)
            items.Add(ToItem(material));
        return ArchiveValues.From(items);
    }

    private static string BuildXPath(SearchCriteria c)
    {
        var conditions = new List<string>();

        if (!string.IsNullOrEmpty(c.MaterialType)) conditions.Add($"@type={Lit(c.MaterialType)}");
        if (!string.IsNullOrEmpty(c.CreatedDate)) conditions.Add($"@created={Lit(c.CreatedDate)}");
        if (!string.IsNullOrEmpty(c.Author)) conditions.Add($"author/@name={Lit(c.Author)}");
        if (!string.IsNullOrEmpty(c.Faculty)) conditions.Add($"author/@faculty={Lit(c.Faculty)}");
        if (!string.IsNullOrEmpty(c.Department)) conditions.Add($"author/@department={Lit(c.Department)}");

        if (!string.IsNullOrWhiteSpace(c.Keyword))
        {
            string k = Lit(c.Keyword.Trim().ToLowerInvariant());
            conditions.Add(
                $"(contains(translate(title,'{Upper}','{Lower}'),{k}) or " +
                $"contains(translate(author/@name,'{Upper}','{Lower}'),{k}))");
        }

        return conditions.Count == 0
            ? "/archive/material"
            : "/archive/material[" + string.Join(" and ", conditions) + "]";
    }

    private static string Lit(string value) =>
        value.Contains('\'') ? $"\"{value}\"" : $"'{value}'";

    private static XmlDocument Load(string path)
    {
        var doc = new XmlDocument();
        doc.Load(path);
        return doc;
    }

    private static ArchiveItem ToItem(XmlNode material)
    {
        XmlNode? author = material.SelectSingleNode("author");
        return new ArchiveItem
        {
            Id = int.TryParse(material.Attributes?["id"]?.Value, out var id) ? id : 0,
            Title = material.SelectSingleNode("title")?.InnerText ?? "",
            Author = author?.Attributes?["name"]?.Value ?? "",
            Faculty = author?.Attributes?["faculty"]?.Value ?? "",
            Department = author?.Attributes?["department"]?.Value ?? "",
            MaterialType = material.Attributes?["type"]?.Value ?? "",
            Size = material.Attributes?["size"]?.Value ?? "",
            CreatedDate = material.Attributes?["created"]?.Value ?? ""
        };
    }
}