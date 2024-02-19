using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum MoveState { Awaiting_Play, Awaiting_Discard, Awaiting_Opponent, Awaiting_Move_Send}
public class MoveManager : MonoBehaviour
{
    private static MoveManager instance;
    public static MoveManager Instance { get => GetInstance(); private set => instance = value; }

    public DisplayCard cardDisplayer;
    public Button confirmButton;

    public GameMove currentMove;
    public MoveState currentMoveState = MoveState.Awaiting_Play; 

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    public void SetUpConfirmButton(Card card)
    {
        DetermineMoveState();
        confirmButton.gameObject.SetActive(true);
        confirmButton.interactable = false;
        confirmButton.onClick.RemoveAllListeners();
        TMP_Text buttonText = confirmButton.GetComponentInChildren<TMP_Text>();
        buttonText.text = "--";

        switch (currentMoveState)
        {
            case MoveState.Awaiting_Play:
                confirmButton.interactable = true;
                buttonText.text = "Play";
                confirmButton.onClick.AddListener(delegate () { SelectCardToPlay(card); });               
                break;
            case MoveState.Awaiting_Discard:
                if (!card.discardable) 
                    break;
                confirmButton.interactable = true;
                buttonText.text = "Discard";
                confirmButton.onClick.AddListener(delegate () { SelectCardToDiscard(card); });
                break;
            case MoveState.Awaiting_Opponent:
                break;
            case MoveState.Awaiting_Move_Send:
                break;
        }
    }

    public void DetermineMoveState()
    {
        if (GameLoader.Instance.activePlayerMoves.Count > GameLoader.Instance.opponentMoves.Count)
        {
            currentMoveState = MoveState.Awaiting_Opponent;
            return;
        }
        else if (currentMove == null)
        {
            currentMove = new GameMove();
        }

        if (currentMove.playedCard.id == 0)
            currentMoveState = MoveState.Awaiting_Play;
        else if (currentMove.discardedCard.id == 0)
            currentMoveState = MoveState.Awaiting_Discard;
        else
        {
            currentMoveState = MoveState.Awaiting_Move_Send;
            Debug.Log("played:" + currentMove.playedCard.id);
            Debug.Log("discarded:" + currentMove.discardedCard.id);
        }
    }

    private void SelectCardToPlay(Card cardToSelect)
    {
        int playedCardWithKey = cardToSelect.assignedID;
        CardKey copyOfPlayedCardKey = new();
        copyOfPlayedCardKey.id = playedCardWithKey;
        currentMove.playedCard = copyOfPlayedCardKey;
        foreach (CardKey key in GameLoader.Instance.activePlayerData.startHand)
        {
            if(key.id == playedCardWithKey)
            {
                GameLoader.Instance.activePlayerData.startHand.Remove(key);
                break;
            }
        }
        currentMove.discard.Add(copyOfPlayedCardKey);
        GameLoader.Instance.LoadPlayerData(GameLoader.Instance.activePlayerData, GameLoader.Instance.activePlayerRepresentor, true);
        confirmButton.gameObject.SetActive(false);
        cardDisplayer.gameObject.SetActive(false);
    }
    private void SelectCardToDiscard(Card cardToSelect)
    {
        int discardedCardWithKey = cardToSelect.assignedID;
        CardKey copyOfDiscardedCardKey = new();
        copyOfDiscardedCardKey.id = discardedCardWithKey;
        currentMove.discardedCard = copyOfDiscardedCardKey;
        foreach (CardKey key in GameLoader.Instance.activePlayerData.startHand)
        {
            if (key.id == discardedCardWithKey)
            {
                GameLoader.Instance.activePlayerData.startHand.Remove(key);
                break;
            }
        }
        currentMove.discard.Add(copyOfDiscardedCardKey);
        GameLoader.Instance.LoadPlayerData(GameLoader.Instance.activePlayerData, GameLoader.Instance.activePlayerRepresentor, true);

        confirmButton.gameObject.SetActive(false);
        cardDisplayer.gameObject.SetActive(false);

        // Send move Here
        UploadMove();
    }

    private async Task UploadMove()
    {
        GameLoader.Instance.activePlayerData.moves.Add(currentMove);
        string gameToUploadTo = "";
        if (GameLoader.Instance.usingDebug)
            gameToUploadTo = GameLoader.Instance.debugGame;
        else
            gameToUploadTo = GameLoader.gameIDToLoad;

        string gameDownloadBlob = await FirebaseLoader.LoadFromDatabase(DBPaths.GAMES_TABLE, gameToUploadTo);
        Game activeGame = JsonUtility.FromJson<Game>(gameDownloadBlob);
        GameData currentUserData = activeGame.gameDatas.Where(u => u.dataOwner == ActiveUser.CurrentActiveUser.username).ToList()[0];
        currentUserData.moves.Add(currentMove);
        string gameUploadBlob = JsonUtility.ToJson(activeGame);

        FirebaseSaver.SaveToDatabase(DBPaths.GAMES_TABLE, gameToUploadTo, gameUploadBlob);
        Debug.Log("Uploaded");
    }

    public Game ExecuteMoves(Game inGame)
    {
        Debug.Log("Executing moves");
        int movesToExecute = Math.Min(inGame.gameDatas[0].moves.Count, inGame.gameDatas[1].moves.Count);
        if (movesToExecute == 0)
        {
            Debug.Log("No moves to execute");
            return inGame;
        }

        // Determine who's moves are for the active player and vice versa
        GameData aData = inGame.gameDatas[0];
        GameData oData = inGame.gameDatas[1];

        if (aData.dataOwner != ActiveUser.CurrentActiveUser.username)
        {
            GameData swapStore = aData;
            aData = oData;
            oData = swapStore;
        }

        List<GameMove> activePlayerMoves = aData.moves;
        List<GameMove> opponentMoves = oData.moves;

        for (int i = 0; i < movesToExecute; i++)
        {
            // Set up the current moves
            GameMove aCurMove = activePlayerMoves[i];
            GameMove oCurMove = opponentMoves[i];

            GameObject aCardObj = Instantiate(GameLoader.Instance.cardPrefabDictionary[aCurMove.playedCard.id]);
            Card aCard = aCardObj.GetComponent<Card>();

            GameObject oCardObj = Instantiate(GameLoader.Instance.cardPrefabDictionary[oCurMove.playedCard.id]);
            Card oCard = oCardObj.GetComponent<Card>();

            // Determine order
            #region order
            int aInitiative = aCard.initiative;
            int oInitiative = oCard.initiative;

            Card firstToExecute = null;
            Card lastToExecute = null;
            if (aInitiative == oInitiative)
            {
                // If any initiativeTie equals 0, this is the first time the move executes and they need to have a tiebreaker assigned
                if(aCurMove.initiativeTie == 0)
                {
                    int aRand = UnityEngine.Random.Range(1, 10);
                    int oRand = UnityEngine.Random.Range(1, 10);
                    while(aRand == oRand) {
                        aRand = UnityEngine.Random.Range(1, 10);
                        oRand = UnityEngine.Random.Range(1, 10);
                    }

                    aCurMove.initiativeTie = aRand;
                    oCurMove.initiativeTie = oRand;
                }

                if(aCurMove.initiativeTie < oCurMove.initiativeTie)
                {
                    firstToExecute = aCard;
                    lastToExecute = oCard;
                }
                else
                {
                    firstToExecute = oCard;
                    lastToExecute = aCard;
                }
            }
            else if(aInitiative < oInitiative)
            {
                firstToExecute = aCard;
                lastToExecute = oCard;
            }
            else
            {
                firstToExecute = oCard;
                lastToExecute = aCard;
            }
            #endregion

            // Find respective players frontline monster
            List<Monster> aMonsters = aData.monsters;
            List<Monster> oMonsters = oData.monsters;
            if (i != 0)
            {
                aMonsters = activePlayerMoves[i - 1].monstersState;
                oMonsters = opponentMoves[i - 1].monstersState;
            }
            

            Monster aMonsterInFront = aMonsters.Where(m => m.myPosition == Position.Front).ToList()[0];
            Monster oMonsterInFront = oMonsters.Where(m => m.myPosition == Position.Front).ToList()[0];

            // Have the correct monster execute the correct move
            if (firstToExecute == aCard)
            {
                firstToExecute.ExecuteCard(aMonsterInFront, oMonsterInFront);
                lastToExecute.ExecuteCard(oMonsterInFront, aMonsterInFront);
            }
            else {
                firstToExecute.ExecuteCard(oMonsterInFront, aMonsterInFront);
                lastToExecute.ExecuteCard(aMonsterInFront, oMonsterInFront);
            }

            activePlayerMoves[i].monstersState = aMonsters;
            opponentMoves[i].monstersState = oMonsters;
        }

        return inGame;
    }

    private static MoveManager GetInstance()
    {
        if(instance != null) 
            return instance;

        return new GameObject("Move Manager").AddComponent<MoveManager>();
    }
}
