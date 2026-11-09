using System;
using System.Collections.Generic;
using System.Text;
using XmlArchiveApp.Models;

namespace XmlArchiveApp.Strategies;

internal static class ArchiveValues
{
    public static Dictionary<string, List<string>> From(IEnumerable<ArchiveItem> items)
    {
        var list = items.ToList();
        return new Dictionary<string, List<string>>
        {
            ["Author"] = Unique(list.Select(i => i.Author)),
            ["MaterialType"] = Unique(list.Select(i => i.MaterialType)),
            ["Faculty"] = Unique(list.Select(i => i.Faculty)),
            ["Department"] = Unique(list.Select(i => i.Department)),
            ["CreatedDate"] = Unique(list.Select(i => i.CreatedDate)),
        };
    }

    private static List<string> Unique(IEnumerable<string> values) =>
        values.Where(v => !string.IsNullOrEmpty(v)).Distinct().OrderBy(v => v).ToList();
}