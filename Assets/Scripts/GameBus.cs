using System.Collections.Generic;

public class GameBus
{
    public ITargetable Player { get; private set; }
    public List<ITargetable> Bots { get; private set; } = new();

    public void SetPlayer(ITargetable character)
    {
        Player = character;
    }

    public void AddBot(ITargetable character)
    {
        Bots.Add(character);
    }
}