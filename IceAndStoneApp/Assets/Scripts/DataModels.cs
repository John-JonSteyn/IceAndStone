using System;
using System.Collections.Generic;

[Serializable]
public enum TeamColour { Red, Orange, Yellow, Green, Blue, Purple }

[Serializable]
public class TeamModel
{
    public string Name = "";
    public TeamColour Colour = TeamColour.Red;
    public bool HasFirstRound = false;
    public List<string> Players = new();
}

[Serializable]
public class TeamPair
{
    public TeamModel TeamA = new();
    public TeamModel TeamB = new();
}
