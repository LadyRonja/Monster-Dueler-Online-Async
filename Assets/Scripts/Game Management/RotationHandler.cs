using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationHandler : MonoBehaviour
{
    private static RotationHandler instance;
    public static RotationHandler Instance { get => GetInstance(); private set => instance = value; }

    public List<Monster> activeUserMonsters = new();
    public List<Monster> opponentMonsters = new();

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    private static RotationHandler GetInstance() 
    { 
        if (instance != null)
            return instance;

        instance = new GameObject("Rotation Handler").AddComponent<RotationHandler>();
        return instance;
    }

    public void RotateTeam(Monster monsterOnTeam, bool rotateCounterClockwise)
    {
        List<Monster> teamToRotate = new();

        // Find what team to rotate
        if (activeUserMonsters.Contains(monsterOnTeam))
            teamToRotate = activeUserMonsters;
        else if (opponentMonsters.Contains(monsterOnTeam))
            teamToRotate = opponentMonsters;
        else
            return;


        // Ensure at least 2 mons are alive
        int aliveMons = 0;
        Monster frontingMonster = null; // relevant for later
        foreach (Monster monster in teamToRotate)
        {
            if(monster.alive)
                aliveMons++;

            if(monster.myPosition == Position.Front)
                frontingMonster = monster;

        }

        if (aliveMons < 2)
            return;

        // Rotate at least once, and don't stop if the monster in front is not alive
        do
        {
            for (int i = 0; i < teamToRotate.Count; i++)
            {
                if(rotateCounterClockwise)
                {
                    if (teamToRotate[i].myPosition == Position.Front) teamToRotate[i].myPosition = Position.BackLeft;
                    if (teamToRotate[i].myPosition == Position.BackLeft) teamToRotate[i].myPosition = Position.BackRight;
                    if (teamToRotate[i].myPosition == Position.BackRight) teamToRotate[i].myPosition = Position.Front;
                }
                else
                {
                    if (teamToRotate[i].myPosition == Position.Front) teamToRotate[i].myPosition = Position.BackRight;
                    if (teamToRotate[i].myPosition == Position.BackRight) teamToRotate[i].myPosition = Position.BackLeft;
                    if (teamToRotate[i].myPosition == Position.BackLeft) teamToRotate[i].myPosition = Position.Front;
                }

            }
        } while (frontingMonster.alive == false);

        // Update monster displays here
        Debug.Log("Monsters have rotated, please implement visual update for this");
    }
}
