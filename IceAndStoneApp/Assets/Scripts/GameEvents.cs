using System;

public static class GameEvents
{
    public static event Action<int> AddScoreARequested;
    public static event Action<int> AddScoreBRequested;
    public static event Action EndRoundRequested;
    public static event Action EndGameRequested;

    public static void RaiseAddScoreA(int value) => AddScoreARequested?.Invoke(value);
    public static void RaiseAddScoreB(int value) => AddScoreBRequested?.Invoke(value);
    public static void RaiseEndRound() => EndRoundRequested?.Invoke();
    public static void RaiseEndGame() => EndGameRequested?.Invoke();
}

