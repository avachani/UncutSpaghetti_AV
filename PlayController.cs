using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Transactions;
using System.Linq;
using System.Security.Cryptography;

public class PlayController : MonoBehaviour, IPointerDownHandler
{
    private GameObject[,] grid = new GameObject[0,0];

    [SerializeField]
    public Text ErrorMessage = null;

    [SerializeField]
    public Slider WidthSlider = null;
    [SerializeField]
    public Slider HeightSlider = null;
    
    Vector2 scale;

    [SerializeField]
    public Text Size = null;

    GameObject sauce;

    void Start()
    {
        LoadDropdown.options.Clear();
        for (int i = 0; i < GameObject.Find("InformationHolder").GetComponent<InformationManager>().SavedGrids.Count(); i++)
        {
            Dropdown.OptionData newData = new Dropdown.OptionData();
            newData.text = "Load Grid " + (i).ToString();
            LoadDropdown.options.Add(newData);
        }
        MakeGrid();
    }

    public void MakeGrid()
    {
        if(sauce) Destroy(sauce);
        sauce = null;

        Size.text = WidthSlider.value.ToString() + " x " + HeightSlider.value.ToString();
        
        foreach (GameObject square in grid)
        {
            Destroy(square);
        }
        foreach (GameObject arrow in arrows)
        {
            Destroy(arrow);
        }

        spaghetti.Clear();
        arrows.Clear();

        int w = (int) WidthSlider.value;
        int h = (int) HeightSlider.value;

        grid = new GameObject[w, h];


        if (w > h)
        {
            scale = new Vector2(7f / w, 7f / w);
        }
        else
        {
            scale = new Vector2(7f / h, 7f / h);
        }

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                grid[x, y] = (GameObject)Instantiate(Resources.Load("Square"), transform);
                grid[x, y].transform.position = new Vector2(x- (w-1)/2f + 1, -y+ (h-1)/2f) * scale;

                grid[x, y].transform.localScale = scale;

                grid[x, y].transform.GetChild(0).gameObject.GetComponent<Text>().text = (x + y * w).ToString();
                grid[x, y].transform.GetChild(2).gameObject.GetComponent<Text>().text = "";
            }
        }
    }

    List<GameObject> spaghetti = new List<GameObject>();
    List<GameObject> arrows = new List<GameObject>();

    bool editing = false;
    GameObject prevSquare = null;

    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject square = eventData.pointerEnter;

        if (square)
        {
            if (square.tag == "Square") 
            {
                if (!editing)
                {
                    if (spaghetti.Count == 0)
                    {
                        spaghetti.Add(square);
                        square.transform.GetChild(0).GetComponent<Text>().fontSize = 20;
                        square.transform.GetChild(0).transform.position += (Vector3) (new Vector3(0.25f, 0.25f) * scale);

                        sauce = (GameObject)Instantiate(Resources.Load("Sauce"), transform);
                        sauce.transform.position = square.transform.position;
                        sauce.transform.localScale = scale;
                    }
                    else
                    {
                        int[] lowestSquare = lowestNeighbor(spaghetti[spaghetti.Count - 1]);
                        if (lowestSquare[0] != -1)
                        {
                            Tuple<int, int> currentCoodinates = CoordinatesOf<GameObject>(grid, square);
                            Tuple<int, int> previousCoodinates = CoordinatesOf<GameObject>(grid, spaghetti[spaghetti.Count - 1]);

                            if (square == grid[lowestSquare[0], lowestSquare[1]])
                            {
                                spaghetti.Add(square);
                                square.transform.GetChild(0).GetComponent<Text>().fontSize = 25;
                                square.transform.GetChild(0).transform.position += (Vector3)(new Vector3(0.25f, 0.25f) * scale);

                                GameObject arrow = (GameObject)Instantiate(Resources.Load("Arrow"), transform);

                                Vector3 offset = new Vector3(0, 0, 0);

                                if (lowestSquare[2] == 0)
                                {
                                    offset = new Vector3(0.5f, 0, 0) * scale;
                                }
                                if (lowestSquare[2] == 1)
                                {
                                    offset = new Vector3(0, 0.5f, 0) * scale;
                                }
                                if (lowestSquare[2] == 2)
                                {
                                    offset = new Vector3(-0.5f, 0, 0) * scale;
                                }
                                if (lowestSquare[2] == 3)
                                {
                                    offset = new Vector3(0, -0.5f, 0) * scale;
                                }

                                arrow.transform.position = square.transform.position + offset;
                                arrow.transform.Rotate(new Vector3(0, 0, lowestSquare[2] * 90));

                                arrow.transform.localScale = scale;

                                arrows.Add(arrow);
                                ErrorMessage.text = "";
                            }
                            else if (spaghetti.Contains(square))
                            {
                               
                                
                                ErrorMessage.text = "This square has already been chosen";
                                
                            }
                            else if (Math.Abs(currentCoodinates.Item1 - previousCoodinates.Item1) + Math.Abs(currentCoodinates.Item2 - previousCoodinates.Item2) > 1)
                            {
                                ErrorMessage.text = "This square is not next to the previous one";
                            }
                            else
                            {
                                ErrorMessage.text = "This square is not the lowest square";
                            }
                        }
                    }
                }
                else 
                {
                    if(prevSquare == null)
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

                        prevSquare.transform.GetChild(2).gameObject.GetComponent<Text>().text = "";
                        square.transform.GetChild(2).gameObject.GetComponent<Text>().text = "";

                        prevSquare = null;
                    }
                }
            }
        }

        if (spaghetti.Count > 0)
        {
            string end = "win";
            for (int x = 0; x < WidthSlider.value; x++)
            {
                for (int y = 0; y < HeightSlider.value; y++)
                {
                    if (!spaghetti.Contains(grid[x,y]))
                    {
                        end = "?";
                        break;
                    }
                }
            }

            if (lowestNeighbor(spaghetti[spaghetti.Count - 1])[0] == -1 && end != "win")
            {
                end = "lose";
            }

            if(end == "win")
            {
                spaghetti[0].GetComponent<Image>().color = new Color32(134, 154, 39, 255);
                spaghetti[0].transform.GetChild(2).gameObject.GetComponent<Text>().text = "✓";
            }
            else if(end == "lose")
            {
                spaghetti[0].GetComponent<Image>().color = new Color32(215, 67, 67, 255);
                spaghetti[0].transform.GetChild(2).gameObject.GetComponent<Text>().text = "X";
            }
            if(end != "?")
            {
                Destroy(sauce);
                sauce = null;
            }
        }
    }

    private int[] lowestNeighbor(GameObject square)
    {
        Tuple<int,int> coordinates = CoordinatesOf<GameObject>(grid, square);

        GameObject[] neighbors = new GameObject[4];

        int x = coordinates.Item1;
        int y = coordinates.Item2;

        if (x != 0)
        {
            if(!spaghetti.Contains(grid[x - 1, y]))
            {
                neighbors[0] = grid[x - 1, y];
            }
        }
        if (y != HeightSlider.value-1)
        {
            if (!spaghetti.Contains(grid[x, y + 1]))
            {
                neighbors[1] = grid[x, y + 1];
            }
        }
        if (x != WidthSlider.value-1)
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

        int lowestNum = (int) WidthSlider.value * (int) HeightSlider.value;
        GameObject lowestSquare = null;

        foreach(GameObject neighbor in neighbors)
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
        
        Tuple<int, int> lowestCoordinates = CoordinatesOf<GameObject>(grid, lowestSquare);

        int[] toReturn = new int[3];

        if (lowestSquare)
        {
            toReturn = new int[] { lowestCoordinates.Item1, lowestCoordinates.Item2, Array.IndexOf(neighbors, lowestSquare)};
        }
        else
        {
            toReturn = new int[] { -1, -1, -1 };
        }

        return toReturn;
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

    [SerializeField]
    Text EditText = null;

    public void EditButton()
    {
        if (spaghetti.Count == 0) { 
            editing = !editing;
        }

        if (editing)
        {
            EditText.text = "Stop Editing"; 
            ErrorMessage.text = "Currently Editing the Grid";
        }
        else
        {
            EditText.text = "Edit Grid";
            ErrorMessage.text = "";
        }
    }

    public void Randomize()
    {
        MakeGrid();
        if (spaghetti.Count == 0)
        {
            List<int> numbers = new List<int>();

            for (int i = 0; i < WidthSlider.value * HeightSlider.value; i++)
            {
                numbers.Add(i);
            }

            numbers = numbers.OrderBy(x => UnityEngine.Random.value).ToList();

            for (int x = 0; x < WidthSlider.value; x++)
            {
                for (int y = 0; y < HeightSlider.value; y++)
                {
                    grid[x, y].transform.GetChild(0).gameObject.GetComponent<Text>().text = numbers[x + y * (int)WidthSlider.value].ToString();
                }
            }
        }
    }

    public void TryAgainButton()
    {
        foreach (GameObject square in spaghetti)
        {
            square.transform.GetChild(0).gameObject.GetComponent<Text>().fontSize = 60;
            square.transform.GetChild(0).position = square.transform.position;
        }
        foreach (GameObject arrow in arrows)
        {
            Destroy(arrow);
        }
        Destroy(sauce);
        sauce = null;
        arrows.Clear();
        spaghetti.Clear();
    }

    public void CheckPathButton()
    {

    }

    [SerializeField]
    Dropdown LoadDropdown = null;

    public void SaveGrid()
    {
        GameObject.Find("InformationHolder").GetComponent<InformationManager>().SaveGrid(grid);
        Dropdown.OptionData newData = new Dropdown.OptionData();
        newData.text = "Load Grid " + (LoadDropdown.options.Count()).ToString();
        LoadDropdown.options.Add(newData);
    }

    public void LoadGrid()
    {
        int index = LoadDropdown.value;

        int[,] newGridNumbers = GameObject.Find("InformationHolder").GetComponent<InformationManager>().SavedGrids[index];
        Color[,] newGridColors = GameObject.Find("InformationHolder").GetComponent<InformationManager>().SavedGridsColor[index];

        int w = newGridNumbers.GetLength(0);
        int h = newGridNumbers.GetLength(1);

        WidthSlider.value = w;
        HeightSlider.value = h;

        MakeGrid();

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                grid[x, y].transform.GetChild(0).gameObject.GetComponent<Text>().text = newGridNumbers[x, y].ToString();
                grid[x, y].GetComponent<Image>().color = newGridColors[x, y];
                
                if (grid[x, y].GetComponent<Image>().color == new Color32(134, 154, 39, 255))
                {
                    grid[x, y].transform.GetChild(2).gameObject.GetComponent<Text>().text = "✓";
                }
                else if(grid[x, y].GetComponent<Image>().color == new Color32(215, 67, 67, 255)){
                    grid[x, y].transform.GetChild(2).gameObject.GetComponent<Text>().text = "X";
                }
            }
        }
    }
}