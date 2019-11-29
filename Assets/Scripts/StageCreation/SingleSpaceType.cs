/// File Name: SingleSpaceType.cs
/// File Author(s): Lincoln Schroeder
/// File Purpose: Identifies the type of spaces that can be occupied in a stage. Each
/// type has a cooresponding sprite for displaying the boundary map.
/// 
/// Date Last Updated: November 12, 2019

public enum SingleSpaceType
{
    EMPTY,
    VERTICAL_STRAIGHT,
    HORIZONTAL_STRAIGHT,
    CORNER_1Q,
    CORNER_2Q,
    CORNER_3Q,
    CORNER_4Q,
    PLAYER_SPAWN
}
