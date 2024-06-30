public class CombatProgressModel
{
    public CombatPhase State { get; }
    public (int, int) CurrentSequenceKey { get; }
    public float Progress { get; }
    public float Progress01 { get; }

    public CombatProgressModel(CombatPhase state, (int, int) currentSequenceKey, float progress,
        float progress01)
    {
        State = state;
        CurrentSequenceKey = currentSequenceKey;
        Progress = progress;
        Progress01 = progress01;
    }
}