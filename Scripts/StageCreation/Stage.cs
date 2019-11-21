using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stage
{
    private BoundaryMap boundary;
    public BoundaryMap Boundary { get { return boundary; } set { boundary = value; } }

    private float spawnMultiplier;
    public float SpawnMultiplier { get { return spawnMultiplier; } set { spawnMultiplier = value; } }

    private bool canPowerupsSpawn;
    public bool CanPowerupsSpawn { get { return canPowerupsSpawn; } set { canPowerupsSpawn = value; } }

    private string name;
    public string Name { get { return name; } set { name = value; } }

    public Stage()
    {
        boundary = new BoundaryMap(0, 0);
        spawnMultiplier = 1.2f;
        canPowerupsSpawn = true;
        name = "emptyStage";
    }

    public Stage(BoundaryMap boundaryMap)
    {
        this.boundary = new BoundaryMap(boundaryMap);
    }
}
