/// File Name: StageControl.cs
/// File Author(s): Lincoln Schroeder
/// File Purpose: In the game scene, this script controls spawning in the selected boundary 
/// map, and placing the character at the correct spawn point.
/// 
/// Date Last Updated: November 27, 2019

using UnityEngine;

public class StageControl : MonoBehaviour
{
    private PersistentControl persistentController;
    private BoundaryDrawer boundaryDrawer;

    /// <summary>
    /// Awake is called on the first frame of instantiation.
    /// </summary>
    private void Awake()
    {
        persistentController = GameObject.FindWithTag("Persistent").GetComponent<PersistentControl>();
        boundaryDrawer = GameObject.FindWithTag("StageHolder").GetComponent<BoundaryDrawer>();
        boundaryDrawer.SetMap(persistentController.SelectedStage.Boundary);
        boundaryDrawer.Editable = false;
        boundaryDrawer.DisplayMap();
        GameObject.FindWithTag("Player").transform.position = new Vector2(boundaryDrawer.GetMap().GetPlayerSpawnX(), 
            boundaryDrawer.GetMap().GetPlayerSpawnY());
    }
}
