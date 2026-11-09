using System;
using System.Collections.Generic;
using System.Text;
using XmlArchiveApp.Models;
using XmlArchiveApp.Strategies;

namespace XmlArchiveApp.Services;

public class ArchiveAnalyzer
{
    private IAnalysisStrategy? _strategy;

    public void SetStrategy(IAnalysisStrategy strategy)
    {
        _strategy = strategy;
    }

    public List<ArchiveItem> Search(string path, SearchCriteria criteria)
    {
        return GetStrategy().Analyze(path, criteria);
    }

    public Dictionary<string, List<string>> GetAttributeValues(string path)
    {
        return GetStrategy().GetAttributeValues(path);
    }

    private IAnalysisStrategy GetStrategy()
    {
        return _strategy ?? throw new InvalidOperationException("Стратегію аналізу не обрано.");
    }
}
