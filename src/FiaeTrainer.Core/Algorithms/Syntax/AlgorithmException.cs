namespace FiaeTrainer.Core.Algorithms.Syntax;

/// <summary> Verständlicher Syntax- oder Ausführungsfehler mit Bezug zur betroffenen Zeile. </summary>
internal sealed class AlgorithmException(int line, string message) : Exception($"Zeile {line}: {message}");
