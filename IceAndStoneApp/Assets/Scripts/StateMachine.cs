using UnityEngine;
using UnityEngine.UIElements;

public class StateMachine : MonoBehaviour
{
    #region Editor Fields

    [Header("Panels")]
    [SerializeField] private UIDocument welcomePanel;
    [SerializeField] private WelcomePanelController welcomeController;

    [SerializeField] private UIDocument teamSetupPanel;
    [SerializeField] private TeamSelectionController teamSelectionController;

    [SerializeField] private UIDocument playerEntryPanel;
    [SerializeField] private PlayerSetupController playerSetupController;

    [SerializeField] private UIDocument gameHudPanel;
    [SerializeField] private GameHudController gameHudController;

    [SerializeField] private UIDocument resultsPanel;
    [SerializeField] private ResultsController resultsController;

    [SerializeField] private int sessionNumber = 1;
    [SerializeField] private int gameNumber = 1;

    #endregion

    #region State

    private TeamPair stateTeams = new();
    private UiState currentState;

    public enum UiState
    {
        Welcome,
        TeamSetup,
        PlayerEntry,
        GameHud,
        Podium
    }

    private static readonly UiState[] flow =
    {
        UiState.Welcome,
        UiState.TeamSetup,
        UiState.PlayerEntry,
        UiState.GameHud,
        UiState.Podium
    };

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        HideAllPanels();
        GoToState(UiState.Welcome);
    }

    private void OnDestroy()
    {
        UnsubscribePanelEvents();
    }

    #endregion

    #region Public Navigation

    /// <summary>Goes to the next state in the flow.</summary>
    public void OnNextRequested() => GoToState(GetNextState(currentState));

    /// <summary>Goes to the next state and stores team data.</summary>
    public void OnNextRequestedWithData(TeamPair teamPair)
    {
        if (teamPair != null)
        {
            CopyTeam(teamPair.TeamA, stateTeams.TeamA);
            CopyTeam(teamPair.TeamB, stateTeams.TeamB);
        }
        GoToState(GetNextState(currentState));
    }

    /// <summary>Goes to the previous state in the flow.</summary>
    public void OnPreviousRequested() => GoToState(GetPreviousState(currentState));

    #endregion

    #region State Switching

    /// <summary>Switches active UI panel based on state.</summary>
    public void GoToState(UiState selectedState)
    {
        UnsubscribePanelEvents();
        HideAllPanels();

        switch (selectedState)
        {
            case UiState.Welcome:
                ShowUIDoc(welcomePanel);
                if (welcomeController == null && welcomePanel != null)
                    welcomeController = welcomePanel.GetComponent<WelcomePanelController>();
                if (welcomeController != null)
                    welcomeController.ProceedRequested += OnNextRequested;
                break;

            case UiState.TeamSetup:
                ShowUIDoc(teamSetupPanel);
                if (teamSelectionController == null && teamSetupPanel != null)
                    teamSelectionController = teamSetupPanel.GetComponent<TeamSelectionController>();
                if (teamSelectionController != null)
                {
                    teamSelectionController.SetInitialValues(stateTeams);
                    teamSelectionController.BackRequested += OnPreviousRequested;
                    teamSelectionController.NextRequested += OnNextRequestedWithData;
                }
                break;

            case UiState.PlayerEntry:
                ShowUIDoc(playerEntryPanel);
                if (playerSetupController == null && playerEntryPanel != null)
                    playerSetupController = playerEntryPanel.GetComponent<PlayerSetupController>();
                if (playerSetupController != null)
                {
                    playerSetupController.SetTeamInfo(stateTeams);
                    playerSetupController.BackRequested += OnPreviousRequested;
                    playerSetupController.NextRequested += OnNextRequestedWithData;
                }
                break;

            case UiState.GameHud:
                ShowUIDoc(gameHudPanel);
                if (gameHudController == null && gameHudPanel != null)
                    gameHudController = gameHudPanel.GetComponent<GameHudController>();
                if (gameHudController != null)
                {
                    gameHudController.SetTeams(stateTeams);
                    gameHudController.SetSessionAndGame(sessionNumber, gameNumber);
                    gameHudController.ResetScores();
                }
                SubscribeGlobalGameEventsForHud();
                break;

            case UiState.Podium:
                ShowUIDoc(resultsPanel);
                if (resultsController == null && resultsPanel != null)
                    resultsController = resultsPanel.GetComponent<ResultsController>();
                if (resultsController != null)
                {
                    var scoresA = gameHudController != null ? gameHudController.GetTeamARoundScores() : System.Array.Empty<int>();
                    var scoresB = gameHudController != null ? gameHudController.GetTeamBRoundScores() : System.Array.Empty<int>();
                    resultsController.SetResults(stateTeams, scoresA, scoresB);
                    resultsController.DoneRequested -= HandleResultsDone;
                    resultsController.TimedOut -= HandleResultsTimedOut;
                    resultsController.DoneRequested += HandleResultsDone;
                    resultsController.TimedOut += HandleResultsTimedOut;
                }
                break;
        }

        currentState = selectedState;
    }

    /// <summary>Unsubscribes events from all controllers.</summary>
    private void UnsubscribePanelEvents()
    {
        if (welcomeController != null)
            welcomeController.ProceedRequested -= OnNextRequested;

        if (teamSelectionController != null)
        {
            teamSelectionController.BackRequested -= OnPreviousRequested;
            teamSelectionController.NextRequested -= OnNextRequestedWithData;
        }

        if (playerSetupController != null)
        {
            playerSetupController.BackRequested -= OnPreviousRequested;
            playerSetupController.NextRequested -= OnNextRequestedWithData;
        }

        if (resultsController != null)
        {
            resultsController.DoneRequested -= HandleResultsDone;
            resultsController.TimedOut -= HandleResultsTimedOut;
        }

        UnsubscribeGlobalGameEvents();
    }

    #endregion

    #region Game Events

    /// <summary>Subscribes to game end events for HUD.</summary>
    private void SubscribeGlobalGameEventsForHud()
    {
        GameEvents.EndGameRequested -= HandleEndGameRequested;
        GameEvents.EndGameRequested += HandleEndGameRequested;
    }

    /// <summary>Unsubscribes from game end events.</summary>
    private void UnsubscribeGlobalGameEvents()
    {
        GameEvents.EndGameRequested -= HandleEndGameRequested;
    }

    /// <summary>Goes to podium screen when game ends.</summary>
    private void HandleEndGameRequested()
    {
        GoToState(UiState.Podium);
    }

    #endregion

    #region Helpers

    /// <summary>Copies a team into another.</summary>
    private static void CopyTeam(TeamModel source, TeamModel destination)
    {
        destination.Name = source.Name;
        destination.Colour = source.Colour;
        destination.HasFirstRound = source.HasFirstRound;
        destination.Players.Clear();
        if (source.Players != null)
            destination.Players.AddRange(source.Players);
    }

    /// <summary>Hides all UI panels.</summary>
    private void HideAllPanels()
    {
        SetActive(welcomePanel, false);
        SetActive(teamSetupPanel, false);
        SetActive(playerEntryPanel, false);
        SetActive(gameHudPanel, false);
        SetActive(resultsPanel, false);
    }

    /// <summary>Handles when results are confirmed.</summary>
    private void HandleResultsDone()
    {
        gameNumber += 1;
        stateTeams = new TeamPair();
        if (gameHudController != null)
            gameHudController.ResetScores();
        GoToState(UiState.Welcome);
    }

    /// <summary>Handles when results timeout expires.</summary>
    private void HandleResultsTimedOut()
    {
        HandleResultsDone();
    }

    /// <summary>Shows a UI panel.</summary>
    private static void ShowUIDoc(UIDocument doc) => SetActive(doc, true);

    /// <summary>Sets panel active state.</summary>
    private static void SetActive(UIDocument doc, bool value)
    {
        if (doc != null && doc.gameObject.activeSelf != value)
            doc.gameObject.SetActive(value);
    }

    /// <summary>Gets the next state.</summary>
    private static UiState GetNextState(UiState current)
    {
        int index = GetIndexOf(current);
        if (index < 0) return current;
        int nextIndex = index + 1;
        if (nextIndex >= flow.Length) return flow[0];
        return flow[nextIndex];
    }

    /// <summary>Gets the previous state.</summary>
    private static UiState GetPreviousState(UiState current)
    {
        int index = GetIndexOf(current);
        if (index < 0) return current;
        int prevIndex = index - 1;
        if (prevIndex < 0) return flow[flow.Length - 1];
        return flow[prevIndex];
    }

    /// <summary>Gets the index of a state in the flow.</summary>
    private static int GetIndexOf(UiState state)
    {
        for (int i = 0; i < flow.Length; i++)
        {
            if (flow[i] == state) return i;
        }
        return -1;
    }

    #endregion
}
