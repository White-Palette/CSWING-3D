using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] Level;

    public int curLevel { private set; get; } = 0;

    private float curDelay = 0f;
    private float levelDelay = 90f;
    private bool doNotLoadNextLevel = false;

    private void Start()
    {
        Level[0].SetActive(true);
    }

    private void Update()
    {
        if (!doNotLoadNextLevel)
        {
            curDelay += Time.deltaTime;
            if (curLevel + 1 == 4 || curLevel + 1 == 9)
            {
                SurvivorModeManager survivorModeManager = Level[curLevel].GetComponent<SurvivorModeManager>();
                if (survivorModeManager.enemyList.Count == 0)
                {
                    nextLevel();
                }
            }
            else if (curLevel == 4 || curLevel == 9)
            {
                SurvivorModeManager survivorModeManager = Level[curLevel].GetComponent<SurvivorModeManager>();
                if (survivorModeManager.enemyList.Count == 0)
                {
                    nextLevel();
                    doNotLoadNextLevel = false;
                }
            }
            else
            {
                if (curDelay >= levelDelay)
                {
                    nextLevel();
                }
            }
        }
    }

    public void nextLevel()
    {
        curDelay = 0f;
        ++curLevel;

        Debug.Log((curLevel + 1) + "스테이지 시작");

        if (curLevel == 5)
        {
            levelDelay = 180f;
        }
        else
        {
            levelDelay = 90f;
        }

        Level[curLevel].SetActive(true);
    }
}
