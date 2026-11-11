using System.Globalization;
using FiaeTrainer.Core.QualityAssurance.Content;

namespace FiaeTrainer.Core.QualityAssurance.Evaluation;

/// <summary> Führt acht feste Rechen-/Zustandsmodelle isoliert aus; weder Fremdcode noch externe Zugriffe. </summary>
public static class QualitySimulator
{
    /// <summary> Testdaten dürfen fachlich ungültig sein, müssen aber zum ausdrücklich begrenzten Eingabeformat passen. </summary>
    public static bool ValidInputs(QualityProgram program, IReadOnlyDictionary<string, string> inputs)
        => inputs.Count == program.Inputs.Count && program.Inputs.All(input => inputs.TryGetValue(input.Key, out string? value) && (input.Kind == "Select" ? input.Options.ContainsKey(value) : int.TryParse(value.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int number) && number >= input.Minimum && number <= input.Maximum));

    /// <summary> Liefert Rückgabe und tatsächlichen Weg; jeder Aufruf beginnt mit frischen lokalen Daten. </summary>
    public static QualityExecution? Run(QualityProgram program, IReadOnlyDictionary<string, string> inputs, string fault = "")
    {
        if (!ValidInputs(program, inputs) || fault.Length > 0 && !program.Faults.ContainsKey(fault)) return null;
        var result = new QualityExecution(); List<string> path = [];
        int N(string key) => int.Parse(inputs[key], CultureInfo.InvariantCulture);
        bool B(string key) => inputs[key] == "true";
        bool D(string key, bool outcome) { string branch = key + (outcome ? ".T" : ".F"); result.Branches.Add(branch); path.Add(branch); return outcome; }
        void S(string key) => result.Statements.Add(key);
        QualityExecution Return(string statement, object output)
        {
            S(statement); result.Output = output is bool boolean ? boolean ? "true" : "false" : Convert.ToString(output, CultureInfo.InvariantCulture)!;
            result.Path = string.Join("|", path); return result;
        }
        switch (program.Key)
        {
            case "capacity":
            {
                int n = N("people"), lower = fault == "accept-zero" ? 0 : 1;
                if (D("D1", (fault == "exclude-min" ? n <= lower : n < lower) || (fault == "exclude-max" ? n >= 24 : n > 24))) return Return("S1", false);
                return Return("S2", true);
            }
            case "fee":
            {
                int n = N("people"); if (D("D1", n < 1 || n > 24)) return Return("S1", -1);
                int fee = n * 1200; S("S2");
                if (D("D2", (B("member") || fault == "ignore-member") && (fault == "strict-threshold" ? n > 10 : n >= 10))) { fee = fee * (fault == "wrong-discount" ? 90 : 80) / 100; S("S3"); }
                return Return("S4", fee);
            }
            case "rental":
            {
                int days = N("days"); if (D("D1", days < 1 || (fault == "exclude-max" ? days >= 14 : days > 14))) return Return("S1", -1);
                int fee = days * 7; S("S2");
                if (D("D2", B("damaged"))) { fee += fault == "missing-damage" ? 0 : fault == "double-damage" ? 40 : 20; S("S3"); }
                return Return("S4", fee);
            }
            case "shipping":
            {
                int weight = N("weight"); if (D("D1", weight < 1 || weight > 5000)) return Return("S1", -1);
                int fee;
                if (D("D2", fault == "strict-split" ? weight < 1000 : weight <= 1000)) { fee = 4; S("S2"); }
                else { fee = fault == "wrong-heavy" ? 7 : 8; S("S3"); }
                if (D("D3", B("express"))) { fee += fault == "ignore-express" ? 0 : 3; S("S4"); }
                return Return("S5", fee);
            }
            case "access":
            {
                string role = inputs["role"]; bool own = B("own");
                if (D("D1", role == "admin" && (fault != "admin-own" || own))) return Return("S1", true);
                if (D("D2", role == "member" && (own || fault == "member-all") || fault == "guest-own" && role == "guest" && own)) return Return("S2", true);
                return Return("S3", false);
            }
            case "attempt":
            {
                int failed = N("failed");
                if (D("D1", fault != "ignore-lock" && (fault == "late-lock" ? failed > 3 : failed >= 3))) return Return("S1", "locked");
                if (D("D2", fault == "reverse-result" ? !B("correct") : B("correct"))) return Return("S2", "allowed");
                return Return("S3", "rejected");
            }
            case "cancel":
            {
                string state = inputs["state"], action = inputs["action"];
                if (D("D1", state == "cancelled")) return Return("S1", fault == "repeat-cancel" && action == "cancel" ? "cancelled" : "rejected");
                if (D("D2", action == "confirm" && (state == "requested" || fault == "repeat-confirm"))) return Return("S2", "confirmed");
                if (D("D3", action == "cancel" || fault == "ignore-action")) return Return("S3", "cancelled");
                return Return("S4", "rejected");
            }
            case "availability":
            {
                int capacity = N("capacity"), booked = N("booked");
                if (D("D1", (fault == "zero-capacity" ? capacity < 0 : capacity < 1) || booked < 0 || fault != "overbooked" && booked > capacity)) return Return("S1", -1);
                return Return("S2", capacity - booked + (fault == "off-by-one" ? 1 : 0));
            }
            default: return null;
        }
    }

    /// <summary> Leitet Klassen und Grenzen aus der Spezifikation ab, unabhängig von einer Fehlervariante. </summary>
    public static HashSet<string> Partitions(QualityProgram program, IReadOnlyDictionary<string, string> inputs)
    {
        if (!ValidInputs(program, inputs)) return [];
        HashSet<string> result = [];
        int N(string key) => int.Parse(inputs[key], CultureInfo.InvariantCulture);
        bool B(string key) => inputs[key] == "true";
        void Add(string key, bool applies) { if (applies) result.Add(key); }
        switch (program.Key)
        {
            case "capacity":
                int people = N("people"); Add("below", people < 1); Add("valid", people is >= 1 and <= 24); Add("above", people > 24); Add("min", people == 1); Add("max", people == 24); Add("before-min", people == 0); Add("after-max", people == 25); break;
            case "fee":
                int n = N("people"); Add("below", n < 1); Add("above", n > 24); Add("discount", n is >= 10 and <= 24 && B("member")); Add("member-small", n is >= 1 and < 10 && B("member")); Add("nonmember", n is >= 1 and <= 24 && !B("member")); Add("before-threshold", n == 9); Add("threshold", n == 10); Add("after-threshold", n == 11); break;
            case "rental":
                int days = N("days"); Add("below", days < 1); Add("above", days > 14); Add("min", days == 1); Add("max", days == 14); Add("damaged", days is >= 1 and <= 14 && B("damaged")); Add("intact", days is >= 1 and <= 14 && !B("damaged")); break;
            case "shipping":
                int weight = N("weight"); Add("below", weight < 1); Add("above", weight > 5000); Add("light", weight is >= 1 and <= 1000); Add("heavy", weight is > 1000 and <= 5000); Add("express", weight is >= 1 and <= 5000 && B("express")); Add("standard", weight is >= 1 and <= 5000 && !B("express")); Add("split", weight == 1000); Add("after-split", weight == 1001); break;
            case "access":
                Add("guest", inputs["role"] == "guest"); Add("member-own", inputs["role"] == "member" && B("own")); Add("member-other", inputs["role"] == "member" && !B("own")); Add("admin", inputs["role"] == "admin"); break;
            case "attempt":
                int failed = N("failed"); Add("locked", failed >= 3); Add("allowed", failed < 3 && B("correct")); Add("rejected", failed < 3 && !B("correct")); Add("before-lock", failed == 2); Add("lock", failed == 3); break;
            case "cancel":
                string state = inputs["state"], action = inputs["action"]; Add("already-cancelled", state == "cancelled"); Add("confirm", state == "requested" && action == "confirm"); Add("cancel", state != "cancelled" && action == "cancel"); Add("invalid-confirm", state == "confirmed" && action == "confirm"); break;
            case "availability":
                int capacity = N("capacity"), booked = N("booked"); Add("invalid-capacity", capacity < 1); Add("invalid-booked", booked < 0); Add("overbooked", capacity > 0 && booked > capacity); Add("empty", capacity > 0 && booked == 0); Add("full", capacity > 0 && booked == capacity); Add("partial", booked > 0 && booked < capacity); break;
        }
        return result;
    }

    /// <summary> Sollwerte dürfen gleichwertig geschrieben sein; Datentyp und Fachwert bleiben verbindlich. </summary>
    public static bool ExpectedEqual(QualityProgram program, string expected, string actual)
    {
        if (program.OutputKind == "Number") return NumberEqual(expected, actual);
        if (program.OutputKind == "Boolean")
        {
            string Normalize(string value) => value.Trim().ToUpperInvariant() switch { "TRUE" or "WAHR" => "true", "FALSE" or "FALSCH" => "false", _ => "" };
            return Normalize(expected).Length > 0 && Normalize(expected) == Normalize(actual);
        }
        return expected.Trim() == actual || program.OutputOptions.TryGetValue(actual, out string? label) && expected.Trim().Equals(label, StringComparison.OrdinalIgnoreCase);
    }
    /// <summary> Zahlenfelder akzeptieren äquivalente Dezimalwerte, aber keine geratenen Einheiten. </summary>
    public static bool NumberEqual(string expected, string actual)
    {
        bool Parse(string text, out decimal number) => decimal.TryParse(text.Trim().Replace(',', '.'), NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out number);
        return Parse(expected, out var first) && Parse(actual, out var second) && first == second;
    }
    /// <summary> Übersetzt ausschließlich die erklärten Text-/Wahrheitswertausgaben. </summary>
    public static string Display(QualityProgram program, string value) => program.OutputKind == "Boolean" ? value.ToLowerInvariant() switch { "true" => "WAHR", "false" => "FALSCH", _ => value } : program.OutputOptions.GetValueOrDefault(value, value);
}
