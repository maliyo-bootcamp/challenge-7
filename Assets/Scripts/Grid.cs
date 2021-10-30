using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
  public int xDimension;
  public int yDimension;
  public float fillTime;
  public PiecePrefab[] piecePrefabs;
  public PiecePosition[] initialPieces;
  public GameObject backgroundPrefab;

  private Dictionary<PieceType, GameObject> _piecePrefabDict;
  private GamePiece[,] _pieces;

  private bool _inverse = false;
  private GamePiece _pressedPiece;
  private GamePiece _enteredPiece;
  private bool _gameOver;
  private bool _isFilling;
  public bool isFilling => _isFilling;
  public Level level;

  [System.Serializable]
  public struct PiecePrefab
  {
    public PieceType type;
    public GameObject prefab;
  }

  [System.Serializable]
  public struct PiecePosition
  {
    public PieceType type;
    public int x;
    public int y;
  }

  private void Awake()
  {
    _piecePrefabDict = new Dictionary<PieceType, GameObject>();
    for (int i = 0; i < piecePrefabs.Length; i++)
    {
      if (!_piecePrefabDict.ContainsKey(piecePrefabs[i].type))
      {
        _piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
      }
    }

    // We create tiles for the pieces to sit on
    for (int x = 0; x < xDimension; x++)
    {
      for (int y = 0; y < yDimension; y++)
      {
        GameObject background = (GameObject)Instantiate(backgroundPrefab, GetWorldPosition(x, y), Quaternion.identity);
        background.transform.parent = transform;
      }
    }

    // assigning pieces array to the amount of pieces to be created on the x and y axis
    _pieces = new GamePiece[xDimension, yDimension];

    for (int x = 0; x < xDimension; x++)
    {
      for (int y = 0; y < yDimension; y++)
      {
        SpawnNewPiece(x, y, PieceType.EMPTY);
      }
    }

    // Destroy(_pieces[4, 2].gameObject);
    // Destroy(_pieces[2, 2].gameObject);
    // Destroy(_pieces[6, 4].gameObject);
    // Destroy(_pieces[6, 2].gameObject);
    // SpawnNewPiece(4, 2, PieceType.BUBBLE);
    // SpawnNewPiece(2, 2, PieceType.BUBBLE);
    // SpawnNewPiece(6, 4, PieceType.BUBBLE);
    // SpawnNewPiece(6, 2, PieceType.BUBBLE);

    StartCoroutine(Fill());
  }


  private void Update()
  {

  }

  public IEnumerator Fill()
  {
    bool needsRefill = true;
    while (needsRefill)
    {
      yield return new WaitForSeconds(fillTime);
      while (FillStep())
      {
        _inverse = !_inverse;
        yield return new WaitForSeconds(fillTime);
      }
      needsRefill = ClearAllValidMatches();
    }
  }

  public bool FillStep()
  {
    bool movedPiece = false;

    for (int y = yDimension - 2; y >= 0; y--)
    {
      for (int loopX = 0; loopX < xDimension; loopX++)
      {
        int x = loopX;

        if (_inverse)
        {
          x = xDimension - 1 - loopX;
        }

        GamePiece piece = _pieces[x, y];

        if (!piece.IsMovable()) continue;
        // {
        GamePiece pieceBelow = _pieces[x, y + 1];

        if (pieceBelow.Type == PieceType.EMPTY)
        {
          Destroy(pieceBelow.gameObject);
          piece.MovableComponent.Move(x, y + 1, fillTime);
          _pieces[x, y + 1] = piece;
          SpawnNewPiece(x, y, PieceType.EMPTY);
          movedPiece = true;
        }
        else
        {
          for (int diag = -1; diag <= 1; diag++)
          {
            if (diag == 0) continue;
            //{
            int diagX = x + diag;

            if (_inverse)
            {
              diagX = x - diag;
            }

            if (diagX < 0 || diagX >= xDimension) continue;
            // {
            GamePiece diagonalPiece = _pieces[diagX, y + 1];

            if (diagonalPiece.Type != PieceType.EMPTY) continue;
            //{
            bool hasPieceAbove = true;

            for (int aboveY = y; aboveY >= 0; aboveY--)
            {
              GamePiece pieceAbove = _pieces[diagX, aboveY];

              if (pieceAbove.IsMovable())
              {
                break;
              }
              else if (/*!pieceAbove.IsMovable() && */ pieceAbove.Type != PieceType.EMPTY)
              {
                hasPieceAbove = false;
                break;
              }
            }

            if (hasPieceAbove) continue;
            // {
            Destroy(diagonalPiece.gameObject);
            piece.MovableComponent.Move(diagX, y + 1, fillTime);
            _pieces[diagX, y + 1] = piece;
            SpawnNewPiece(x, y, PieceType.EMPTY);
            movedPiece = true;
            break;
          }
        }
      }
    }

    for (int x = 0; x < xDimension; x++)
    {
      GamePiece pieceBelow = _pieces[x, 0];
      if (pieceBelow.Type != PieceType.EMPTY) continue;
      //{
      Destroy(pieceBelow.gameObject);
      GameObject newPiece = (GameObject)Instantiate(_piecePrefabDict[PieceType.NORMAL], GetWorldPosition(x, -1), Quaternion.identity, this.transform);
      // newPiece.transform.parent = transform;
      _pieces[x, 0] = newPiece.GetComponent<GamePiece>();
      _pieces[x, 0].Init(x, -1, this, PieceType.NORMAL);
      _pieces[x, 0].MovableComponent.Move(x, 0, fillTime);
      _pieces[x, 0].ColorComponent.SetColor((ColorType)Random.Range(0, _pieces[x, 0].ColorComponent.NumColors));
      movedPiece = true;
      //  }
    }
    return movedPiece;
  }

  public Vector2 GetWorldPosition(int x, int y)
  {
    // float xPos = transform.position.x - xDimension / 2.0f + x;
    // float yPos = transform.position.y + yDimension / 2.0f - y;
    return new Vector2(transform.position.x - xDimension / 2.0f + x,
                      transform.position.y + yDimension / 2.0f - y);
  }

  public GamePiece SpawnNewPiece(int x, int y, PieceType type)
  {
    GameObject newPiece = (GameObject)Instantiate(_piecePrefabDict[type], GetWorldPosition(x, y), Quaternion.identity, this.transform);
    _pieces[x, y] = newPiece.GetComponent<GamePiece>();
    _pieces[x, y].Init(x, y, this, type);
    return _pieces[x, y];
  }

  public bool isAdjacent(GamePiece piece1, GamePiece piece2)
  {
    return (piece1.X == piece2.X && (int)Mathf.Abs(piece1.Y - piece2.Y) == 1)
        || (piece1.Y == piece2.Y && (int)Mathf.Abs(piece1.X - piece2.X) == 1);
  }

  public void SwapPieces(GamePiece piece1, GamePiece piece2)
  {
    if (_gameOver) return;
    if (!piece1.IsMovable() || !piece2.IsMovable()) return;
    //{
    _pieces[piece1.X, piece1.Y] = piece2;
    _pieces[piece2.X, piece2.Y] = piece1;

    if (GetMatch(piece1, piece2.X, piece2.Y) != null || GetMatch(piece2, piece1.X, piece1.Y) != null
                                                     || piece1.Type == PieceType.RAINBOW || piece2.Type == PieceType.RAINBOW)
    {
      int piece1X = piece1.X;
      int piece1Y = piece1.Y;

      piece1.MovableComponent.Move(piece2.X, piece2.Y, fillTime);
      piece2.MovableComponent.Move(piece1X, piece1Y, fillTime);

      if (piece1.Type == PieceType.RAINBOW && piece1.IsClearable() && piece2.isColored())
      {
        ClearColorPiece clearColor = piece1.GetComponent<ClearColorPiece>();

        if (clearColor)
        {
          clearColor.Color = piece2.ColorComponent.Color;
        }
        ClearPiece(piece1.X, piece1.Y);
      }

      if (piece2.Type == PieceType.RAINBOW && piece2.IsClearable() && piece1.isColored())
      {
        ClearColorPiece clearColor = piece2.GetComponent<ClearColorPiece>();

        if (clearColor)
        {
          clearColor.Color = piece1.ColorComponent.Color;
        }
        ClearPiece(piece2.X, piece2.Y);
      }

      ClearAllValidMatches();

      // clear special pieces even if not matched
      if (piece1.Type == PieceType.ROW_CLEAR || piece1.Type == PieceType.COLUMN_CLEAR)
      {
        ClearPiece(piece1.X, piece1.Y);
      }

      if (piece2.Type == PieceType.ROW_CLEAR || piece2.Type == PieceType.COLUMN_CLEAR)
      {
        ClearPiece(piece2.X, piece2.Y);
      }

      _pressedPiece = null;
      _enteredPiece = null;

      StartCoroutine(Fill());

      // ?? Consider doing this with delegates <What are delegates?>

      level.OnMove();
    }
    else
    {
      _pieces[piece1.X, piece1.Y] = piece1;
      _pieces[piece2.X, piece2.Y] = piece2;
    }
    // }
  }

  public void PressPiece(GamePiece piece)
  {
    _pressedPiece = piece;
  }

  public void EnterPiece(GamePiece piece)
  {
    _enteredPiece = piece;
  }

  public void ReleasePiece()
  {
    if (isAdjacent(_pressedPiece, _enteredPiece))
    {
      SwapPieces(_pressedPiece, _enteredPiece);
    }
  }

  public List<GamePiece> GetMatch(GamePiece piece, int newX, int newY)
  {
    if (!piece.isColored()) return null;
    {
      var color = piece.ColorComponent.Color;
      var horizontalPieces = new List<GamePiece>();
      var verticalPieces = new List<GamePiece>();
      var matchingPieces = new List<GamePiece>();

      // vertical traversy
      horizontalPieces.Add(piece);

      for (int dir = 0; dir <= 1; dir++)
      {
        for (int xOffset = 1; xOffset < xDimension; xOffset++)
        {
          int x;

          if (dir == 0) // means we going left 
          {
            x = newX - xOffset;
          }
          else // we going right
          {
            x = newX + xOffset;
          }

          if (x < 0 || x >= xDimension) { break; }

          if (_pieces[x, newY].isColored() && _pieces[x, newY].ColorComponent.Color == color)
          {
            horizontalPieces.Add(_pieces[x, newY]);
          }
          else
          {
            break;
          }
        }
      }

      if (horizontalPieces.Count >= 3)
      {
        for (int i = 0; i < horizontalPieces.Count; i++)
        {
          matchingPieces.Add(horizontalPieces[i]);
        }
      }


      // Traverse vertically if a match is found (T & L shape)
      if (horizontalPieces.Count >= 3)
      {
        for (int i = 0; i < horizontalPieces.Count; i++)
        {
          for (int dir = 0; dir <= 1; dir++)
          {
            for (int yOffset = 1; yOffset < yDimension; yOffset++)
            {
              int y;

              if (dir == 0)
              {
                y = newY - yOffset;
              }
              else
              {
                y = newY + yOffset;
              }

              if (y < 0 || y >= yDimension)
              {
                break;
              }

              if (_pieces[horizontalPieces[i].X, y].isColored() && _pieces[horizontalPieces[i].X, y].ColorComponent.Color == color)
              {
                verticalPieces.Add(_pieces[horizontalPieces[i].X, y]);
              }
              else
              {
                break;
              }
            }
          }
          if (verticalPieces.Count < 2)
          {
            verticalPieces.Clear();
          }
          else
          {
            for (int j = 0; j < verticalPieces.Count; j++)
            {
              matchingPieces.Add(verticalPieces[j]);
            }
            break;
          }
        }
      }

      if (matchingPieces.Count >= 3)
      {
        return matchingPieces;
      }

      verticalPieces.Clear();
      horizontalPieces.Clear();
      // vertical traversal
      verticalPieces.Add(piece);

      for (int dir = 0; dir <= 1; dir++)
      {
        for (int yOffset = 1; yOffset < xDimension; yOffset++)
        {
          int y;

          if (dir == 0) // we going UP
          {
            y = newY - yOffset;
          }
          else // we going DOWN
          {
            y = newY + yOffset;
          }

          if (y < 0 || y >= yDimension)
          {
            break;
          }

          if (_pieces[newX, y].isColored() && _pieces[newX, y].ColorComponent.Color == color)
          {
            verticalPieces.Add(_pieces[newX, y]);
          }
          else
          {
            break;
          }
        }
      }

      if (verticalPieces.Count >= 3)
      {
        for (int i = 0; i < verticalPieces.Count; i++)
        {
          matchingPieces.Add(verticalPieces[i]);
        }
      }

      // Traverse horizontal if a match is found (T & L shape)
      if (verticalPieces.Count >= 3)
      {
        for (int i = 0; i < verticalPieces.Count; i++)
        {
          for (int dir = 0; dir <= 1; dir++)
          {
            //! fixed here
            for (int xOffset = 1; xOffset < yDimension; xOffset++)
            {
              int x;

              if (dir == 0) // search to Left
              {
                x = newX - xOffset;
              }
              else // search to Right
              {
                x = newX + xOffset;
              }

              if (x < 0 || x >= xDimension)
              {
                break;
              }

              if (_pieces[x, verticalPieces[i].Y].isColored() && _pieces[x, verticalPieces[i].Y].ColorComponent.Color == color)
              {
                verticalPieces.Add(_pieces[x, verticalPieces[i].Y]);
              }
              else
              {
                break;
              }
            }
          }
          if (horizontalPieces.Count < 2)
          {
            horizontalPieces.Clear();
          }
          else
          {
            for (int j = 0; j < horizontalPieces.Count; j++)
            {
              matchingPieces.Add(horizontalPieces[j]);
            }
            break;
          }
        }
      }

      if (matchingPieces.Count >= 3)
      {
        return matchingPieces;
      }
    }
    return null;
  }

  private bool ClearAllValidMatches()
  {
    bool needsRefill = false;

    for (int y = 0; y < yDimension; y++)
    {
      for (int x = 0; x < xDimension; x++)
      {

        if (!_pieces[x, y].IsClearable()) continue;
        // {
        List<GamePiece> match = GetMatch(_pieces[x, y], x, y);

        if (match == null) continue;
        // {
        PieceType specialPieceType = PieceType.COUNT;
        GamePiece randomPiece = match[Random.Range(0, match.Count)];
        int specialPieceX = randomPiece.X;
        int specialPieceY = randomPiece.Y;

        if (match.Count == 4)
        {
          if (_pressedPiece == null || _enteredPiece == null)
          {
            specialPieceType = (PieceType)Random.Range((int)PieceType.ROW_CLEAR, (int)PieceType.COLUMN_CLEAR);
          }
          else if (_pressedPiece.Y == _enteredPiece.Y)
          {
            specialPieceType = PieceType.ROW_CLEAR;
          }
          else
          {
            specialPieceType = PieceType.COLUMN_CLEAR;
          }
        }

        else if (match.Count >= 5)
        {
          specialPieceType = PieceType.RAINBOW;
        }

        for (int i = 0; i < match.Count; i++)
        {
          if (!ClearPiece(match[i].X, match[i].Y)) continue;

          needsRefill = true;

          if (match[i] != _pressedPiece && match[i] != _enteredPiece) continue;

          specialPieceX = match[i].X;
          specialPieceY = match[i].Y;
        }

        if (specialPieceType == PieceType.COUNT) continue;

        Destroy(_pieces[specialPieceX, specialPieceY]);

        GamePiece newPiece = SpawnNewPiece(specialPieceX, specialPieceY, specialPieceType);

        if ((specialPieceType == PieceType.ROW_CLEAR || specialPieceType == PieceType.COLUMN_CLEAR) && newPiece.isColored() && match[0].isColored())
        {
          newPiece.ColorComponent.SetColor(match[0].ColorComponent.Color);
        }
        else if (specialPieceType == PieceType.RAINBOW && newPiece.isColored())
        {
          newPiece.ColorComponent.SetColor(ColorType.ANY);
        }
      }
    }
    return needsRefill;
  }

  private bool ClearPiece(int x, int y)
  {
    if (!_pieces[x, y].IsClearable() || _pieces[x, y].ClearableComponent.IsBeingCleared) return false;
    // {
    _pieces[x, y].ClearableComponent.Clear();
    SpawnNewPiece(x, y, PieceType.EMPTY);
    ClearObstacles(x, y);
    return true;
    // }
  }

  public void ClearObstacles(int x, int y)
  {
    for (int adjacentX = x - 1; adjacentX <= x + 1; adjacentX++)
    {
      if (adjacentX == x || adjacentX < 0 || adjacentX >= xDimension) continue;
      // {
      if (_pieces[adjacentX, y].Type == PieceType.BUBBLE || !_pieces[adjacentX, y].IsClearable()) continue;
      // {
      _pieces[adjacentX, y].ClearableComponent.Clear();
      SpawnNewPiece(adjacentX, y, PieceType.EMPTY);
      // }
      //  }
    }

    for (int adjacentY = y - 1; adjacentY <= y + 1; adjacentY++)
    {
      if (adjacentY == y || adjacentY < 0 || adjacentY >= yDimension) continue;
      // {
      if (_pieces[x, adjacentY].Type != PieceType.BUBBLE || !_pieces[x, adjacentY].IsClearable()) continue;
      // {
      _pieces[x, adjacentY].ClearableComponent.Clear();
      SpawnNewPiece(x, adjacentY, PieceType.EMPTY);
      // }
      //}
    }
  }

  public void ClearRow(int row)
  {
    for (int x = 0; x < xDimension; x++)
    {
      ClearPiece(x, row);
    }
  }

  public void ClearColumn(int column)
  {
    for (int y = 0; y < yDimension; y++)
    {
      ClearPiece(column, y);
    }
  }

  public void ClearColor(ColorType color)
  {
    for (int x = 0; x < xDimension; x++)
    {
      for (int y = 0; y < yDimension; y++)
      {
        if ((_pieces[x, y].isColored() && _pieces[x, y].ColorComponent.Color == color) || (color == ColorType.ANY))
        {
          ClearPiece(x, y);
        }
      }
    }
  }

  public void GameOver()
  {
    _gameOver = true;
  }

  public List<GamePiece> GetPiecesOfType(PieceType type)
  {
    var piecesOfType = new List<GamePiece>();

    for (int x = 0; x < xDimension; x++)
    {
      for (int y = 0; y < yDimension; y++)
      {
        if (_pieces[x, y].Type == type)
        {
          piecesOfType.Add(_pieces[x, y]);
        }
      }
    }
    return piecesOfType;
  }

}
