using System.Text.Json;

namespace FiaeTrainer.Web.Components.Diagrams;

/// <summary> Begrenzter Verlauf unabhängiger Entwürfe innerhalb einer geöffneten Aufgabe. </summary>
public sealed class DiagramHistory<T> where T : class
{
    private readonly List<string> _states = [];
    private int _index = -1;
    /// <summary> Ein früherer Bearbeitungsstand liegt vor. </summary>
    public bool CanUndo => _index > 0;
    /// <summary> Ein zurückgenommener Stand liegt vor. </summary>
    public bool CanRedo => _index >= 0 && _index < _states.Count - 1;
    /// <summary> Beginnt bei einer anderen Aufgabe oder einem extern geladenen Entwurf neu. </summary>
    public void Reset(T value) { _states.Clear(); _index = -1; Record(value); }
    /// <summary> Verwirft den Vorwärtszweig erst bei einer tatsächlichen Änderung. </summary>
    public void Record(T value)
    {
        string state = JsonSerializer.Serialize(value);
        if (_index >= 0 && _states[_index] == state) return;
        if (CanRedo) _states.RemoveRange(_index + 1, _states.Count - _index - 1);
        _states.Add(state);
        if (_states.Count > 40) _states.RemoveAt(0);
        _index = _states.Count - 1;
    }
    /// <summary> Liefert eine unabhängige Kopie; gespeicherte Antworten bleiben unangetastet. </summary>
    public T? Move(int direction)
    {
        if (direction == -1 && CanUndo || direction == 1 && CanRedo) _index += direction;
        else return null;
        return JsonSerializer.Deserialize<T>(_states[_index]);
    }
}
