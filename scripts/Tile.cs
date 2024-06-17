using Godot;

public partial class Tile : Node2D
{
    #region Enums
    public enum TileType
    {
        Empty = 1,
        Start,
        End,
        Blocker,
        Path
    }
    #endregion

    #region Variables
    private Color _colorGrey = new Color(128, 128, 128, 0.25f);
    private Color _colorRed = new Color(255, 0, 0, 0.85f);
    private Color _colorGreen = new Color(0, 128, 0, 0.85f);
    private Color _colorYellow = new Color(255, 255, 0, 0.85f);
    private Color _colorCyan = new Color(0, 255, 255, 0.85f);

    private TileType _tileType = TileType.Empty;
    private int _x = -1;
    private int _y = -1;

    private float _circleScaleEmpty = 0.05f;
    private float _circleScaleBlocker = 0.25f;
    private float _circleScalePath = 0.15f;
    private Vector2[] _triangleUp;
    private Vector2[] _triangleDown;
    #endregion

    #region Property
    public TileType Type { get { return _tileType; } }
    public int X { get { return _x; } }
    public int Y { get { return _y; } }
    #endregion

    #region Methods - Godot
    public override void _Ready()
    {
        _triangleUp = new Vector2[3];
        _triangleUp[0] = new Vector2(Constants.NODE_SIZE * 0.5f, 0);
        _triangleUp[1] = new Vector2(0, Constants.NODE_SIZE);
        _triangleUp[2] = new Vector2(Constants.NODE_SIZE, Constants.NODE_SIZE);

        _triangleDown = new Vector2[3];
        _triangleDown[0] = new Vector2(0, 0);
        _triangleDown[1] = new Vector2(Constants.NODE_SIZE, 0);
        _triangleDown[2] = new Vector2(Constants.NODE_SIZE * 0.5f, Constants.NODE_SIZE);
    }

    public override void _Draw()
    {
        switch (_tileType)
        {
            case TileType.Empty:
                {
                    DrawCircle(new Vector2(Constants.NODE_SIZE * 0.5f, Constants.NODE_SIZE * 0.5f), Constants.NODE_SIZE * _circleScaleEmpty, _colorGrey);
                    break;
                }
            case TileType.Start:
                {
                    DrawColoredPolygon(_triangleUp, _colorYellow);
                    break;
                }
            case TileType.End:
                {
                    DrawColoredPolygon(_triangleDown, _colorCyan);
                    break;
                }
            case TileType.Blocker:
                {
                    DrawCircle(new Vector2(Constants.NODE_SIZE * 0.5f, Constants.NODE_SIZE * 0.5f), Constants.NODE_SIZE * _circleScaleBlocker, _colorRed);
                    break;
                }
            case TileType.Path:
                {
                    DrawCircle(new Vector2(Constants.NODE_SIZE * 0.5f, Constants.NODE_SIZE * 0.5f), Constants.NODE_SIZE * _circleScalePath, _colorGreen);
                    break;
                }
        }
    }
    #endregion

    #region Methods - Local
    public void SetTileType(TileType type)
    {
        _tileType = type;
        this.QueueRedraw();
    }

    public void SetIndex(int x, int y)
    {
        _x = x;
        _y = y;
    }

    public string GetTypeString()
    {
        switch (_tileType)
        {
            case TileType.Empty:
                {
                    return "-";
                }
            case TileType.Start:
                {
                    return "S";
                }
            case TileType.End:
                {
                    return "E";
                }
            case TileType.Blocker:
                {
                    return "B";
                }
            case TileType.Path:
            default:
                {
                    return "-";
                }
        }
    }
    #endregion
}
