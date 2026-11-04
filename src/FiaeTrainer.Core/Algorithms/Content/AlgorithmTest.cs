using System.Text.Json;

namespace FiaeTrainer.Core.Algorithms.Content;

/// <summary> Fachlich festgelegter Testfall; die Sollausgabe wird nicht aus der Nutzerlösung abgeleitet. </summary>
public sealed record AlgorithmTest(string Name, Dictionary<string, JsonElement> Inputs, string[] Output);
