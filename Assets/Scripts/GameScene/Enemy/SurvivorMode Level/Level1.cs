using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1 : SurvivorModeManager
{

    private void OnEnable()
    {
        FixedPursuitSpawnCount = 5;
        smallPursuitSpawnCount = 3;
    }

    protected override IEnumerator SpawnPurusit()
    {
        return base.SpawnPurusit();
    }

    protected override void goNextLevel()
    {
        base.goNextLevel();
    }
}
