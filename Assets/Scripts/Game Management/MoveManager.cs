using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        foreach (CardKey key in GameLoader.Instance.activePlayerData.hand)
        {
            if(key.id == playedCardWithKey)
            {
                GameLoader.Instance.activePlayerData.hand.Remove(key);
                break;
            }
        }
        GameLoader.Instance.activePlayerData.discard.Add(copyOfPlayedCardKey);
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
        foreach (CardKey key in GameLoader.Instance.activePlayerData.hand)
        {
            if (key.id == discardedCardWithKey)
            {
                GameLoader.Instance.activePlayerData.hand.Remove(key);
                break;
            }
        }
        GameLoader.Instance.activePlayerData.discard.Add(copyOfDiscardedCardKey);
        GameLoader.Instance.LoadPlayerData(GameLoader.Instance.activePlayerData, GameLoader.Instance.activePlayerRepresentor, true);

        confirmButton.gameObject.SetActive(false);
        cardDisplayer.gameObject.SetActive(false);

        // Send move Here
    }

    private static MoveManager GetInstance()
    {
        if(instance != null) 
            return instance;

        return new GameObject("Move Manager").AddComponent<MoveManager>();
    }
}
