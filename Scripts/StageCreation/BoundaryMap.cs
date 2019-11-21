using UnityEngine;

[System.Serializable]
public class BoundaryMap
{
    private int width;
    private int height;
    private SingleSpaceType[,] gridRepresentation;
    private int playerSpawnPos_x;
    private int playerSpawnPos_y;

    /// <summary>
    /// Main constructor for BoundaryMap.
    /// </summary>
    /// <param name="width">Max number of sprites that can be placed horizontally.</param>
    /// <param name="height">Max number of sprites that can be placed vertically.</param>
    public BoundaryMap(int width, int height)
    {
        this.width = width;
        this.height = height;
        gridRepresentation = new SingleSpaceType[width, height];
        playerSpawnPos_x = (int)(5 * ((float)width / 2f));
        playerSpawnPos_y = (int)(5 * ((float)height / 2f));
    }

    public BoundaryMap(BoundaryMap copy)
    {
        width = copy.GetWidth();
        height = copy.GetHeight();
        gridRepresentation = new SingleSpaceType[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
                gridRepresentation[i, j] = copy.GetTypeAtPosition(i, j);
        }
        playerSpawnPos_x = copy.GetPlayerSpawnX();
        playerSpawnPos_y = copy.GetPlayerSpawnY();
    }

    /// <summary>
    /// Initializes the grid to be all empty spaces.
    /// </summary>
    private void InitializeGrid()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                gridRepresentation[i, j] = SingleSpaceType.EMPTY;
            }
        }
    }

    /// <summary>
    /// Updates the given grid position to the new SingleSpaceType.
    /// </summary>
    /// <param name="x">X-index position of the space to be updated.</param>
    /// <param name="y">Y-index position of the space to be updated.</param>
    /// <param name="newType">The SingleSpaceType that the space will be changed to.</param>
    public void UpdateSpace(int x, int y, SingleSpaceType newType)
    {
        if (newType == SingleSpaceType.PLAYER_SPAWN)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (gridRepresentation[i, j] == SingleSpaceType.PLAYER_SPAWN)
                        ResetSpace(i, j);
                }
            }
            playerSpawnPos_x = x * BoundaryDrawer.SPRITE_UNIT_WIDTH;
            playerSpawnPos_y = y * BoundaryDrawer.SPRITE_UNIT_WIDTH;
        }
        gridRepresentation[x, y] = newType;
    }

    /// <summary>
    /// Resets the given grid position to an EMPTY space.
    /// </summary>
    /// <param name="x">X-index position of the space to be reset.</param>
    /// <param name="y">Y-index position of the space to be reset.</param>
    public bool ResetSpace(int x, int y)
    {
        if (gridRepresentation[x, y] != SingleSpaceType.EMPTY)
        {
            gridRepresentation[x, y] = SingleSpaceType.EMPTY;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Getter for the width of the grid.
    /// </summary>
    /// <returns>Width of the grid.</returns>
    public int GetWidth()
    {
        return width;
    }

    /// <summary>
    /// Getter for the height of the grid.
    /// </summary>
    /// <returns>Height of the grid.</returns>
    public int GetHeight()
    {
        return height;
    }

    /// <summary>
    /// Provides the value of the space at the given grid position.
    /// </summary>
    /// <param name="x">X-index position of the space requested.</param>
    /// <param name="y">Y-index position of the space requested.</param>
    /// <returns>The SingleSpaceType represented at the given grid position.</returns>
    public SingleSpaceType GetTypeAtPosition(int x, int y)
    {
        return gridRepresentation[x, y];
    }
    
    public int GetPlayerSpawnX()
    {
        return playerSpawnPos_x;
    }

    public int GetPlayerSpawnY()
    {
        return playerSpawnPos_y;
    }
}
