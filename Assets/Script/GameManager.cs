using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("#Game Control")]
    public float isLive;
    [Header("#Player Control")]
    public int health;
    public int maxhealth;

    void GameStart()
    {
        health = maxhealth;
    }
}
