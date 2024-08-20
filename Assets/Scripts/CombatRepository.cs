using System.Collections.Generic;
using System.Linq;

public interface ICombatRepository
{
    List<(int, int)> GetSequencesKeys();
    bool IsSequenceExists((int, int) code);

    float GetPreAttackTime((int, int) code);
    float GetAttackTime((int, int) code);
    float GetPostAttackTime((int, int) code);
    float GetFailTime((int, int) code);

    float GetPreBlockTime();
    float GetBlockTime();
    float GetPostBlockTime();
    float GetBlockFailTime();
}

public class CombatRepository : ICombatRepository
{
    private readonly CombatConfig _config;
    private readonly Dictionary<(int, int), AttackElement> _attacks;

    /// <summary>
    /// Config of timings transform to indexes (0,0), (0,1), (1,0), (1,1).
    /// By this codes as suffix the attack is found in the animator
    /// </summary>
    public CombatRepository(CombatConfig config)
    {
        _config = config;
        _attacks = new Dictionary<(int, int), AttackElement>();

        for (var i = 0; i < config.Sequences.Count; i++)
        {
            for (var j = 0; j < config.Sequences[i].Count; j++)
            {
                var element = config.Sequences[i][j];
                element.Init(i, j);
                _attacks[(i, j)] = element;
            }
        }
    }

    public List<(int, int)> GetSequencesKeys() => _attacks.Select(item => item.Key).ToList();
    private float GetDefaultPreAttackTime() => _config.PreAttackTime;
    private float GetDefaultAttackTime() => _config.AttackTime;
    private float GetDefaultPostAttackTime() => _config.PostAttackTime;
    private float GetDefaultAttackFailTime() => _config.AttackFailTime;

    private float GetDefaultPreBlockTime() => _config.PreBlockTime;
    private float GetDefaultBlockTime() => _config.BlockTime;
    private float GetDefaultPostBlockTime() => _config.PostBlockTime;
    private float GetDefaultBlockFailTime() => _config.BlockFailTime;

    public bool IsSequenceExists((int, int) code)
    {
        return _attacks.TryGetValue(code, out var element);
    }

    public bool TryGetSequence((int, int) code, out AttackElement element)
    {
        return _attacks.TryGetValue(code, out element);
    }

    public float GetPreAttackTime((int, int) code)
    {
        if (TryGetSequence(code, out var element))
            return element.PreAttackTime ?? GetDefaultPreAttackTime();

        throw new System.ArgumentOutOfRangeException();
    }

    public float GetAttackTime((int, int) code)
    {
        if (TryGetSequence(code, out var element))
            return element.AttackTime ?? GetDefaultAttackTime();

        throw new System.ArgumentOutOfRangeException();
    }

    public float GetPostAttackTime((int, int) code)
    {
        if (TryGetSequence(code, out var element))
            return element.PostAttackTime ?? GetDefaultPostAttackTime();

        throw new System.ArgumentOutOfRangeException();
    }

    public float GetFailTime((int, int) code)
    {
        if (TryGetSequence(code, out var element))
            return element.FailTime ?? GetDefaultAttackFailTime();

        throw new System.ArgumentOutOfRangeException();
    }

    public float GetPreBlockTime() => GetDefaultPreBlockTime();
    public float GetBlockTime() => GetDefaultBlockTime();
    public float GetPostBlockTime() => GetDefaultPostBlockTime();
    public float GetBlockFailTime() => GetDefaultBlockFailTime();
}
