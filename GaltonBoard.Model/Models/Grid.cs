namespace GaltonBoard.Model.Models;

public class Grid
{
    private Border _boardBorder;
    private Cell[,] _cells;
    public readonly int Rows = Constants.GridBoardRows;
    public readonly int Columns = Constants.GridBoardColumns;
    private double _cellWidth;
    private double _cellHeight;


    public Grid(Border boardBorder)
    {
        _boardBorder = boardBorder;
        CreateCells();
    }

    private void CreateCells()
    {
        _cells = new Cell[Rows + 1, Columns + 1];
        _cellWidth = _boardBorder.Width / Columns;
        _cellHeight = _boardBorder.Height / Rows;

        for (var i = 0; i < Rows + 1; i++)
        {
            for (var j = 0; j < Columns + 1; j++)
            {
                _cells[i, j] = new Cell();
            }
        }
    }

    public Cell[] GetCellAndNeighbors(int row, int column)
    {
        var cells = new List<Cell>();

        for (var i = row - 1; i <= row + 1; i++)
        {
            for (var j = column - 1; j <= column + 1; j++)
            {
                if (i >= 0 && i < _cells.GetLength(0) && j >= 0 && j < _cells.GetLength(1))
                {
                    cells.Add(_cells[i, j]);
                }
            }
        }

        return cells.ToArray();
    }

    public Cell GetCell(Vector position)
    {
        var row = (int)(position.Y / _cellHeight);
        var column = (int)(position.X / _cellWidth);

        return _cells[row, column];
    }

    public void AddParticle(Particle particle)
    {
        var cell = GetCell(particle.Position);
        cell.AddParticle(particle, particle.Type);
    }

    public void Clear()
    {
        var cells = _cells.Cast<Cell>();
        Parallel.ForEach(cells, cell => cell.Clear());
    }
}