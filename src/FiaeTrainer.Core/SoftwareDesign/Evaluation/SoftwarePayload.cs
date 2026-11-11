using System.Text.Json;
using System.Xml;
using System.Xml.Linq;

namespace FiaeTrainer.Core.SoftwareDesign.Evaluation;

/// <summary> Vergleicht kleine Nachrichten strukturell; führt keine Nutzereingaben oder externen Zugriffe aus. </summary>
public static class SoftwarePayload
{
    /// <summary> JSON-Eigenschaftsreihenfolge und XML-Einrückung sind frei; Typen und Arrayreihenfolge zählen. </summary>
    public static bool Equivalent(string format, string expected, string actual)
    {
        if (actual.Length > 8000 || string.IsNullOrWhiteSpace(actual)) return false;
        try
        {
            if (format == "Json")
            {
                using var first = JsonDocument.Parse(expected, new() { MaxDepth = 16 });
                using var second = JsonDocument.Parse(actual, new() { MaxDepth = 16 });
                return JsonEqual(first.RootElement, second.RootElement);
            }
            if (format == "Xml") return Canonical(ReadXml(expected)) == Canonical(ReadXml(actual));
            return false;
        }
        catch (Exception ex) when (ex is JsonException or XmlException or InvalidOperationException or FormatException) { return false; }
    }
    private static bool JsonEqual(JsonElement first, JsonElement second)
    {
        if (first.ValueKind != second.ValueKind) return false;
        if (first.ValueKind == JsonValueKind.Object)
        {
            var a = first.EnumerateObject().ToArray(); var b = second.EnumerateObject().ToArray();
            return a.Length == b.Length && a.Select(item => item.Name).Distinct(StringComparer.Ordinal).Count() == a.Length && b.Select(item => item.Name).Distinct(StringComparer.Ordinal).Count() == b.Length
                && a.All(item => second.TryGetProperty(item.Name, out var value) && JsonEqual(item.Value, value));
        }
        if (first.ValueKind == JsonValueKind.Array) return first.GetArrayLength() == second.GetArrayLength() && first.EnumerateArray().Zip(second.EnumerateArray()).All(pair => JsonEqual(pair.First, pair.Second));
        if (first.ValueKind == JsonValueKind.Number) return first.TryGetDecimal(out decimal a) && second.TryGetDecimal(out decimal b) ? a == b : first.GetRawText() == second.GetRawText();
        return first.ValueKind == JsonValueKind.String ? first.GetString() == second.GetString() : first.GetRawText() == second.GetRawText();
    }
    private static XElement ReadXml(string value)
    {
        using var text = new StringReader(value);
        var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null, MaxCharactersInDocument = 8000, MaxCharactersFromEntities = 0, IgnoreComments = true };
        using (var depthReader = XmlReader.Create(text, settings))
            while (depthReader.Read()) if (depthReader.Depth > 16) throw new XmlException("Die Nachricht ist zu tief verschachtelt.");
        using var input = new StringReader(value);
        using var reader = XmlReader.Create(input, settings);
        return XElement.Load(reader);
    }
    /// <summary> Die XML-Normalisierung erhält Elementreihenfolge, Namensräume und Textwerte. </summary>
    private static string Canonical(XElement element) => new XElement(element.Name, element.Attributes().Where(attribute => !attribute.IsNamespaceDeclaration).OrderBy(attribute => attribute.Name.ToString(), StringComparer.Ordinal).Select(attribute => new XAttribute(attribute.Name, attribute.Value)), element.Nodes().Where(node => node is XElement || node is XText text && !string.IsNullOrWhiteSpace(text.Value)).Select(node => node is XElement child ? (object)XElement.Parse(Canonical(child)) : ((XText)node).Value)).ToString(SaveOptions.DisableFormatting);
}
