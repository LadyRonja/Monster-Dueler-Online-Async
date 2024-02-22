using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public enum RPSState { Awaiting_Play, Awaiting_Opponent, Awaiting_Move_Send }
public class RPSManager : MonoBehaviour
{
    private static RPSManager instance;
    public static RPSManager Instance { get => GetInstance(); private set => instance = value; }

    [SerializeField] Button soldierButton;
    [SerializeField] Button politicianButton;
    [SerializeField] Button assassinButton;
    [SerializeField] Button fireballButton;
    [SerializeField] Button counterSpellButton;
    List<MoveButton> moveButtons = new();

    public RPSMove currentMove;
    public RPSState currentMoveState = RPSState.Awaiting_Play;

    private void Awake()
    {
        #region Singleton
        if (instance == null || instance == this)
        {
            instance = this;
            soldierButton.onClick.AddListener(delegate { SelectMove(RPS.SOLDIER); });
            politicianButton.onClick.AddListener(delegate { SelectMove(RPS.POLITICIAN); });
            assassinButton.onClick.AddListener(delegate { SelectMove(RPS.ASSASSIN); });
            fireballButton.onClick.AddListener(delegate { SelectMove(RPS.FIREBALL); });
            counterSpellButton.onClick.AddListener(delegate { SelectMove(RPS.COUNTER_SPELL); });
            moveButtons = new()
            {
                soldierButton.GetComponent<MoveButton>(),
                politicianButton.GetComponent<MoveButton>(),
                assassinButton.GetComponent<MoveButton>(),
                fireballButton.GetComponent<MoveButton>(),
                counterSpellButton.GetComponent<MoveButton>()
            };

            DisableButtons();
        }
        else
            Destroy(this.gameObject);
        #endregion
    }

    private void Start()
    {
        RPSLoader.Instance.OnGameLoaded += DetermineMoveState;
    }

    public void DetermineMoveState()
    {
        if(RPSLoader.Instance.loadedGame == null)
        {
            DisableButtons();
            return;
        }

        if(RPSLoader.Instance.activePlayerMoves == null)
        {
            DisableButtons();
            return;
        }

        if (RPSLoader.Instance.loadedGame.gameIsOver)
        {
            DisableButtons();
            return;
        }

        if (RPSLoader.Instance.activePlayerMoves.Count > RPSLoader.Instance.opponentMoves.Count)
        {
            currentMoveState = RPSState.Awaiting_Opponent;
            DisableButtons();
            return;
        }
        
        if (currentMove == null)
        {
            currentMove = new RPSMove();
        }

        if (currentMove.selectedMove == RPS.UNSELECTED) { 
            currentMoveState = RPSState.Awaiting_Play;
            EnableButtons();
        }
        else
        {
            DisableButtons();
            currentMoveState = RPSState.Awaiting_Move_Send;
        }
    }

    public void SelectMove(RPS move)
    {
        DetermineMoveState();
        if (currentMoveState != RPSState.Awaiting_Play)
            return;

        DetermineMoveState();
        Debug.Log("Selected move");
        currentMove.selectedMove = move;
        UploadMove();
    }

    private async Task UploadMove()
    {
        RPSLoader.Instance.activePlayerMoves.Add(currentMove);
        string gameToUploadTo = "";
        if (RPSLoader.Instance.usingDebug)
            gameToUploadTo = RPSLoader.Instance.debugGame;
        else
            gameToUploadTo = RPSLoader.rpsGameToLoad;

        string gameDownloadBlob = await FirebaseLoader.LoadFromDatabase(DBPaths.RPS_TABLE, gameToUploadTo);
        RPSGame activeGame = JsonUtility.FromJson<RPSGame>(gameDownloadBlob);
        if (activeGame.playerA == ActiveUser.CurrentActiveUser.username)
            activeGame.playerAMoves = RPSLoader.Instance.activePlayerMoves;
        else
            activeGame.playerBMoves = RPSLoader.Instance.activePlayerMoves;

        string gameUploadBlob = JsonUtility.ToJson(activeGame);

        FirebaseSaver.SaveToDatabase(DBPaths.RPS_TABLE, gameToUploadTo, gameUploadBlob);
        RPSLoader.Instance.FetchGame();
        currentMove = null;
        DetermineMoveState();
        Debug.Log("Uploaded");
    }

    private void DisableButtons()
    {
        foreach (MoveButton mb in moveButtons)
        {
            mb.DisableButton();
        }
    }

    private void EnableButtons()
    {
        foreach (MoveButton mb in moveButtons)
        {
            mb.EnableButton();
        }
    }

    private void OnDestroy()
    {
        RPSLoader.Instance.OnGameLoaded -= DetermineMoveState;
    }

    private static RPSManager GetInstance()
    {
        if (instance != null)
            return instance;

        return new GameObject("Move Manager").AddComponent<RPSManager>();
    }
}
