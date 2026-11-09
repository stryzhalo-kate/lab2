namespace XmlArchiveApp.Models;

public class SearchCriteria
{
    public string Author { get; set; } = string.Empty;
    public string MaterialType { get; set; } = string.Empty;
    public string Faculty { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string CreatedDate { get; set; } = string.Empty;
    public string Keyword { get; set; } = string.Empty;

    public bool IsEmpty =>
        string.IsNullOrEmpty(Author) && string.IsNullOrEmpty(MaterialType) &&
        string.IsNullOrEmpty(Faculty) && string.IsNullOrEmpty(Department) &&
        string.IsNullOrEmpty(CreatedDate) && string.IsNullOrWhiteSpace(Keyword);

    public List<Func<ArchiveItem, bool>> ToConditions()
    {
        var conditions = new List<Func<ArchiveItem, bool>>();

        if (!string.IsNullOrEmpty(Author)) conditions.Add(i => i.Author == Author);
        if (!string.IsNullOrEmpty(MaterialType)) conditions.Add(i => i.MaterialType == MaterialType);
        if (!string.IsNullOrEmpty(Faculty)) conditions.Add(i => i.Faculty == Faculty);
        if (!string.IsNullOrEmpty(Department)) conditions.Add(i => i.Department == Department);
        if (!string.IsNullOrEmpty(CreatedDate)) conditions.Add(i => i.CreatedDate == CreatedDate);
        if (!string.IsNullOrWhiteSpace(Keyword))
            conditions.Add(i => Contains(i.Title, Keyword) || Contains(i.Author, Keyword));

        return conditions;
    }

    public bool Matches(ArchiveItem item) => ToConditions().All(f => f(item));

    private static bool Contains(string text, string word) =>
        text.Contains(word.Trim(), StringComparison.OrdinalIgnoreCase);
}