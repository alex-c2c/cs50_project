# A* Path Finding Visualizer using Godot / C#
## Video Demo
[Youtube](https://youtu.be/dLGfaZqJ7Eg)
## What is this?
A Godot C# project to practise A* path finding algorithm + other data structures. Created using Godot v4.2.1.
## How to use
1) Download and install Godot Engine - .NET.
2) Open Godot and import this project.
3) Build & Run.
## What does this do?
### When Run...
Creates a grid with X by Y tiles, with control buttons on the left.
- X & Y values are modifiable under the inspector.
- Currently set at 86 x 53 to fit in nicely into a 1920x1080 screen.
### Button: Set Start Tile
Switches into a `Set Start Tile` state. Allows user to select a starting point within the grid.
- `Click` on any `Empty Tile` to make it into the starting point.
- `Click and Drag` to quickly shift the starting point around the grid.
### Button: Clear Start Tile
Allows user to reset `Start Tile` into `Empty Tile`.
### Button: Set End Tile
Switches into a `Set End Tile` state. Allows to select an ending point within the grid.
- `Click` on any `Empty Tile` to make it into the ending point.
- `Click and Drag` to quickly shift the ending point around the grid.
### Button: Clear End Tile
Allows user to reset `End Tile` into `Empty Tile`.
### Button: Set Blockers
Switches into a `Set Blockers` state. Allows user to "draw" obstacles around the grid.
- `Click` on any `Empty Tile` to set selected tile as a `Blocker Tile`.
- `Click` on any `Blocker Tile` to set it back into an `Empty Tile`.
- `Click and Drag` to quickly set tiles under cursor as `Blocker Tile` or `Empty Tile`.
### Button: Clear Blockers
Allows user to reset ALL `Blocker Tile` into `Empty Tile`.
### Button: Clear Path
Allows user to reset ALL `Path Tile` into `Empty Tile`.
### Button: Clear All
Allows user to reset ALL tiles into `Empty Tile`.
### Button: Start Visualizer
- if either `Start Tile` or `End Tile` is not set, show throw a warning.
- Executes A* path finding algorithm.
- If return path is found, set relevant tiles as `Path Tile`.
- if return path is NOT found, show a message.
### Button: Save Nodes
Allows user to save current tile states (whole grid) into a file.
- Opens a file dialog.
- Saved data is `Run length Encoded`.
### Button: Load Nodes
Allows user to load tile state from a file.
- Opens a file dialog.
- Loaded data needs to be `Run Length Encoded`.
## Scripts
### Astar.cs
Holds the logic for A* path finding algorithm. Allows traversing of all 8 nodes immediately adjacent (4) and diagonal (4) of any node. Considers diagonal traversing as "blocked" if 2 of the immediate adjacent steps are also blocked, i.e. unable to slip through diagonally.
### Constants.cs
Holds value for constant variables.
### Grid.cs
Holds the logic to manage the 2D `Tile` array
### Heap.cs
Custom implementation of a generic min-heap to hold the node data for path finding calculations. Element comparison function is passed in from `Astar.cs`.
### Main.cs
Holds all the UI logic for this project. Contains all the functions which manages,
- Labels
- Button
- Signals
- Inputs
- Tile Controls
### Tile.cs
Holds the logic to control state of individual tile.
### Utils.cs
Holds the logic for applying and reversing Run Length Encoding when saving or loading current title states. 
