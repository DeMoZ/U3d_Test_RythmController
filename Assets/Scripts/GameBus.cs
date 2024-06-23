using System.Collections.Generic;

public class GameBus
{
    public Character Player { get; private set; }
    public List<Character> Bots { get; private set; } = new List<Character>();

    public void SetPlayer(Character character)
    {
        Player = character;
    }

    public void AddBot(Character character)
    {
        Bots.Add(character);
    }
}