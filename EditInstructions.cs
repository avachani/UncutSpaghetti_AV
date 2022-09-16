using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Transactions;
using System.Linq;

public class EditInstructions : MonoBehaviour, IPointerDownHandler
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
    }

    GameObject prevSquare = null;

    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject square = eventData.pointerEnter;

        if (square)
        {
            if (square.tag == "Square")
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
                    square.GetComponent<Image>().color = new Color32(255, 207, 112, 255);

                    prevSquare = null;
                }
                
            }
        }
    }
}