using System.Text.Json.Serialization;

namespace FiaeTrainer.Core.Learning;

/// <summary> Schützt die eigene Lösung beim Nachlesen und unterscheidet eine ausdrücklich begonnene Wiederholung. </summary>
public abstract class ReviewableProgress<TAnswer> where TAnswer : class
{
    private TAnswer? _previousDraft;
    /// <summary> Mindestens einmal ohne Hilfe gelöst. </summary>
    public bool Independent { get; set; }
    /// <summary> Geprüfte Kopie. Bei Altprofilen ist nur der letzte Entwurf bekannt. </summary>
    public TAnswer? IndependentSolution { get; set; }
    public bool RepeatInProgress { get; set; }
    [JsonIgnore] public bool CanReview => Independent && !RepeatInProgress;
    [JsonIgnore] public bool IsReading { get; private set; }
    [JsonIgnore] public bool ShowingSavedAnswer => CanReview && !IsReading;

    public void BeginReading(TAnswer current)
    {
        _previousDraft = current;
        IsReading = true;
    }

    public TAnswer ReturnFromReading(TAnswer current)
    {
        var answer = IndependentSolution ?? _previousDraft ?? current;
        _previousDraft = null;
        IsReading = false;
        return answer;
    }

    public void RememberIndependentSolution(TAnswer answer)
    {
        Independent = true;
        IndependentSolution = answer;
        RepeatInProgress = false;
    }
}
