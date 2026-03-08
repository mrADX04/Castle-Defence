using Godot;
using System;

public partial class GridManager : Node2D
{
    [Export] public int Rows = 5;
    [Export] public int Columns = 9;

    [Export] public int CellWidth = 128;
    [Export] public int CellHeight = 128;

    private Vector2I hoveredCell = new Vector2I(-1, -1); //Stores the currently hovered cell coordinates. Initialized to (-1, -1) to indicate that no cell is currently hovered.

    private Tower[,] towers;

    public override void _Ready()
    {
        towers = new Tower[Rows, Columns];

        QueueRedraw(); // draw debug grid, also a default method in Godot. It schedules a redraw of the node, which will call the _Draw() method. We use it here to ensure our grid is drawn when the scene starts.
    }


    //Is a default method in Godot that is called every frame. Here, we use it to update the hovered cell based on the mouse position.
    //It checks if the mouse has moved to a different cell and triggers a redraw if necessary to update the visual feedback for the hovered cell.
    public override void _Process(double delta)
    {
        Vector2 mousePos = GetGlobalMousePosition();
        Vector2I newCell = GetCellFromWorld(mousePos);

        //Tracks mouse movement across the grid & updates the hovered cell accordingly. If the mouse moves to a different cell, it updates the hoveredCell variable and calls QueueRedraw() to update the visual feedback.
        // Checks if inside grid
        if (newCell.X >= 0 && newCell.X < Rows &&
            newCell.Y >= 0 && newCell.Y < Columns)
        {
            if (newCell != hoveredCell)
            {
                hoveredCell = newCell;
                QueueRedraw();
            }
        }
        else
        {
            if (hoveredCell != new Vector2I(-1, -1))
            {
                hoveredCell = new Vector2I(-1, -1);
                QueueRedraw();
            }
        }
    }

    //public override void _Draw()  //is a default method in Godot. Often used for custom drawing. Here, we use it to draw a grid for debugging purposes. It is used with QueueRedraw() to trigger the drawing process.
    //{
    //    for (int r = 0; r < Rows; r++)
    //    {
    //        for (int c = 0; c < Columns; c++)
    //        {
    //            Vector2 pos = new Vector2(c * CellWidth, r * CellHeight);

    //            DrawRect(
    //                new Rect2(pos, new Vector2(CellWidth, CellHeight)),
    //                Colors.Blue,
    //                false
    //            );
    //        }
    //    }
    //}

    public override void _Draw()  //Default method in Godot. Often used for custom drawing. Here, we use it to draw a grid for debugging purposes. It is used with QueueRedraw() to trigger the drawing process.
    {
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                Vector2 pos = new Vector2(c * CellWidth, r * CellHeight);

                Rect2 rect = new Rect2(pos, new Vector2(CellWidth, CellHeight));

                DrawRect(rect, Colors.Green, false);

                if (hoveredCell.X == r && hoveredCell.Y == c)
                {
                    DrawRect(rect, new Color(0, 1, 0, 0.3f), true);
                }
            }
        }
    }

    public override void _Input(InputEvent @event)  //detect mouse clicks to place towers(or Units)
    {
        if (@event is InputEventMouseButton mouseEvent
            && mouseEvent.Pressed
            && mouseEvent.ButtonIndex == MouseButton.Left) //This detects only left mouse button clicks. Not right clicks or other mouse buttons.
        {
            Vector2 mousePos = GetGlobalMousePosition();

            //Converting Mouse position to grid cell coordinates.
            //Also, Instead of Vector2, Vector2I is used to restrict the coordinates to integers, which is necessary for indexing the grid array.
            Vector2I cell = GetCellFromWorld(mousePos);

            if (!IsInsideGrid(cell)) //Checks if the clicked cell is within the grid boundaries. If not, it ignores the click to prevent errors.
                return;

            if (!IsCellOccupied(cell))
            {
                Vector2 spawnPos = GetWorldPositionFromCell(cell);

                Tower tower = new Tower();
                tower.Position = spawnPos;
                AddChild(tower);  //This adds the tower to the scene tree in Godot, making it a child of the GridManager node. This is necessary for the tower to be rendered and updated in the game

                PlaceTower(cell, tower);

                GD.Print("Placed Tower at: " + spawnPos);
            }
            else
            {
                GD.Print("Tile already occupied!"); //Here cell refers to tile in game.
            }
        }
    }

    public Vector2I GetCellFromWorld(Vector2 worldPosition)  //Function for converting Mouse position to grid cell coordinates.
    {
        int column = (int)(worldPosition.X / CellWidth);  //(int) is called typecasting in C#. It converts the result of the division to an integer, effectively giving us the column index.
        int row = (int)(worldPosition.Y / CellHeight);

        return new Vector2I(row, column);
    }

    public Vector2 GetWorldPositionFromCell(Vector2I cell) //Funtion for converting grid cell coordinates back to world position, which is useful for placing towers at the center of the cell.
    {
        float x = cell.Y * CellWidth + CellWidth / 2; // column -> cell.Y
        float y = cell.X * CellHeight + CellHeight / 2; // row -> cell.X

        return new Vector2(x, y);
    }

    public bool IsCellOccupied(Vector2I cell) //Function to check if a cell is occupied or not. It returns true if the cell is free (not occupied) and false if it is occupied.
    {
        return towers[cell.X, cell.Y] != null;
    }

    public void PlaceTower(Vector2I cell, Tower tower) //This Function is responsible for placing a tower in the grid. It takes the row and column indices along with the tower instance and updates the towers array to mark that cell as occupied by the tower.
    {
        towers[cell.X, cell.Y] = tower;
    }

    public bool IsInsideGrid(Vector2I cell) //Function to prevent crashes when clicking outside grid. It checks if the given row and column indices are within the bounds of the grid defined by Rows and Columns.
    {
        return cell.X >= 0 && cell.X < Rows &&
           cell.Y >= 0 && cell.Y < Columns;
    }
}