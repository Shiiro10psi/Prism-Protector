using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    int health = 10;
    public int score = 0;
    public int lasersSurvived = 0;
    
    public void ResetState()
    {
        health = 10;
        score = 0;
        lasersSurvived = 0;
    }

    public void DamageHealth(int i)
    {
        health -= i;
        if (health <= 0)
        {
            health = 0;
            GameOver();
        }
    }

    public int GetHealth() { return health; }

    public void AddScore(int i)
    {
        score += i;
        Debug.Log(i + "," + score);
    }

    public int GetScore() { return score; }

    public void LaserSurvived() { lasersSurvived++; }

    public int GetLasers() { return lasersSurvived; }

    void GameOver()
    {
        FindObjectOfType<HeartPiece>().Break();
        FindObjectOfType<GameStateUI>().GameOver();
    }

   
}
