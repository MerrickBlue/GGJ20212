using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When both players gets into this object the game is won
/// </summary>
public class FinalGoal : MonoBehaviour
{
    [SerializeField] protected bool soulPlayerHere;
    [SerializeField] protected bool bodyPlayerHere;

    protected void OnTriggerEnter(Collider other)
    {
        //If the other object is not one of the player we just exit this function.
        if (other.tag != "Player")
        {
            return;
        }

        //Now we try to get the soul controller and if it's there we set the correct boolean to true.
        var soulController = other.gameObject.GetComponent<CTRL_Soul>();
        if (soulController != null)
        {
            soulPlayerHere = true;            
        }

        var bodyController = other.gameObject.GetComponent<PlayerLogic>();
        if (soulController != null)
        {
            bodyPlayerHere = true;
        }

        //Finally we check the win condition and if met we call the right function.
        CheckWinCondition();
    }

    protected void CheckWinCondition()
    {
        if (soulPlayerHere && bodyPlayerHere)
        {
            GC_SoulsSpawner.instance.WinGame();
        }
    }
}
