using Assets.Scripts.Utils;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class MapManager : MonoBehaviour
{
    [SerializeField]
    Vector3 _gemSize = new Vector3(0.1f, 0.1f, 0.1f);

    // ���
    public List<List<Tuple<GameObject, int>>> _gems = new List<List<Tuple<GameObject, int>>>();
    
    // ���Ե�
    List<List<GameObject>> _slots = new List<List<GameObject>>();

    GameObject _gamePlayUI;
    GameObject GamePlayUI { set { _gamePlayUI = value; } get { return _gamePlayUI; } }
    GridLayoutGroup _gridLayOutGroup;

    Vector3 _firstPickedPostion;
    Vector3 _secondPickedPosition;

    Vector2 _leftTop = Vector2.zero;

    int _height;
    int _width;
    
    public void Init(GameObject gamePlayUI)
    {
        GamePlayUI = gamePlayUI; 
        
        _gridLayOutGroup = GamePlayUI.GetComponentInChildren<GridLayoutGroup>();
        if (_gridLayOutGroup == null)
        {
            Debug.Log($"{gamePlayUI.name}�� ������Ʈ�� �����ϴ�!");
            return;
        }


        Transform[] slots = _gridLayOutGroup.GetComponentsInChildren<Transform>().Where(t => t != _gridLayOutGroup.transform).ToArray();
        _slots.Clear();

        // �⺻���� ������ width height ����
        Transform parentTransform = _gridLayOutGroup.transform;
        RectTransform rectTransform = GamePlayUI.GetComponent<RectTransform>();
        RectTransform gridRectTransform = parentTransform.GetComponent<RectTransform>();
        RectTransform slotRectTransform = parentTransform.GetChild(0).GetComponent<RectTransform>();
        _leftTop = new Vector2(-gridRectTransform.rect.width, -gridRectTransform.rect.height);

        // �⺻���� ������ width height ����
        float slotWidth = gridRectTransform.rect.width / slotRectTransform.rect.width;
        float slotHeight = gridRectTransform.rect.height / slotRectTransform.rect.height;
        _height = (int)MathF.Floor(slotWidth);
        _width = (int)MathF.Floor(slotHeight);
        _gems.Capacity = _height;


        int slotCnt = slots.Count();
        int height = slotCnt / _width;

        int tempIdx = 0;
        for (int y = 0; y < height;  y++)
        {
            _slots.Add(new List<GameObject>());
            for (int x = 0; x < _width; x++)
            {
                _slots[y].Add(slots[tempIdx++].gameObject);
            }
        }

        // ���� ����
        for (int y = 0; y < _height; y++)
        {
            _gems.Add(new List<Tuple<GameObject, int>>());
            for (int x = 0; x < _width; x++)
            {
                _gems[y].Add(GenerateRandGem(y, x));
            }
        }
         
        _gemIntervalSize = _gems[0][0].Item1.transform.position.y - _gems[1][0].Item1.transform.position.y;
    }

    Tuple<GameObject, int> GenerateRandGem(int idxY, int idxX, Vector3? controllPos = null)
    {
        // ���� ����
        int randNum = UnityEngine.Random.Range((int)Define.GemType.Start + 1, (int)Define.GemType.End - 1);
        string genName = Util.EnumToString((Define.GemType)randNum);
        
        // Gem ��Ͽ� ��ħ
        GameObject gem = Managers.Resource.Instantiate(genName);


        gem.transform.parent = _slots[idxY][idxX].transform;

        if (controllPos == null)
            gem.transform.localPosition = Vector3.zero;
        else
            gem.transform.localPosition = (Vector3)(Vector3.zero + controllPos);

        gem.transform.localScale = new Vector3(15.0f, 15.0f, 15.0f);

        return new Tuple<GameObject, int>(gem, randNum);
    }


    [SerializeField]
    bool _isPicked = false;

    [SerializeField]
    public GameObject _pickedObj;

    [SerializeField]
    public GameObject _changeObj;

    public bool IsPicked { get { return _isPicked; } }

    public float _gemIntervalSize;

    public void InitSwap()
    {
        // ��ġ ����
        Memorize();
        // ����
        Swap(_pickedObj, _changeObj);
        // ���� ����
        StartSwapMove();
    }

    public void InitExplodeCheck()
    {
        ExplodeCheck();

        CleanUpStaticGarbageList();

        ClearPickedObjects();
    }

    void CleanUpStaticGarbageList()
    {
        foreach (var go in garbages)
        {
            go.GetComponent<GemController>().Explode();
        }

        garbages.Clear();
    }


    void Swap(GameObject a, GameObject b)
    {
        // ���� // extension ���
        _slots.Swap<GameObject>(FindGemSlot(_pickedObj), FindGemSlot(_changeObj));

        _gems.Swap<GameObject, int>(_pickedObj, _changeObj); 
    }
    
    void Memorize()
    {
        _firstPickedPostion = Managers.Map._pickedObj.transform.position;
        _secondPickedPosition = Managers.Map._changeObj.transform.position;
    }

    public IEnumerator _swapMoveCoroutine;
    void StartSwapMove()
    {
        _swapMoveCoroutine = SwapMove();
        StartCoroutine(_swapMoveCoroutine);
    }

    private IEnumerator SwapMove()
    {
        while (true)
        {
            if (_pickedObj == null || _changeObj == null)
            {
                _swapMoveCoroutine = null;
                yield break;
            }

            if ((_pickedObj.transform.position - _secondPickedPosition).magnitude <= 0.01f)
                break;

            _pickedObj.transform.position = Vector3.Lerp(_pickedObj.transform.position, _secondPickedPosition, 3.0f * Time.deltaTime);
            _changeObj.transform.position = Vector3.Lerp(_changeObj.transform.position, _firstPickedPostion, 3.0f * Time.deltaTime);
            yield return null; 
        }

        if ((_pickedObj.transform.position - _secondPickedPosition).magnitude <= 0.01f)
        {
            _pickedObj.transform.position = _secondPickedPosition;
            _changeObj.transform.position = _firstPickedPostion;
            _swapMoveCoroutine = null;
            
            yield break;
        }
    }

    // U D L R
    int[] dx = { 0, 0, -1, 1 };
    int[] dy = { -1, 1, 0, 0 };

    public bool SetPickObject(GameObject go)
    {
        if (_swapMoveCoroutine != null)
            return false;

        if (IsPicked == false)
        {
            _pickedObj = go;
            _isPicked = true;
            return false;
        }
        else
        {
            _changeObj = go;
            _isPicked = false;

            // �ε��� üũ
            GameObject changeSlot = FindGemSlot(_changeObj);
            GameObject pickSlot = FindGemSlot(_pickedObj);

            Tuple<int, int> idx = _slots.FindIndexByTuple(changeSlot);
            Tuple<int, int> checkIdx = _slots.FindIndexByTuple(pickSlot);
            
            int row = idx.Item1;
            int col = idx.Item2;

            int pickRow = checkIdx.Item1;
            int pickCol = checkIdx.Item2;
            for (int i = 0; i < dx.Length; i++)
            {
                int checkRow = row + dx[i];
                int checkCol = col + dy[i];

                if (checkRow < 0 || checkRow > _slots.Count - 1 || checkCol < 0 || checkCol > _slots[0].Count - 1)
                    continue;

                if (_gems[checkRow][checkCol] == _gems[pickRow][pickCol])
                    return true;
            }

            return true;
        }
    }

    void ExplodeCheck()
    {
        CheckLineExplode(_changeObj);
        CheckLineExplode(_pickedObj);
    }

    void ClearPickedObjects()
    {
        _changeObj = null;
        _pickedObj = null;
    }

    IEnumerator _gemMoveCoroutine;
    void GemMove(GameObject obj, Vector3? pos = null, bool isHorizontal = true) // Horizontal �� �����϶��� Pos�� ����
    {
        List<GameObject> _objs = new List<GameObject>();

        var slotPos = FindGemSlotIdx(obj);
        int slotCol = slotPos.Item2;
        int upperRow = slotPos.Item1 - 1;

        // ���� ����� ��� ����Ʈ�� �߰�
        while (upperRow >= 0)
        {
            if (_gems[upperRow][slotCol] == null)
                break;

            GameObject startObj = _gems[upperRow][slotCol].Item1;
            _objs.Add(startObj);
            upperRow--;
        }

        // ����Ʈ�� �ִ� ����� ��ĭ�� �Ʒ��� �̵�
        int gemCntForHor = -2;
        foreach (GameObject gem in _objs)
        {
            slotPos = FindGemSlotIdx(gem);
            int currentRow = slotPos.Item1;
            int currentCol = slotPos.Item2;

            int newRow = -1;
            Vector3 newPos = Vector3.zero;
            
            // �̵��� ��ġ
            newRow = currentRow + 1;

            if (_gems[newRow][currentCol] == null)
                continue;

            if (isHorizontal)
                newPos = _gems[newRow][currentCol].Item1.transform.position;
            else
            {
                // ��ġ ��ȯ
                Vector3 updatePos = (Vector3)pos;
                updatePos.y += _gemIntervalSize * gemCntForHor;
                newPos = updatePos;
                gemCntForHor++;

                // �׸��� ������Ʈ
                var originPos = FindGemSlotIdx(obj);
                currentRow = originPos.Item1;

                newRow = currentRow + 1;
            }

            // �׸��� ������Ʈ
            _gems[newRow][currentCol] = _gems[currentRow][currentCol];

            // �� �̵� �ڷ�ƾ ����
            _gemMoveCoroutine = StartDownMove(gem, newPos);
            StartCoroutine(_gemMoveCoroutine);
        }
    }

    IEnumerator StartDownMove(GameObject gem, Vector3 newPos)
    {
        float elapsedTime = 0;
        float duration = 0.5f;
        Vector3 startingPos = gem.transform.position;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            gem.transform.position = Vector3.Lerp(startingPos, newPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gem.transform.position = newPos;
        yield break;
    }

    GameObject FindLowestPosition(List<GameObject> objs)
    {
        if (objs == null || objs.Count == 0)
        {
            Debug.Log("���� ���� ������Ʈ�� ã�� ���߽��ϴ�.");
            return null;
        }

        // OrderedByDescending �� �ϸ� �������� // ����� ��������
        GameObject lowestObj = objs.OrderBy(obj => obj.transform.position.y).FirstOrDefault();

        return lowestObj;
    }

    static HashSet<GameObject> garbages = new HashSet<GameObject>();
    void CheckLineExplode(GameObject obj)
    {
        Tuple<int, int> pos = FindGemSlotIdx(obj);

        int row = pos.Item1;
        int col = pos.Item2;

        List<GameObject> horizontalGems = BFSSearch(row, col, 0, 1); // R
        // ������ ���ؼ� AddRange Ȱ�� 
        horizontalGems.AddRange(BFSSearch(row, col, 0, -1)); // L

        List<GameObject> verticalGems = BFSSearch(row, col, 1, 0); // D

        verticalGems.AddRange(BFSSearch(row, col, -1, 0)); // U

        horizontalGems.Add(obj);
        verticalGems.Add(obj);

        // 3�� �̻��̸� gemcontroller explode
        if (horizontalGems.Count >= 3)
        {
            foreach (GameObject gem in horizontalGems)
            {
                var gemslot = FindGemSlotIdx(gem);
                // �� ���� ���� �� ���� ����� �ϴ� ������ �۾� ����
                GemMove(gem);

                // �����
                _gems[0][gemslot.Item2] = (GenerateRandGem(0, gemslot.Item2));
            }

            garbages.AddRange(horizontalGems);
        }

        if (verticalGems.Count >= 3)
        {
            GameObject lowestObj = FindLowestPosition(verticalGems);

            int cnt = 0;
            GemMove(lowestObj, lowestObj.transform.position, false);

            var gemslot = FindGemSlotIdx(verticalGems[0]);
            for (int i = 0; i < verticalGems.Count; i++)
            {
                _gems[cnt][gemslot.Item2] = (GenerateRandGem(cnt, gemslot.Item2));
                cnt++;
            }

            garbages.AddRange(verticalGems);

        }
    }



    List<GameObject> BFSSearch(int startRow, int startCol, int dRow, int dCol)
    {
        List<GameObject> gems = new List<GameObject>();
        Queue<Tuple<int, int>> queue = new Queue<Tuple<int, int>>();
        
        // hashset�� ��ø �湮�� ���� ���� ���
        HashSet<Tuple<int, int>> visited = new HashSet<Tuple<int, int>>();

        queue.Enqueue(new Tuple<int, int>(startRow, startCol));

        while (queue.Count > 0)
        {
            var pos = queue.Dequeue();
            int row = pos.Item1 + dRow;
            int col = pos.Item2 + dCol;

            if (row >= 0 && row < _slots[0].Count && col >= 0 && col < _slots.Count) 
            {
                if (_slots[row][col] == null)
                    continue;

                if (_gems[row][col].Item1 == null)
                    continue;

                if (_gems[row][col].Item1.name != _gems[startRow][startCol].Item1.name)
                    continue;

                var newPos = new Tuple<int, int>(row, col);
                if (visited.Contains(newPos) == false)
                {
                    gems.Add(_gems[row][col].Item1);
                    queue.Enqueue(newPos);
                    visited.Add(newPos);
                }
            }
        }

        return gems;
    }

    public GameObject FindGemSlot(GameObject go)
    {
        Transform slotTr = go.transform.GetComponentInParent<RectTransform>();
        GameObject slot = _slots.Find(slotTr.gameObject);

        if (slot == null)
        {
            Debug.Log("������ ã�� ���߽��ϴ�");
            return null;
        }

        return slot;
    }

    public Tuple<int, int> FindGemSlotIdx(GameObject go)
    {
        Tuple<int, int> idx = _gems.FindIndexByTuple(go);

        if (idx == null)
            Debug.LogError($"FindGemSlotIdx �Լ� ���� NULL �Դϴ�.: {go}");

        return new Tuple<int,int> (idx.Item1, idx.Item2);
    }

}
