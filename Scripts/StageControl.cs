using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageControl : MonoBehaviour
{
    private PersistentControl persistentController;
    private BoundaryDrawer boundaryDrawer;

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
