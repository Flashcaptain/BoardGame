using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private TeamEnum _currentTeam = TeamEnum.Blue;

    [SerializeField]
    private GameObject _placementUI;

    [SerializeField]
    private GameObject _teamBlueUI;

    [SerializeField]
    private MapGeneration _map;

    [SerializeField]
    private int _turns;

    private List<Unit> _units = new List<Unit>();

    private List<Tile> _tiles = new List<Tile>();

    private Unit _selectedUnit;

    private int _totalPlaceAble;
    private int _placeAble;

    private List<Frame> _frames = new List<Frame>();
    private Frame _frame;

    private bool _placementPhase = true;

    private int _turn;

    private void Awake()
    {
        Instance = this;
        foreach (Frame go in Resources.FindObjectsOfTypeAll(typeof(Frame)) as Frame[])
        {
            _frames.Add(go);
            _placeAble += go._amount;
            _totalPlaceAble = _placeAble;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }

    public void SelectSpot(Vector2 spot)
    {
        if (_placementPhase)
        {
            PlaceUnit(spot);
            return;
        }

        for (int i = 0; i < _units.Count; i++)
        {
            if (_units[i]._position == spot)
            {
                //if (_selectedUnit != null && _units[i]._class == ClassEnum.Wall && _selectedUnit._canClimb)
                //{
                //    //MoveUnit(_selectedUnit , spot , true);
                //    return;
                //}

                if (_units[i]._team == _currentTeam)
                {
                    SelectUnit(_units[i]); //selected unit thatcan be moved
                    return;
                }

                if (_selectedUnit == null)
                {
                    //this is an enemy unit please select one of your units first
                    return;
                }

                if (_map._tiles[(int)spot.x, (int)spot.y]._tileStatus == TileStatusEnum.Attack)
                {
                    AttackUnit(_selectedUnit , _units[i]);
                    NextTurn();
                    return;
                }
            }
        }

        //no unit on this spot

        if (_selectedUnit == null)
        {
            //you have no unit selected
            return;
        }

        if (_map._tiles[(int)spot.x, (int)spot.y]._tileStatus == TileStatusEnum.Walk)
        {
            MoveUnit(_selectedUnit , spot);
            NextTurn();
            return;
        }

        //cant go here
    }

    private void SelectUnit(Unit unit)
    {
        _selectedUnit = unit;
        CalculatePath(unit);
    }

    private void CalculatePath(Unit unit)
    {
        ClearTiles();

        bool canAttack = true;
        _tiles.Add(_map._tiles[(int)unit._position.x, (int)unit._position.y]);
        List<Tile> tempTiles = new List<Tile>(_tiles);

        for (int i = 0; i < unit._range; i++)
        {
            for (int t = 0; t < tempTiles.Count; t++)
            {
                int x = (int)tempTiles[t]._position.x;
                int y = (int)tempTiles[t]._position.y;

                CheckTile(x + 1, y, unit, canAttack);
                CheckTile(x - 1, y, unit, canAttack);
                CheckTile(x, y + 1, unit, canAttack);
                CheckTile(x, y - 1, unit, canAttack);

                if (!unit._isRanged)
                {
                    canAttack = false;
                    if (unit._hasMoved)
                    {
                        i = unit._range;
                    }
                }
            }
            tempTiles = new List<Tile>(_tiles);
        }

        for (int i = 0; i < _tiles.Count; i++)
        {
            _tiles[i].ChangeTexture();
        }
    }

    private void CheckTile(int x, int y, Unit unit, bool canAttack)
    {
        if (!InMapBounds(x,y) || _map._tiles[x, y]._tileStatus != TileStatusEnum.None)
        {
            return;
        }

        if (_map._tiles[x, y]._occupied)
        {
            if (canAttack)
            {
                if (_map._tiles[x, y]._team != unit._team)
                {
                    _map._tiles[x, y]._tileStatus = TileStatusEnum.Attack;
                    _tiles.Add(_map._tiles[x, y]);
                    return;
                }
            }
            return;
        }
        if (!unit._canFly)
        {
            switch (_map._tiles[x, y]._type)
            {
                case TypeEnum.Ground:
                    break;
                case TypeEnum.Hole:
                    return;
                case TypeEnum.Water:
                    return;
            }
        }

        if (unit._hasMoved)
        {
            return;
        }
        _map._tiles[x, y]._tileStatus = TileStatusEnum.Walk;
        _tiles.Add(_map._tiles[x, y]);
    }

    private bool InMapBounds(int x, int y)
    {   
        if (x < 0 || y < 0 || x > _map._tiles.GetLength(0) - 1 || y > _map._tiles.GetLength(1) - 1)
        {
            return false;
        }
        return true;
    }

    private void ClearTiles()
    {
        for (int i = 0; i < _tiles.Count; i++)
        {
            _tiles[i]._tileStatus = TileStatusEnum.None;
            _tiles[i].ChangeTexture();
        }
        _tiles = new List<Tile>();
    }

    private void MoveUnit(Unit unit , Vector2 spot)
    {
        _map._tiles[(int)unit._position.x, (int)unit._position.y]._occupied = false;

        ClearTiles();
        unit._position = spot;
        unit.transform.position = new Vector3(spot.x, 1, spot.y) * _map._tileSize;
        _map._tiles[(int)spot.x, (int)spot.y]._occupied = true;
        _map._tiles[(int)spot.x, (int)spot.y]._team = unit._team;
        unit._hasMoved = true;
    }

    private void AttackUnit(Unit attacking , Unit attacked)
    {
        ClearTiles();
        bool kill = false;
        bool die = false;

        for (int i = 0; i < attacking._canKill.Count; i++)
        {
            if (attacking._canKill[i] == attacked._class)
            {
                //remove attacked unit
                kill = true;
            }
        }

        if (attacking._isRanged == attacked._isRanged)
        {
            for (int i = 0; i < attacked._canKill.Count; i++)
            {
                if (attacked._canKill[i] == attacking._class)
                {
                    //remove attacking unit
                    die = true;
                }
            }
        }

        if (kill)
        {
            if (attacked._class == ClassEnum.King)
            {
                Victory(attacked._team);
            }
            _units.Remove(attacked);
            Destroy(attacked.gameObject);
        }
        if (die)
        {
            _units.Remove(attacking);
            Destroy(attacking.gameObject);
        }
    }

    public void GetUnit(Frame frame)
    {
        _frame = frame;
    }

    private void PlaceUnit(Vector2 spot)
    {
        if (_frame == null || _frame._amount == 0)
        {
            //no more units left
            return;
        }
        if (_map._tiles[(int)spot.x, (int)spot.y]._team != _currentTeam /*(_currentTeam == TeamEnum.Red && spot.y > _map._redBorder) || (_currentTeam == TeamEnum.Blue && spot.y < _map._blueBorder)*/)
        {
            return;
        }

        CheckTile((int)spot.x, (int)spot.y, _frame._unit, false);
        if (_map._tiles[(int)spot.x, (int)spot.y]._tileStatus == TileStatusEnum.Walk)
        {
            _units.Add(Instantiate(_frame._unit, Vector3.up, transform.rotation));
            _units[_units.Count - 1].SetUp(_currentTeam);
            MoveUnit(_units[_units.Count - 1], spot);
            _map._tiles[(int)spot.x, (int)spot.y]._tileStatus = TileStatusEnum.None;
            _frame.RemoveOne();
            if (PlacedUnits())
            {
                StartGame();
            }
            return;
        }
        //cant place here
    }

    public void StartGame()
    {

        if (_currentTeam == TeamEnum.Blue)
        {
            _currentTeam = TeamEnum.Red;
            _teamBlueUI.SetActive(false);
            for (int i = 0; i < _units.Count; i++)
            {
                _units[i].gameObject.SetActive(false);
            }
            return;
        }

        ResetUnits();
        _placementPhase = false;
        _placementUI.SetActive(false);
    }

    private void NextTurn()
    {
        _turn++;

        if (_turn == _turns)
        {
            SwitchTeam();
        }
    }

    public void SwitchTeam()
    {
        ResetUnits();
        _turn = 0;

        if (_currentTeam == TeamEnum.Blue)
        {
            _currentTeam = TeamEnum.Red;
            return;
        }
        _currentTeam = TeamEnum.Blue;
    }

    private void ResetUnits()
    {
        for (int i = 0; i < _units.Count; i++)
        {
            _units[i]._hasMoved = false;
            _units[i].gameObject.SetActive(true);
        }
    }

    private bool PlacedUnits()
    {
        _placeAble--;
        return (_totalPlaceAble / 2 == _placeAble || _placeAble == 0);
    }

    private void Victory(TeamEnum team)
    {
        Debug.Log(team.ToString() + " lost");
    }
}
