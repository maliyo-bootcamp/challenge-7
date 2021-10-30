using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{

  private int _x;
  private int _y;

  public int X
  {
    get { return _x; }

    set
    {
      if (IsMovable())
      {
        _x = value;
      }
    }
  }

  public int Y
  {
    get { return _y; }

    set
    {
      if (IsMovable())
      {
        _y = value;
      }
    }
  }
  private MoveablePiece _movableComponent;
  public MoveablePiece MovableComponent => _movableComponent;
  private ClearablePiece _clearableComponent;
  public ClearablePiece ClearableComponent => _clearableComponent;
  private ColorPiece _colorComponent;
  public ColorPiece ColorComponent => _colorComponent;

  public int score;
  public PieceType _type;
  private Grid _grid;

  public PieceType Type
  {
    get { return _type; }
  }

  public Grid GridRef
  {
    get { return _grid; }
  }


  private void Awake()
  {
    _movableComponent = GetComponent<MoveablePiece>();
    _colorComponent = GetComponent<ColorPiece>();
    _clearableComponent = GetComponent<ClearablePiece>();
  }

  public void Init(int x, int y, Grid grid, PieceType type)
  {
    _x = x;
    _y = y;
    _grid = grid;
    _type = type;
  }

  private void OnMouseEnter()
  {
    _grid.EnterPiece(this);
  }

  private void OnMouseDown()
  {
    _grid.PressPiece(this);
  }

  private void OnMouseUp()
  {
    _grid.ReleasePiece();
  }

  public bool IsMovable()
  {
    return _movableComponent != null;
  }

  public bool isColored()
  {
    return _colorComponent != null;
  }

  public bool IsClearable()
  {
    return _clearableComponent != null;
  }
}
