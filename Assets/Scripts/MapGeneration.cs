using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;

    [SerializeField]
    [Range(6,99)]
    private int _mapsize;

    [SerializeField]
    public float _tileSize;

    [SerializeField]
    private Tile _tilePrefab;

    public Tile[,] _tiles;

    private float _redBorder;
    private float _blueBorder;

    private void OnValidate()
    {
        if (_mapsize % 3 != 0)
        {
            _mapsize++;
        }
    }

    private void Start()
    {
        _tiles = new Tile[_mapsize, _mapsize];
        SpawnBorder();
        SpawnTiles();
        float cameraPosition = ((_mapsize - 1) * _tileSize) / 2;
        _camera.transform.position = new Vector3(cameraPosition, cameraPosition, cameraPosition);
        _camera.orthographicSize = cameraPosition + _tileSize;

    }

    private void SpawnBorder()
    {
        _redBorder = Mathf.Round(_mapsize / 3);
        _blueBorder = _redBorder * 2;
    }

    private void SpawnTiles()
    {
        for (int x = 0; x < _mapsize; x++)
        {
            for (int y = 0; y < _mapsize; y++)
            {
                _tiles[x, y] = Instantiate(_tilePrefab, new Vector3(x, 0, y) * _tileSize, transform.rotation);
                _tiles[x, y]._position = new Vector2(x, y);
                _tiles[x, y]._team = TeamEnum.none;
                if (y < _redBorder)
                {
                    _tiles[x, y]._team = TeamEnum.Red;
                }
                if (y >= _blueBorder)
                {
                    _tiles[x, y]._team = TeamEnum.Blue;
                }
            }
        }
    }
}
