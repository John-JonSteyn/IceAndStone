using IceAndStone.App.Config;
using IceAndStone.App.Models;
using IceAndStone.App.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>Owns app state, team data, and round/score tracking. Subscribes to Control Panel events.</summary>
public class StateMachine : MonoBehaviour
{

    #region Singleton
    public static StateMachine Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Startup();
    }

    private void OnDestroy()
    {
        UnsubscribeControlPanelEvents();
        if (Instance == this) Instance = null;
    }
    #endregion

    #region Editor Fields
    [Header("Panels")]
    [SerializeField] private UIDocument _welcomePanel;
    [SerializeField] private UIDocument _teamSetupPanel;
    [SerializeField] private UIDocument _playerEntryPanel;
    [SerializeField] private UIDocument _gameHudPanel;
    [SerializeField] private UIDocument _resultsPanel;

    [SerializeField] private GameHudController _gameHudController;
    [SerializeField] private ResultsController _resultsController;
    #endregion

    #region Properties
    public enum UiState { Welcome, TeamSetup, PlayerEntry, GameHud, Podium }
    private UiState _current;

    private TeamPair _stateTeams = new();

    private readonly List<int> _teamARoundScores = new();
    private readonly List<int> _teamBRoundScores = new();
    private int _pendingScoreA;
    private int _pendingScoreB;
    private int _sessionNumber = 0;
    private int _gameNumber = 0;
    private long _gameId;
    private long _sessionId;

    public TeamPair Teams => _stateTeams;
    public int SessionNumber => _sessionNumber;
    public int GameNumber => _gameNumber;
    public long SessionId => _sessionId;
    public IReadOnlyList<int> GetTeamARoundScores() => _teamARoundScores.ToArray();
    public IReadOnlyList<int> GetTeamBRoundScores() => _teamBRoundScores.ToArray();
    #endregion

    private void Startup()
    {
        SubscribeControlPanelEvents();
        HideAllPanels();
        GoToState(UiState.Welcome);
    }

    #region Navigation
    public void GoToState(UiState selected)
    {
        HideAllPanels();

        switch (selected)
        {
            case UiState.Welcome:
                _sessionNumber++;
                ResetAllState();
                ShowUIDoc(_welcomePanel);
                break;

            case UiState.TeamSetup:
                try
                {
                    var startSession = ApiInterface.StartSessionAsync(ConfigManager.Current.LaneId);
                    _sessionId = startSession.Id;
                    Debug.Log($"Session Id: {_sessionId}");
                }
                catch (Exception ex)
                {
                    Debug.Log($"Error starting session. Message: {ex}");
                }
                ShowUIDoc(_teamSetupPanel);
                break;

            case UiState.PlayerEntry:
                ShowUIDoc(_playerEntryPanel);
                break;

            case UiState.GameHud:
                _gameNumber++;
                _ = StartGame();
                ResetScores();
                ShowUIDoc(_gameHudPanel);
                if (_gameHudController == null && _gameHudPanel != null)
                    _gameHudController = _gameHudPanel.GetComponent<GameHudController>();
                if (_gameHudController != null)
                    _gameHudController.RefreshFromState();
                break;

            case UiState.Podium:
                _ = EndGame();
                ShowUIDoc(_resultsPanel);
                break;
        }

        _current = selected;
    }

    public void GoToWelcome() => GoToState(UiState.Welcome);
    #endregion

    #region Events
    private void SubscribeControlPanelEvents()
    {
        UnsubscribeControlPanelEvents();
        GameEvents.AddScoreARequested += HandleAddScoreA;
        GameEvents.AddScoreBRequested += HandleAddScoreB;
        GameEvents.EndRoundRequested += HandleEndRound;
        GameEvents.EndGameRequested += HandleEndGame;
    }

    private void UnsubscribeControlPanelEvents()
    {
        GameEvents.AddScoreARequested -= HandleAddScoreA;
        GameEvents.AddScoreBRequested -= HandleAddScoreB;
        GameEvents.EndRoundRequested -= HandleEndRound;
        GameEvents.EndGameRequested -= HandleEndGame;
    }

    private void HandleAddScoreA(int score)
    {
        _pendingScoreA = Mathf.Max(0, score);
        if (_current == UiState.GameHud && _gameHudController != null)
            _gameHudController.RefreshFromState();
    }

    private void HandleAddScoreB(int score)
    {
        _pendingScoreB = Mathf.Max(0, score);
        if (_current == UiState.GameHud && _gameHudController != null)
            _gameHudController.RefreshFromState();
    }

    private void HandleEndRound()
    {
        _teamARoundScores.Add(_pendingScoreA);
        _teamBRoundScores.Add(_pendingScoreB);
        _pendingScoreA = 0;
        _pendingScoreB = 0;

        if (_current == UiState.GameHud && _gameHudController != null)
            _gameHudController.RefreshFromState();
    }

    private void HandleEndGame()
    {
        GoToState(UiState.Podium);
    }
    #endregion

    #region Mutators
    public void SetTeam(TeamPair teams)
    {
        if (teams == null) return;
        CopyTeam(teams.TeamA, _stateTeams.TeamA);
        CopyTeam(teams.TeamB, _stateTeams.TeamB);

        if (_current == UiState.GameHud && _gameHudController != null)
            _gameHudController.RefreshFromState();
    }

    public void ResetScores()
    {
        _teamARoundScores.Clear();
        _teamBRoundScores.Clear();
        _pendingScoreA = 0;
        _pendingScoreB = 0;

        if (_current == UiState.GameHud && _gameHudController != null)
            _gameHudController.RefreshFromState();
    }
    #endregion

    #region Utility
    private void ResetAllState()
    {
        _sessionId = 0;
        _gameId = 0;
        _gameNumber = 0;

        _stateTeams = new TeamPair();
        _teamARoundScores.Clear();
        _teamBRoundScores.Clear();
        _pendingScoreA = 0;
        _pendingScoreB = 0;

        if (_gameHudController != null)
            _gameHudController.RefreshFromState();
    }

    private static void CopyTeam(TeamModel source, TeamModel destination)
    {
        destination.Name = source.Name;
        destination.Colour = source.Colour;
        destination.HasFirstRound = source.HasFirstRound;
        destination.Players.Clear();
        if (source.Players != null) destination.Players.AddRange(source.Players);
    }

    private void HideAllPanels()
    {
        SetActive(_welcomePanel, false);
        SetActive(_teamSetupPanel, false);
        SetActive(_playerEntryPanel, false);
        SetActive(_gameHudPanel, false);
        SetActive(_resultsPanel, false);
    }

    private static void ShowUIDoc(UIDocument doc) => SetActive(doc, true);

    private static void SetActive(UIDocument doc, bool value)
    {
        if (doc != null && doc.gameObject.activeSelf != value)
            doc.gameObject.SetActive(value);
    }
    #endregion

    #region public API Calls
    public void EndSession()
    {
        ApiInterface.EndSessionAsync(StateMachine.Instance.SessionId);
    }

    public async Task StartGame()
    {
        if (_sessionId == 0)
            return;

        try
        {
            using var cts = new CancellationTokenSource(5000);
            var startResp = await ApiInterface.StartGameAsync(
                new StartGameRequest { SessionId = _sessionId, TargetRounds = null },
                cts.Token
            );
            startResp.EnsureSuccessStatusCode();

            var gameDto = await ApiInterface.ReadJsonAsync<GameResponse>(startResp.Content);
            if (gameDto == null)
            {
                Debug.LogError("[StateMachine] /games/start returned empty body.");
                return;
            }

            _gameId = gameDto.Id;
        }
        catch (Exception ex)
        {
            //Debug.LogError($"[StateMachine] Error starting game: {ex}");
            return;
        }

        try
        {
            using var cts = new CancellationTokenSource(5000);
            var createReq = new CreateTeamsRequest
            {
                GameId = _gameId,
                TeamAName = _stateTeams.TeamA.Name,
                TeamAColour = _stateTeams.TeamA.Colour.ToString(),
                TeamBName = _stateTeams.TeamB.Name,
                TeamBColour = _stateTeams.TeamB.Colour.ToString(),
                FirstRoundTeam = _stateTeams.TeamA.HasFirstRound ? "A"
                                 : _stateTeams.TeamB.HasFirstRound ? "B"
                                 : null
            };

            var pairHttp = await ApiInterface.CreateTeamsPairAsync(createReq, cts.Token);
            pairHttp.EnsureSuccessStatusCode();

            var pairDto = await ApiInterface.ReadJsonAsync<TeamPairResponseDto>(pairHttp.Content);
            if (pairDto?.Item1 == null || pairDto.Item2 == null)
            {
                Debug.LogError("[StateMachine] /teams/create-pair returned invalid body.");
                return;
            }

            _stateTeams.TeamA.Id = pairDto.Item1.Id;
            _stateTeams.TeamA.Name = string.IsNullOrWhiteSpace(pairDto.Item1.Name) ? _stateTeams.TeamA.Name : pairDto.Item1.Name;
            _stateTeams.TeamA.Colour = ParseColourOrDefault(pairDto.Item1.Colour, _stateTeams.TeamA.Colour);

            _stateTeams.TeamB.Id = pairDto.Item2.Id;
            _stateTeams.TeamB.Name = string.IsNullOrWhiteSpace(pairDto.Item2.Name) ? _stateTeams.TeamB.Name : pairDto.Item2.Name;
            _stateTeams.TeamB.Colour = ParseColourOrDefault(pairDto.Item2.Colour, _stateTeams.TeamB.Colour);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[StateMachine] Error creating teams: {ex}");
            return;
        }

        await AddPlayersForTeamAsync(_stateTeams.TeamA);
        await AddPlayersForTeamAsync(_stateTeams.TeamB);
    }

    private async Task EndGame()
    {
        if (_sessionId == 0 || _gameId == 0)
            return;

        var request = new EndGameRequest
        {
            GameId = _gameId
        };

        try
        {
            using var cancellationToken = new CancellationTokenSource(5000);
            var response = await ApiInterface.EndGameAsync(request, cancellationToken.Token);
            response.EnsureSuccessStatusCode();

            Debug.Log("[StateMachine] Game ended successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[StateMachine] Error ending game: {ex}");
        }
    }

    private static TeamColour ParseColourOrDefault(string colour, TeamColour fallback)
    {
        if (!string.IsNullOrWhiteSpace(colour) &&
            Enum.TryParse<TeamColour>(colour, true, out var parsed))
            return parsed;
        return fallback;
    }

    private async Task AddPlayersForTeamAsync(TeamModel team)
    {
        if (team.Id == 0 || team.Players == null || team.Players.Count == 0)
            return;

        var addPlayersRequest = new AddPlayersRequest
        {
            TeamId = team.Id,
            PlayerNames = new List<string>(team.Players)
        };

        try
        {
            using var cancellationToken = new CancellationTokenSource(5000);
            var url = await ApiInterface.AddPlayersAsync(addPlayersRequest, cancellationToken.Token);
            url.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[StateMachine] Error adding players for team '{team.Name}' (Id={team.Id}): {ex}");
        }
    }
    #endregion
}
