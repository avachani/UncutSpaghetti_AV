using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InformationManager : MonoBehaviour
{
    // Start is called before the first frame update
    public List<int[,]> SavedGrids = new List<int[,]> { };
    public List<Color[,]> SavedGridsColor = new List<Color[,]> { };

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene("Title");
    }

    public void SaveGrid(GameObject[,] grid)
    {
        int w = grid.GetLength(0);
        int h = grid.GetLength(1);

        int[,] newSave = new int[w, h];
        Color[,] newSaveColor = new Color[w, h];

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                newSave[x, y] = int.Parse(grid[x, y].transform.GetChild(0).gameObject.GetComponent<Text>().text);
                newSaveColor[x, y] = grid[x, y].GetComponent<Image>().color;
            }
        }

        SavedGrids.Add(newSave);
        SavedGridsColor.Add(newSaveColor);
    }
}
