using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    public int currectScore;
    public UnityEvent onScoreUpdate;

    // Start is called before the first frame update
    void Start()
    {
        currectScore = 0;
    }

    public void AddScore(int score)
    {
        currectScore += score;
        onScoreUpdate.Invoke();
    }
}
