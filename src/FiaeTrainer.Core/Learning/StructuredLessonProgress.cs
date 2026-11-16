namespace FiaeTrainer.Core.Learning;

/// <summary> Gemeinsamer Lernversuch für bearbeitbare Modelle, Auswahlen, Testfälle und Pläne. </summary>
public abstract class StructuredLessonProgress<TAnswer> : ReviewableProgress<TAnswer> where TAnswer : class, new()
{
    public TAnswer Answer { get; set; } = new();
    public bool IndependentMode { get; set; } = true;
    public bool Assisted { get; set; }
    public int HintLevel { get; set; }
    public bool SolutionViewed { get; set; }
    public int Attempts { get; set; }
    public bool Completed { get; set; }
    protected abstract TAnswer Copy(TAnswer answer);

    public void OpenSavedAnswer()
    {
        if (!CanReview) return;
        IndependentMode = true;
        if (IndependentSolution != null) Answer = Copy(IndependentSolution);
    }

    public void UseGuidance()
    {
        if (CanReview)
        {
            BeginReading(Answer);
            Answer = Copy(Answer);
        }
        IndependentMode = false;
        Assisted = true;
    }

    public void ResumeAnswer() => ResetEditor(Copy(ReturnFromReading(Answer)));

    public void StartAgain(TAnswer initial)
    {
        RepeatInProgress = Independent;
        ResetEditor(initial);
    }

    private void ResetEditor(TAnswer answer)
    {
        Answer = answer;
        IndependentMode = true;
        Assisted = false;
        HintLevel = 0;
        SolutionViewed = false;
    }

    public void RecordEvaluation(bool correct)
    {
        Attempts++;
        Completed |= correct;
        if (correct && IndependentMode && !Assisted) RememberIndependentSolution(Copy(Answer));
    }
}
