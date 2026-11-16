using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using XmlArchiveApp.Models;

namespace XmlArchiveApp.Services;

public class HtmlTransformer
{
    public string Transform(string xmlPath, string xslPath)
    {
        var xslt = new XslCompiledTransform();
        xslt.Load(xslPath);

        var sb = new StringBuilder();
        using (XmlWriter writer = XmlWriter.Create(sb, xslt.OutputSettings))
        {
            xslt.Transform(xmlPath, writer);
        }
        return sb.ToString();
    }

     
    public string TransformItems(IEnumerable<ArchiveItem> items, string xslPath)
    {
        var xml = new XDocument(
            new XElement("archive",
                items.Select(i =>
                {
                    var material = new XElement("material",
                        new XAttribute("id", i.Id),
                        new XAttribute("type", i.MaterialType));

                    if (!string.IsNullOrEmpty(i.CreatedDate))
                        material.Add(new XAttribute("created", i.CreatedDate));
                    if (!string.IsNullOrEmpty(i.Size))
                        material.Add(new XAttribute("size", i.Size));

                    material.Add(new XElement("title", i.Title));
                    material.Add(new XElement("author",
                        new XAttribute("name", i.Author),
                        new XAttribute("faculty", i.Faculty),
                        new XAttribute("department", i.Department)));
                    return material;
                })));

        var xslt = new XslCompiledTransform();
        xslt.Load(xslPath);

        var sb = new StringBuilder();
        using (XmlReader reader = xml.CreateReader())
        using (XmlWriter writer = XmlWriter.Create(sb, xslt.OutputSettings))
        {
            xslt.Transform(reader, writer);
        }
        return sb.ToString();
    }
}