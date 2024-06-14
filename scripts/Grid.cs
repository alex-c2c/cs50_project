using Godot;

public partial class Grid : Node2D
{
    #region Variables
    private PackedScene _tileScene;
    private CanvasGroup _canvasGroup;

    private bool _isInitialized = false;
    private int _colSize = 0;
    private int _rowSize = 0;

    private Tile[,] _tileArray = null;

    private Color _colorGrey = new Color("414042");

    #endregion

    #region Methods - Godot
    public override void _Ready()
    {
        _tileScene = ResourceLoader.Load(ProjectSettings.GlobalizePath("res://scenes/Tile.tscn")) as PackedScene;

        _canvasGroup = GetNode<CanvasGroup>("CanvasGroup");
    }

    public override void _Draw()
    {
        if (!_isInitialized)
        {
            return;
        }

        for (int y = 0; y < _rowSize + 1; y++)
        {
            for (int x = 0; x < _colSize + 1; x++)
            {
                DrawLine(new Vector2(Constants.NODE_SIZE * x, 0), new Vector2(Constants.NODE_SIZE * x, Constants.NODE_SIZE * _rowSize), _colorGrey, 2, false);
                DrawLine(new Vector2(0, Constants.NODE_SIZE * y), new Vector2(Constants.NODE_SIZE * _colSize, Constants.NODE_SIZE * y), _colorGrey, 2, false);
            }
        }
    }
    #endregion

    #region Methods - Local
    public void Initialize(int colSize, int rowSize)
    {
        _isInitialized = true;
        _colSize = colSize;
        _rowSize = rowSize;

        _tileArray = new Tile[_colSize, _rowSize];

        for (int y = 0; y < _rowSize; y++)
        {
            for (int x = 0; x < _colSize; x++)
            {
                Tile newTile = _tileScene.Instantiate() as Tile;
                newTile.Name = $"Tile_{x:D2}_{y:D2}";
                newTile.Position = new Vector2(x * Constants.NODE_SIZE, y * Constants.NODE_SIZE);
                newTile.SetIndex(x, y);

                _canvasGroup.AddChild(newTile);

                _tileArray[x, y] = newTile;
            }
        }
    }

    private bool IsGridIndexOutOfBounds(Vector2I gridIndex)
    {
        return gridIndex.X < 0 || gridIndex.X >= _colSize || gridIndex.Y < 0 || gridIndex.Y >= _rowSize;
    }

    public Tile GetTile(Vector2I gridIndex)
    {
        if (IsGridIndexOutOfBounds(gridIndex))
        {
            return null;
        }

        return _tileArray[gridIndex.X, gridIndex.Y];
    }

    public void SetTileType(Vector2I gridIndex, Tile.TileType type)
    {
        if (IsGridIndexOutOfBounds(gridIndex))
        {
            return;
        }

        _tileArray[gridIndex.X, gridIndex.Y].SetTileType(type);
    }
    #endregion
}
