using System;
using System.Collections.Generic;

namespace IceAndStone.App.Models
{

    [Serializable]
    public enum TeamColour
    {
        Red,
        Orange,
        Yellow,
        Green,
        Blue,
        Purple
    }

    [Serializable]
    public sealed class TeamModel
    {
        public long Id;
        public string Name = string.Empty;
        public TeamColour Colour = TeamColour.Red;
        public bool HasFirstRound = false;
        public List<string> Players = new();
    }

    [Serializable]
    public sealed class TeamPair
    {
        public TeamModel TeamA = new();
        public TeamModel TeamB = new();
    }

    [Serializable]
    public sealed class CreateTeamsRequest
    {
        public long GameId;
        public string TeamAName = string.Empty;
        public string TeamAColour = string.Empty;
        public string TeamBName = string.Empty;
        public string TeamBColour = string.Empty;
        public string FirstRoundTeam;
    }

    [Serializable]
    public sealed class AddPlayersRequest
    {
        public long TeamId;
        public List<string> PlayerNames = new();
    }

    [Serializable]
    public sealed class TeamResponseDto
    {
        public long Id;
        public string Name = string.Empty;
        public string Colour = string.Empty;
    }

    [Serializable]
    public sealed class TeamPairResponseDto
    {
        public TeamResponseDto Item1;
        public TeamResponseDto Item2;
    }
}
