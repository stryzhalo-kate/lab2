using System.Text;
using System.Xml;
using System.Xml.Xsl;

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
}