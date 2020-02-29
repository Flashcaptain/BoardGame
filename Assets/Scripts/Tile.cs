using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2 _position;

    public TypeEnum _type;

    [HideInInspector]
    public TileStatusEnum _tileStatus;

    [HideInInspector]
    public bool _occupied;

    [HideInInspector]
    public TeamEnum _team = TeamEnum.none;

    [SerializeField]
    private MeshRenderer _meshRenderer;

    private void Start()
    {
        ChangeTexture();
    }

    public void ChangeTexture()
    {
        switch (_tileStatus)
        {
            case TileStatusEnum.None:
                ReversTexture();
                break;
            case TileStatusEnum.Walk:
                _meshRenderer.material.color = Color.yellow;
                break;
            case TileStatusEnum.Attack:
                _meshRenderer.material.color = Color.red;
                break;
        }
    }

    private void ReversTexture()
    {
        switch (_type)
        {
            case TypeEnum.Ground:
                _meshRenderer.material.color = Color.white;
                break;
            case TypeEnum.Hole:
                _meshRenderer.material.color = Color.black;
                break;
            case TypeEnum.Water:
                _meshRenderer.material.color = Color.blue;
                break;
        }
    }

    private void OnMouseDown()
    {
        GameManager.Instance.SelectSpot(_position);
    }
}
