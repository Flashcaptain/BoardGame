using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [HideInInspector]
    public Vector2 _position;

    [HideInInspector]
    public TeamEnum _team;

    [HideInInspector]
    public bool _hasMoved;

    public ClassEnum _class;

    public Sprite _image;

    public int _range;

    public bool _isRanged;

    public bool _canClimb;

    public bool _canFly;

    [HideInInspector]
    public List<ClassEnum> _canKill = new List<ClassEnum>();

    [HideInInspector]
    public bool[] _classBool;

    private MeshRenderer _meshRenderer;

    public void SetUp(TeamEnum team)
    {
        _team = team;
        _meshRenderer = GetComponent<MeshRenderer>();

        switch (_team)
        {
            case TeamEnum.Blue:
                _meshRenderer.material.color = Color.blue;
                break;
            case TeamEnum.Red:
                _meshRenderer.material.color = Color.red;
                break;
        }
    }
}
