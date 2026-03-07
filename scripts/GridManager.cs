using Godot;
using System;

public partial class GridManager : Node2D
{
    [Export] public int Rows = 5;
    [Export] public int Columns = 9;

    [Export] public int CellWidth = 128;
    [Export] public int CellHeight = 128;

    private Vector2I hoveredCell = new Vector2I(-1, -1); //Stores the currently hovered cell coordinates. Initialized to (-1, -1) to indicate that no cell is currently hovered.

    private bool[,] gridOccupied;

    public override void _Ready()
    {
        gridOccupied = new bool[Rows, Columns];

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
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            Vector2 mousePos = GetGlobalMousePosition();

            //Converting Mouse position to grid cell coordinates.
            //Also, Instead of Vector2, Vector2I is used to restrict the coordinates to integers, which is necessary for indexing the grid array.
            Vector2I cell = GetCellFromWorld(mousePos);

            if (IsCellOccupied(cell.X, cell.Y))
            {
                Vector2 spawnPos = GetWorldPositionFromCell(cell.X, cell.Y);
                GD.Print("Placed Tower at: " + spawnPos);
                SetCellOccupied(cell.X, cell.Y);
            }
            else
            {
                GD.Print("Cell already occupied!");
            }
        }
    }

    public Vector2I GetCellFromWorld(Vector2 worldPosition)  //Function for converting Mouse position to grid cell coordinates.
    {
        int column = (int)(worldPosition.X / CellWidth);  //(int) is called typecasting in C#. It converts the result of the division to an integer, effectively giving us the column index.
        int row = (int)(worldPosition.Y / CellHeight);

        return new Vector2I(row, column);
    }

    public Vector2 GetWorldPositionFromCell(int row, int column) //Funtion for converting grid cell coordinates back to world position, which is useful for placing towers at the center of the cell.
    {
        float x = column * CellWidth + CellWidth / 2;
        float y = row * CellHeight + CellHeight / 2;

        return new Vector2(x, y);
    }

    public bool IsCellOccupied(int row, int column) //Function to check if a cell is occupied or not. It returns true if the cell is free (not occupied) and false if it is occupied.
    {
        return !gridOccupied[row, column];
    }

    public void SetCellOccupied(int row, int column) //Function to mark a cell as occupied when a tower is placed. It sets the corresponding entry in the gridOccupied array to true, indicating that the cell is now occupied.
    {
        gridOccupied[row, column] = true;
    }
}