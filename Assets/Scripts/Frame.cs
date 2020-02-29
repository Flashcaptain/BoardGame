using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Frame : MonoBehaviour
{
    [HideInInspector]
    public List<Text> _text;

    [SerializeField]
    private Image _image;

    public Unit _unit;
    public int _amount;

    private void Awake()
    {
        if (_unit != null)
        {
            _text[0].text = _unit.gameObject.name;
            _text[1].text = _amount.ToString();
            _image.sprite = _unit._image;
        }
    }

    public void RemoveOne()
    {
        _amount--;
        _text[1].text = _amount.ToString();
    }

    public void SpawnUnit()
    {
        GameManager.Instance.GetUnit(this);
    }
}
