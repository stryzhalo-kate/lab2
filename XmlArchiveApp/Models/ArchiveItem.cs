using System;
using System.Collections.Generic;
using System.Text;

namespace XmlArchiveApp.Models;

public class ArchiveItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Faculty { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string MaterialType { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string CreatedDate { get; set; } = string.Empty;
}