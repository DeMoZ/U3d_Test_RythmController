using DMZ.Events;

namespace Attack3x3
{
    public class Attack3x3PlayerData
    {
        public readonly DMZState<Attack3x3State> AttackSequenceState = new(Attack3x3State.Idle);
        public readonly DMZState<AttackProgressData> AttackProgress = new();
        public DMZState<(int, int)> CurrentSequenceKey = new((-1, -1));

        public class AttackProgressData
        {
            public Attack3x3State State { get; }
            public (int, int) CurrentSequenceKey { get; }
            public float Progress { get; }
            public float Progress01 { get; }

            public AttackProgressData(Attack3x3State state, (int, int) currentSequenceKey, float progress,
                float progress01)
            {
                State = state;
                CurrentSequenceKey = currentSequenceKey;
                Progress = progress;
                Progress01 = progress01;
            }
        }
    }
}