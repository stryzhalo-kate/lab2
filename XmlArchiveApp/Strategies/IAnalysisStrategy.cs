using System;
using System.Collections.Generic;
using System.Text;

using XmlArchiveApp.Models;

namespace XmlArchiveApp.Strategies;

public interface IAnalysisStrategy
{
    
    List<ArchiveItem> Analyze(string path, SearchCriteria criteria);

    Dictionary<string, List<string>> GetAttributeValues(string path);
}