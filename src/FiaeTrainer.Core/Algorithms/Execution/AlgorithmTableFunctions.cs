using System.Text.Json;
using FiaeTrainer.Core.Algorithms.Syntax;

namespace FiaeTrainer.Core.Algorithms.Execution;

/// <summary> Isolierte Datenquelle und Ausgabevertrag des Tabellenauftrags; keine Datei- oder Datenbankzugriffe. </summary>
internal sealed class AlgorithmTableFunctions
{
    /// <summary> Lesestand und Ausgabe gehören nur zu diesem Prüffall. </summary>
    private readonly string[] _records;
    private int _read;
    private int _written;
    private bool _header;
    private bool _footer;
    private bool _endRead;

    /// <summary> Übernimmt die synthetische Datenfolge aus dem Auftrag, nicht aus dem eingereichten Programm. </summary>
    internal AlgorithmTableFunctions(IReadOnlyDictionary<string, JsonElement> inputs)
    {
        if (!inputs.TryGetValue("datensaetze", out var records) || records.ValueKind != JsonValueKind.Array || records.GetArrayLength() > 100)
            throw new InvalidDataException("Der Tabellenauftrag benötigt höchstens 100 Datensätze.");
        _records = records.EnumerateArray().Select(record => record.ValueKind == JsonValueKind.String ? record.GetString()! : throw new InvalidDataException("Datensätze müssen Texte sein.")).ToArray();
        if (_records.Any(record => record.Length == 0) || _records.Sum(record => record.Length) > 12000)
            throw new InvalidDataException("Die Datenfolge enthält einen leeren oder zu umfangreichen Datensatz.");
    }

    /// <summary> Nur diese vier Namen werden im passenden Auftrag bereitgestellt. </summary>
    internal static bool Contains(string name) => name.ToLowerInvariant() is "hole_daten" or "schreibe_header" or "schreibe_datensatz" or "schreibe_footer";

    /// <summary> Prüft Aufrufvertrag und Ausgabeordnung; null bezeichnet einen Aufruf ohne Rückgabewert. </summary>
    internal object? Invoke(string name, IReadOnlyList<object> arguments, int line, Action<object> print)
    {
        string function = name.ToLowerInvariant();
        int count = function == "schreibe_datensatz" ? 1 : 0;
        if (arguments.Count != count) throw new AlgorithmException(line, $"„{name}“ erwartet {count} Argumente, übergeben wurden {arguments.Count}.");
        switch (function)
        {
            case "hole_daten":
                if (_read < _records.Length) return _records[_read++];
                _endRead = true;
                return "";
            case "schreibe_header":
                if (_header || _footer) throw new AlgorithmException(line, "Die Tabellenüberschrift muss genau einmal vor den ausgegebenen Datensätzen stehen.");
                _header = true;
                print("Kundennummer | Name | Ort");
                break;
            case "schreibe_datensatz":
                if (!_header || _footer) throw new AlgorithmException(line, "Ein Datensatz gehört zwischen Tabellenüberschrift und Tabellenabschluss.");
                if (arguments[0] is not string record) throw new AlgorithmException(line, "schreibe_datensatz erwartet den Text eines abgerufenen Datensatzes.");
                if (record.Length == 0) throw new AlgorithmException(line, "Der Leerstring beendet die Datenfolge und darf nicht als Datensatz ausgegeben werden.");
                if (_written >= _read || _written >= _records.Length || record != _records[_written])
                    throw new AlgorithmException(line, "Gib jeden abgerufenen Datensatz genau einmal und in seiner ursprünglichen Reihenfolge aus.");
                _written++;
                print(record);
                break;
            case "schreibe_footer":
                if (!_header || _footer) throw new AlgorithmException(line, "Der Tabellenabschluss muss genau einmal nach der Überschrift und allen Datensätzen stehen.");
                if (_written != _records.Length || !_endRead) throw new AlgorithmException(line, "Rufe bis zum Leerstring ab und gib alle Datensätze aus, bevor du die Tabelle abschließt.");
                _footer = true;
                print("Ende der Tabelle");
                break;
        }
        return null;
    }

    /// <summary> Ein still beendetes Programm ersetzt keine vollständige Ausgabe über die vereinbarten Hilfsfunktionen. </summary>
    internal void EnsureComplete(int line)
    {
        if (!_header || !_footer) throw new AlgorithmException(line, "Verwende schreibe_header() und schreibe_footer() auch bei einer leeren Datenfolge.");
    }
}
