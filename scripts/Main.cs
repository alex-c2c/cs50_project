using Godot;
using System.Collections.Generic;
using System.IO;

public partial class Main : Node2D
{
    #region Enums
    public enum State
    {
        Idle = 1,
        SetStart,
        SetEnd,
        SetBlockers,
    }
    #endregion

    #region Exports
    [ExportGroup("Grid Size")]
    [Export] private int _colSize = 86;
    [Export] private int _rowSize = 53;

    [ExportGroup("UI - Start Tile")]
    [Export] private Label _labelStartTile;
    [Export] private Button _buttonStartTileSet;
    [Export] private Button _buttonStartTileClear;

    [ExportGroup("UI - End Tile")]
    [Export] private Label _labelEndTile;
    [Export] private Button _buttonEndTileSet;
    [Export] private Button _buttonEndTileClear;

    [ExportGroup("UI - Blocker Tiles")]
    [Export] private Button _buttonBlockerSet;
    [Export] private Button _buttonBlockerClear;

    [ExportGroup("UI - Clear Tiles")]
    [Export] private Button _buttonClearPath;
    [Export] private Button _buttonClearAll;

    [ExportGroup("UI - Start")]
    [Export] private Button _buttonStart;

    [ExportGroup("Dialogs")]
    [Export] private FileDialog _saveFileDialog;
    [Export] private FileDialog _loadFileDialog;
    #endregion

    #region Properties
    public int ColSize { get { return _colSize; } }
    public int RowSize { get { return _rowSize; } }
    #endregion

    #region Variables
    private Grid _grid = null;

    private bool _isMouseClicked = false;
    private bool _isMouseMoved = false;
    private Vector2I _currentGridIndex = new Vector2I(-1, -1);
    private Vector2I _emptyGridIndex = new Vector2I(-1, -1);
    private Rect2 _boundingRect;

    private State _state = State.Idle;

    private Tile _startTile = null;
    private Tile _endTile = null;
    private List<Tile> _blockers = new List<Tile>();
    private List<Tile> _path = new List<Tile>();
    #endregion

    #region Methods - Godot
    public override void _Ready()
    {
        _grid = GetNode<Grid>("Grid");
        _grid.Initialize(_colSize, _rowSize);

        _boundingRect = new Rect2(new Vector2(_grid.Position.X, _grid.Position.Y), new Vector2(Constants.NODE_SIZE * _colSize, Constants.NODE_SIZE * _rowSize));
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton)
        {
            if (eventMouseButton.Pressed)
            {
                _isMouseClicked = true;
                _isMouseMoved = false;
            }
            else
            {
                if (!_isMouseClicked)
                {
                    return;
                }

                Vector2I gridIndex = ConvertMousePosToGridIndex(eventMouseButton.Position);
                if (!gridIndex.Equals(_emptyGridIndex) && !_currentGridIndex.Equals(gridIndex))
                {
                    _currentGridIndex = gridIndex;
                    SetTile(_currentGridIndex);
                }

                _isMouseClicked = false;
                _isMouseMoved = false;
                _currentGridIndex = _emptyGridIndex;
            }
        }
        else if (@event is InputEventMouseMotion eventMouseMotion)
        {
            if (!_isMouseClicked)
            {
                return;
            }

            _isMouseMoved = true;

            Vector2I gridIndex = ConvertMousePosToGridIndex(eventMouseMotion.Position);
            if (!gridIndex.Equals(_emptyGridIndex) && !_currentGridIndex.Equals(gridIndex))
            {
                _currentGridIndex = gridIndex;
                SetTile(_currentGridIndex);
            }
        }
    }
    #endregion

    #region Methods - Local
    private bool IsMousePosWithinBoundingRect(Vector2 mousePos)
    {
        return mousePos.X >= _boundingRect.Position.X && mousePos.X <= _boundingRect.Position.X + _boundingRect.Size.X && mousePos.Y >= _boundingRect.Position.Y && mousePos.Y <= _boundingRect.Position.Y + _boundingRect.Size.Y;
    }

    private Vector2I ConvertMousePosToGridIndex(Vector2 mousePos)
    {
        if (!IsMousePosWithinBoundingRect(mousePos))
        {
            return _emptyGridIndex;
        }

        float calibrateX = mousePos.X - _grid.Position.X;
        float calibrateY = mousePos.Y - _grid.Position.Y;

        int gridX = (int)(calibrateX / Constants.NODE_SIZE);
        int gridY = (int)(calibrateY / Constants.NODE_SIZE);

        return new Vector2I(gridX, gridY);
    }

    private void SetTile(Vector2I gridIndex)
    {
        if (_state == State.Idle)
        {
            return;
        }

        Tile touchedTile = _grid.GetTile(gridIndex);
        if (touchedTile is null)
        {
            return;
        }

        switch (_state)
        {
            case State.SetStart:
                {
                    SetStartTile(touchedTile);
                    break;
                }
            case State.SetEnd:
                {
                    SetEndTile(touchedTile);
                    break;
                }
            case State.SetBlockers:
                {
                    if (touchedTile.Type == Tile.TileType.Empty)
                    {
                        AddBlocker(touchedTile);
                    }
                    else if (touchedTile.Type == Tile.TileType.Blocker)
                    {
                        RemoveBlocker(touchedTile);
                    }
                    break;
                }
        }
    }

    private void SetStartTile(Tile newTile)
    {
        if (newTile.Type != Tile.TileType.Empty)
        {
            return;
        }

        if (_startTile is not null)
        {
            _startTile.SetTileType(Tile.TileType.Empty);
        }

        newTile.SetTileType(Tile.TileType.Start);
        _startTile = newTile;

        UpdateLabels();
    }

    private void ClearStartTile()
    {
        if (_startTile is null)
        {
            return;
        }

        _startTile.SetTileType(Tile.TileType.Empty);
        _startTile = null;
    }

    private void SetEndTile(Tile newTile)
    {
        if (newTile.Type != Tile.TileType.Empty)
        {
            return;
        }

        if (_endTile is not null)
        {
            _endTile.SetTileType(Tile.TileType.Empty);
        }

        newTile.SetTileType(Tile.TileType.End);
        _endTile = newTile;

        UpdateLabels();
    }

    private void ClearEndTile()
    {
        if (_endTile is null)
        {
            return;
        }

        _endTile.SetTileType(Tile.TileType.Empty);
        _endTile = null;
    }

    private void AddBlocker(Tile newTile)
    {
        newTile.SetTileType(Tile.TileType.Blocker);
        _blockers.Add(newTile);
    }

    private void RemoveBlocker(Tile tile)
    {
        tile.SetTileType(Tile.TileType.Empty);
        _blockers.Remove(tile);
    }

    private void ClearBlockers()
    {
        if (_blockers is null || _blockers.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < _blockers.Count; i++)
        {
            Tile tile = _blockers[i];
            tile.SetTileType(Tile.TileType.Empty);
        }

        _blockers.Clear();
    }


    private void ClearPath()
    {
        if (_path is null || _path.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < _path.Count; i++)
        {
            Tile tile = _path[i];
            tile.SetTileType(Tile.TileType.Empty);
        }

        _path.Clear();
    }

    private void ClearAll()
    {
        ClearStartTile();
        ClearEndTile();
        ClearBlockers();
        ClearPath();
    }

    private void SetReturnPath(List<Vector2I> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            Vector2I pt = path[i];
            Tile tile = _grid.GetTile(pt);

            if (tile is not null && tile.Type == Tile.TileType.Empty)
            {
                tile.SetTileType(Tile.TileType.Path);
                _path.Add(tile);
            }
        }
    }

    private void StartVisualizer()
    {
        Vector2I startPt = new Vector2I(_startTile.X, _startTile.Y);
        Vector2I endPt = new Vector2I(_endTile.X, _endTile.Y);
        Vector2I[] blockerPts = new Vector2I[_blockers.Count];
        for (int i = 0; i < _blockers.Count; i++)
        {
            blockerPts[i] = new Vector2I(_blockers[i].X, _blockers[i].Y);
        }

        List<Vector2I> returnPath = Astar.StartPathFinding(_colSize, _rowSize, startPt, endPt, blockerPts);
        if (returnPath is null)
        {
            GD.Print($"Return path not found");
        }
        else
        {
            SetReturnPath(returnPath);
        }
    }

    private void UpdateLabels()
    {
        if (_startTile is null)
        {
            _labelStartTile.Text = $"Start Tile: [ , ]";
        }
        else
        {
            _labelStartTile.Text = $"Start Tile: [ {_startTile.X} , {_startTile.Y} ]";
        }

        if (_endTile is null)
        {
            _labelEndTile.Text = $"End Tile: [ , ]";
        }
        else
        {
            _labelEndTile.Text = $"End Tile: [ {_endTile.X} , {_endTile.Y} ]";
        }
    }

    private void UpdateControls()
    {
        switch (_state)
        {
            case State.Idle:
                {
                    _buttonStartTileSet.Text = "Set Start Tile";
                    _buttonEndTileSet.Text = "Set End Tile";
                    _buttonBlockerSet.Text = "Set Blockers";

                    _buttonStartTileSet.ButtonPressed = false;
                    _buttonEndTileSet.ButtonPressed = false;
                    _buttonBlockerSet.ButtonPressed = false;
                    break;
                }

            case State.SetStart:
                {
                    _buttonStartTileSet.Text = "Setting Start Tile...";
                    _buttonEndTileSet.Text = "Set End Tile";
                    _buttonBlockerSet.Text = "Set Blockers";

                    _buttonStartTileSet.ButtonPressed = true;
                    _buttonEndTileSet.ButtonPressed = false;
                    _buttonBlockerSet.ButtonPressed = false;
                    break;
                }

            case State.SetEnd:
                {
                    _buttonStartTileSet.Text = "Set Start Tile";
                    _buttonEndTileSet.Text = "Setting End Tile...";
                    _buttonBlockerSet.Text = "Set Blockers";

                    _buttonStartTileSet.ButtonPressed = false;
                    _buttonEndTileSet.ButtonPressed = true;
                    _buttonBlockerSet.ButtonPressed = false;
                    break;
                }

            case State.SetBlockers:
                {
                    _buttonStartTileSet.Text = "Set Start Tile";
                    _buttonEndTileSet.Text = "Set End Tile";
                    _buttonBlockerSet.Text = "Setting Blockers...";

                    _buttonStartTileSet.ButtonPressed = false;
                    _buttonEndTileSet.ButtonPressed = false;
                    _buttonBlockerSet.ButtonPressed = true;
                    break;
                }
        }
    }

    #endregion

    #region Methods - Signals
    private void _on_button_start_tile_set_pressed()
    {
        if (_state == State.SetStart)
        {
            _state = State.Idle;
        }
        else
        {
            _state = State.SetStart;
        }

        UpdateControls();
    }

    private void _on_button_start_tile_clear_pressed()
    {
        _state = State.Idle;

        ClearStartTile();

        UpdateLabels();
        UpdateControls();
    }

    private void _on_button_end_tile_set_pressed()
    {
        if (_state == State.SetEnd)
        {
            _state = State.Idle;
        }
        else
        {
            _state = State.SetEnd;
        }

        UpdateControls();
    }

    private void _on_button_end_tile_clear_pressed()
    {
        _state = State.Idle;

        ClearEndTile();

        UpdateLabels();
        UpdateControls();
    }

    private void _on_button_blocker_set_pressed()
    {
        if (_state == State.SetBlockers)
        {
            _state = State.Idle;
        }
        else
        {
            _state = State.SetBlockers;
        }

        UpdateControls();
    }

    private void _on_button_blocker_clear_pressed()
    {
        _state = State.Idle;

        ClearBlockers();

        UpdateControls();
    }

    private void _on_button_clear_path_pressed()
    {
        _state = State.Idle;

        ClearPath();

        UpdateControls();
    }

    private void _on_button_clear_all_pressed()
    {
        _state = State.Idle;

        ClearAll();

        UpdateLabels();
        UpdateControls();
    }

    private void _on_button_start_pressed()
    {
        _state = State.Idle;

        ClearPath();

        StartVisualizer();

        UpdateControls();
    }

    private void _on_save_file_dialog_file_selected(string filePath)
    {
        // overwrite existing file
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        List<string> nodeData = _grid.ConvertGridToData();

        using (StreamWriter outputFile = new StreamWriter(filePath))
        {
            foreach (string line in nodeData)
            {
                outputFile.WriteLine(line);
            }
        }
    }


    private void _on_button_save_pressed()
    {
        _saveFileDialog.Show();
    }

    private void _on_load_file_dialog_file_selected(string filePath)
    {
        List<string> inputData = new List<string>();
        using (StreamReader inputFile = new StreamReader(filePath))
        {
            string line;
            while ((line = inputFile.ReadLine()) != null)
            {
                inputData.Add(Utils.ReverseRLE(line));
            }
        }

        int rowSize = inputData.Count;
        int colSize = 0;

        if (rowSize > 0)
        {
            colSize = inputData[0].Length;
        }

        if (colSize != _colSize || rowSize != _rowSize)
        {
            GD.PrintErr($"ColSize({colSize} vs {_colSize}) or rowSize({rowSize} vs {_rowSize}) does not match. Aborting Load Data.");
            return;
        }

        ClearAll();

        _grid.ApplyDataToGrid(inputData);

        Tile startTile = _grid.GetStartTile();
        if (startTile is not null)
        {
            _startTile = startTile;
        }

        Tile endTile = _grid.GetEndTile();
        if (endTile is not null)
        {
            _endTile = endTile;
        }

        List<Tile> blockerTiles = _grid.GetTiles(Tile.TileType.Blocker);
        foreach (Tile tile in blockerTiles)
        {
            _blockers.Add(tile);
        }

        List<Tile> pathTiles = _grid.GetTiles(Tile.TileType.Path);
        foreach (Tile tile in pathTiles)
        {
            _path.Add(tile);
        }

        _state = State.Idle;
        UpdateControls();
        UpdateLabels();

    }

    private void _on_button_load_pressed()
    {
        _loadFileDialog.Show();
    }
    #endregion
}
