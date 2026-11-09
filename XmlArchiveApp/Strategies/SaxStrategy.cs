using System.Xml;
using XmlArchiveApp.Models;

namespace XmlArchiveApp.Strategies;

public class SaxStrategy : IAnalysisStrategy
{
    public List<ArchiveItem> Analyze(string path, SearchCriteria criteria)
    {
        var conditions = criteria.ToConditions();
        return Read(path, item => conditions.All(f => f(item)));
    }

    public Dictionary<string, List<string>> GetAttributeValues(string path) =>
        ArchiveValues.From(Read(path, _ => true));

    private static List<ArchiveItem> Read(string path, Func<ArchiveItem, bool> accept)
    {
        var items = new List<ArchiveItem>();
        ArchiveItem? current = null;

        void Finish()
        {
            if (current != null && accept(current)) items.Add(current);
            current = null;
        }

        using XmlReader reader = XmlReader.Create(path);
        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case "material":
                        current = new ArchiveItem
                        {
                            Id = int.TryParse(reader.GetAttribute("id"), out var id) ? id : 0,
                            MaterialType = reader.GetAttribute("type") ?? "",
                            Size = reader.GetAttribute("size") ?? "",
                            CreatedDate = reader.GetAttribute("created") ?? ""
                        };
                        if (reader.IsEmptyElement) Finish();
                        break;

                    case "title":
                        if (current != null && reader.Read())
                            current.Title = reader.Value;
                        break;

                    case "author":
                        if (current != null)
                        {
                            current.Author = reader.GetAttribute("name") ?? "";
                            current.Faculty = reader.GetAttribute("faculty") ?? "";
                            current.Department = reader.GetAttribute("department") ?? "";
                        }
                        break;
                }
            }
            else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "material")
            {
                Finish();
            }
        }
        return items;
    }
}