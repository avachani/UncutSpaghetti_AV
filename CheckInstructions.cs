using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Transactions;
using System.Linq;

public class CheckInstructions : MonoBehaviour
{
    private GameObject[,] grid = new GameObject[0, 0];
    
    int w = 3;
    int h = 4;

    void Start()
    {
        MakeGrid();
    }

    public void MakeGrid()
    {

        

        grid = new GameObject[w, h];


        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                grid[x, y] = (GameObject)Instantiate(Resources.Load("Square"), transform);
                grid[x, y].transform.position = new Vector2(x - (w - 1) / 2f + 5.25f, -y + (h - 1) / 2f + 0.5f);


                grid[x, y].transform.GetChild(0).gameObject.GetComponent<Text>().text = (x + y * w).ToString();
            }
        }

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                Check(grid[x, y]);
            }
        }
    }

    List<GameObject> spaghetti = new List<GameObject>();

    public void Check(GameObject square)
    {
        spaghetti.Clear();

        spaghetti.Add(square);

        while (!End())
        {

            GameObject nextSquare = lowestNeighbor(spaghetti[spaghetti.Count - 1]);
            spaghetti.Add(nextSquare);
        }
    }

    private bool End()
    {
        spaghetti[0].GetComponent<Image>().color = new Color32(134, 154, 39, 255);
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (!spaghetti.Contains(grid[x, y]))
                {
                    spaghetti[0].GetComponent<Image>().color = Color.gray;
                }
            }
        }

        if (lowestNeighbor(spaghetti[spaghetti.Count - 1]) == null && spaghetti[0].GetComponent<Image>().color != new Color32(134, 154, 39, 255))
        {
            spaghetti[0].GetComponent<Image>().color = new Color32(215, 67, 67, 255);
            return true;
        }
        else if (spaghetti[0].GetComponent<Image>().color == new Color32(134, 154, 39, 255))
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    bool editing = false;
    GameObject prevSquare = null;

    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject square = eventData.pointerEnter;

        if (square)
        {
            if (square.tag == "Square")
            {
                if (editing)
                {
                    if (prevSquare == null)
                    {
                        prevSquare = square;
                        prevSquare.GetComponent<Image>().color = new Color32(103, 181, 224, 255);
                    }
                    else
                    {
                        string prevNumber = prevSquare.transform.GetChild(0).gameObject.GetComponent<Text>().text;
                        string curNumber = square.transform.GetChild(0).gameObject.GetComponent<Text>().text;
                        prevSquare.transform.GetChild(0).gameObject.GetComponent<Text>().text = curNumber;
                        square.transform.GetChild(0).gameObject.GetComponent<Text>().text = prevNumber;

                        prevSquare.GetComponent<Image>().color = new Color32(255, 207, 112, 255);

                        prevSquare = null;

                        for (int x = 0; x < w; x++)
                        {
                            for (int y = 0; y < h; y++)
                            {
                                Check(grid[x, y]);
                            }
                        }
                    }
                }
            }
        }
    }

    private GameObject lowestNeighbor(GameObject square)
    {
        Tuple<int, int> coordinates = CoordinatesOf<GameObject>(grid, square);

        GameObject[] neighbors = new GameObject[4];

        int x = coordinates.Item1;
        int y = coordinates.Item2;

        if (x != 0)
        {
            if (!spaghetti.Contains(grid[x - 1, y]))
            {
                neighbors[0] = grid[x - 1, y];
            }
        }
        if (y != h - 1)
        {
            if (!spaghetti.Contains(grid[x, y + 1]))
            {
                neighbors[1] = grid[x, y + 1];
            }
        }
        if (x != w - 1)
        {
            if (!spaghetti.Contains(grid[x + 1, y]))
            {
                neighbors[2] = grid[x + 1, y];
            }
        }
        if (y != 0)
        {
            if (!spaghetti.Contains(grid[x, y - 1]))
            {
                neighbors[3] = grid[x, y - 1];
            }
        }

        int lowestNum = w * h;
        GameObject lowestSquare = null;

        foreach (GameObject neighbor in neighbors)
        {
            if (neighbor)
            {
                int currentNum = int.Parse(neighbor.transform.GetChild(0).gameObject.GetComponent<Text>().text);
                if (currentNum < lowestNum)
                {
                    lowestNum = currentNum;
                    lowestSquare = neighbor;
                }
            }
        }

        return lowestSquare;
    }

    private Tuple<int, int> CoordinatesOf<T>(T[,] matrix, T value)
    {
        int w = matrix.GetLength(0); // width
        int h = matrix.GetLength(1); // height

        for (int x = 0; x < w; ++x)
        {
            for (int y = 0; y < h; ++y)
            {
                if (matrix[x, y].Equals(value))
                    return Tuple.Create(x, y);
            }
        }

        return Tuple.Create(-1, -1);
    }

}