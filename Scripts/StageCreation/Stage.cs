/// File Name: Stage.cs
/// File Author(s): Lincoln Schroeder
/// File Purpose: This Serializable script is used for storing all necessary info to represent
/// a custom stage into files. Strictly used for holding data.
/// 
/// Date Last Updated: November 27, 2019

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

    /// <summary>
    /// Default constructor.
    /// </summary>
    public Stage()
    {
        boundary = new BoundaryMap(0, 0);
        spawnMultiplier = 1.2f;
        canPowerupsSpawn = true;
        name = "emptyStage";
    }

    /// <summary>
    /// Constructor which sets the boundary map to the desired map.
    /// </summary>
    /// <param name="boundaryMap">Desired boundary map.</param>
    public Stage(BoundaryMap boundaryMap)
    {
        this.boundary = new BoundaryMap(boundaryMap);
    }
}
