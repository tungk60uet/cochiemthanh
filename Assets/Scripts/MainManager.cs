using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour {

    [SerializeField]
    GameObject[] blue = new GameObject[3];//K,L,M
    [SerializeField]
    GameObject[] red = new GameObject[3];//O,P,Q
    [SerializeField]
    GameObject huongdi,viTriPrefab,chessIdle;
    GameObject firstChess = null;
    List<GameObject> ally, enemy, chessIdleContainer = new List<GameObject>();

    GameObject[,] matrix = new GameObject[8, 8];

    Vector2 selectedCell = new Vector2(-1, -1);
    Vector2 CastlePos=new Vector2(4,7),TeleportPos=new Vector2(3,7);
    List<Vector2> listHuongdi = new List<Vector2>();
    void Start()
    {
        matrix[0, 0] = blue[0];
        matrix[3, 0] = blue[1];
        matrix[7, 0] = blue[2];
        matrix[0, 7] = red[0];
        matrix[4, 7] = red[1];
        matrix[7, 7] = red[2];
        ally = new List<GameObject>(blue);
        enemy = new List<GameObject>(red);
        ShowChessIdle();
    }

    private void ShowChessIdle()
    {
        foreach (GameObject item in chessIdleContainer)
        {
            Destroy(item);
        }
        foreach (GameObject a in ally)
        {
            if (!a.Equals(firstChess))
            {
               chessIdleContainer.Add(Instantiate(chessIdle,a.transform.position,Quaternion.identity));
            }
        }
    }

    bool inChessboard(Vector2 pos)
    {
        if (pos.x < -0.5f || pos.y < -0.5f || pos.x > 7.5f || pos.y > 7.5f) return false;
        else return true;
    }

    void MoveChess(Vector2 pos)
    {
        if(matrix[(int) pos.x, (int)pos.y] != null)
        {
            enemy.Remove(matrix[(int)pos.x, (int)pos.y]);
            Destroy(matrix[(int)pos.x, (int)pos.y]);
        }
        matrix[(int) pos.x, (int)pos.y] = matrix[(int)selectedCell.x, (int)selectedCell.y];
        matrix[(int)selectedCell.x, (int)selectedCell.y] = null;
        matrix[(int)pos.x, (int)pos.y].transform.position = pos;
        SelectCell(new Vector2(-1, -1));
        if (firstChess == null&&ally.Count>1) firstChess = matrix[(int)pos.x, (int)pos.y];
        else
        {
            firstChess = null;
            //changeTurn
            if (CastlePos.x==4)
            {
                CastlePos = new Vector2(3, 0);
                TeleportPos = new Vector2(4, 0);
            }
            else
            {
                CastlePos = new Vector2(4, 7);
                TeleportPos = new Vector2(3, 7);
            }
            List<GameObject> tmp = ally;
            ally = enemy;
            enemy = tmp;
        }
        ShowChessIdle();
    }
    List<Vector2> GetListTarget(Vector2 pos)
    {
        List<Vector2> res=new List<Vector2>();
        //Huong len
        if(inChessboard(new Vector2(pos.x, pos.y+1))){
            if(matrix[(int)pos.x, (int)pos.y + 1] == null)
            {
                if(inChessboard(new Vector2(pos.x-1, pos.y + 2)))
                {
                    if(ally.IndexOf(matrix[(int)pos.x-1, (int)pos.y + 2]) == -1)
                    {
                        res.Add(new Vector2(pos.x - 1, pos.y + 2));
                    }
                }
                if (inChessboard(new Vector2(pos.x + 1, pos.y + 2)))
                {
                    if (ally.IndexOf(matrix[(int)pos.x + 1, (int)pos.y + 2]) == -1)
                    {
                        res.Add(new Vector2(pos.x + 1, pos.y + 2));
                    }
                }
            }
        }
        //Huong xuong
        if (inChessboard(new Vector2(pos.x, pos.y - 1)))
        {
            if (matrix[(int)pos.x, (int)pos.y - 1] == null)
            {
                if (inChessboard(new Vector2(pos.x - 1, pos.y - 2)))
                {
                    if (ally.IndexOf(matrix[(int)pos.x - 1, (int)pos.y - 2]) == -1)
                    {
                        res.Add(new Vector2(pos.x - 1, pos.y - 2));
                    }
                }
                if (inChessboard(new Vector2(pos.x + 1, pos.y - 2)))
                {
                    if (ally.IndexOf(matrix[(int)pos.x + 1, (int)pos.y - 2]) == -1)
                    {
                        res.Add(new Vector2(pos.x + 1, pos.y - 2));
                    }
                }
            }
        }

        //Huong trai
        if (inChessboard(new Vector2(pos.x-1, pos.y)))
        {
            if (matrix[(int)pos.x-1, (int)pos.y] == null)
            {
                if (inChessboard(new Vector2(pos.x - 2, pos.y - 1)))
                {
                    if (ally.IndexOf(matrix[(int)pos.x - 2, (int)pos.y - 1]) == -1)
                    {
                        res.Add(new Vector2(pos.x - 2, pos.y - 1));
                    }
                }
                if (inChessboard(new Vector2(pos.x -2 , pos.y +1)))
                {
                    if (ally.IndexOf(matrix[(int)pos.x -2, (int)pos.y +1]) == -1)
                    {
                        res.Add(new Vector2(pos.x -2, pos.y +1));
                    }
                }
            }
        }
        //Huong phai
        if (inChessboard(new Vector2(pos.x + 1, pos.y)))
        {
            if (matrix[(int)pos.x + 1, (int)pos.y] == null)
            {
                if (inChessboard(new Vector2(pos.x + 2, pos.y - 1)))
                {
                    if (ally.IndexOf(matrix[(int)pos.x + 2, (int)pos.y - 1]) == -1)
                    {
                        res.Add(new Vector2(pos.x + 2, pos.y - 1));
                    }
                }
                if (inChessboard(new Vector2(pos.x + 2, pos.y + 1)))
                {
                    if (ally.IndexOf(matrix[(int)pos.x + 2, (int)pos.y + 1]) == -1)
                    {
                        res.Add(new Vector2(pos.x + 2, pos.y + 1));
                    }
                }
            }
        }
        return res;
    }
    void SelectCell(Vector2 pos)
    {
        for (int i = huongdi.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(huongdi.transform.GetChild(i).gameObject);
        }
        listHuongdi.Clear();

        selectedCell = pos;

        if (selectedCell != new Vector2(-1, -1)) {
            listHuongdi = GetListTarget(pos);
            foreach (Vector2 v in listHuongdi)
            {
                Instantiate(viTriPrefab, v, Quaternion.identity, huongdi.transform);
            }
        }; 
    }

    void touchBegan(Vector2 pos)
    {
        Vector2 touchPosWorld = Camera.main.ScreenToWorldPoint(pos);
        //Bam ra ngoai ban co
        if (!inChessboard(touchPosWorld)) SelectCell(new Vector2(-1, -1));
        //Bam vao trong ban co
        else
        {
            int _x = Mathf.RoundToInt(touchPosWorld.x);
            int _y = Mathf.RoundToInt(touchPosWorld.y);
            //Debug.Log(_x + "," + _y);
            //Chua chon
            if (selectedCell.x == -1)
             {
                 //chon vao quan minh
                 if (ally.IndexOf(matrix[_x, _y]) != -1&& !matrix[_x, _y].Equals(firstChess))
                 {
                     SelectCell(new Vector2(_x, _y));
                 }
             }
             //Da chon
             else
             {
                //chon vao quan minh
                if (ally.IndexOf(matrix[_x, _y]) != -1 && !matrix[_x, _y].Equals(firstChess))
                {
                    SelectCell(new Vector2(_x, _y));
                }
                //Cac huong di
                else if (listHuongdi.IndexOf(new Vector2(_x, _y)) != -1)
                {
                    MoveChess(new Vector2(_x, _y));
                }
                //Bam ra ngoai
                else
                {
                    SelectCell(new Vector2(-1, -1));
                }
            }
             
        }
    }
    void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                touchBegan(Input.GetTouch(0).position);
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            touchBegan(Input.mousePosition);
        }
    }
}
