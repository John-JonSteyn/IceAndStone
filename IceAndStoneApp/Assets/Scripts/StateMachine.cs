using System.Collections.Generic;
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

    public TeamPair Teams => _stateTeams;
    public int SessionNumber => _sessionNumber;
    public int GameNumber => _gameNumber;
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
                _gameNumber = 0;
                ResetAllState();
                ShowUIDoc(_welcomePanel);
                break;

            case UiState.TeamSetup:
                ShowUIDoc(_teamSetupPanel);
                break;

            case UiState.PlayerEntry:
                ShowUIDoc(_playerEntryPanel);
                break;

            case UiState.GameHud:
                _gameNumber++;
                ResetScores();
                ShowUIDoc(_gameHudPanel);
                if (_gameHudController == null && _gameHudPanel != null)
                    _gameHudController = _gameHudPanel.GetComponent<GameHudController>();
                if (_gameHudController != null)
                    _gameHudController.RefreshFromState();
                break;

            case UiState.Podium:
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
}
