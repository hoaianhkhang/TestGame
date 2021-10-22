using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class boardCreateScript : MonoBehaviour
{
    public int width;
    public int height;
    public List<GameObject> germ;

    public GameObject[,] listOfGerms;

    public int maxScore;
    public int Score;
    public bool Pause;

    public GameObject PauseButton;
    public GameObject RetryButton;
    public GameObject dimImage;

    public UnityEngine.UI.Text scoreText;

    public void pauseGame()
    {
        if(Pause == false)
        {
            dimImage.SetActive(true);
            RetryButton.SetActive(true);
            Pause = true;
        }
        
        else
        {
            dimImage.SetActive(false);
            RetryButton.SetActive(false);
            Pause = false;
        }
            
    }

    public void retryGame()
    {
        SceneManager.LoadScene("GameScene");
    }


    public void createBoard()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                int randomNumb;
                Vector2 temp = new Vector2(i, j);
                randomNumb = chooseRandomGerm();
                
                int iteration = 0;
                while(matchesAt(i,j,germ[randomNumb]) && iteration < 100)
                {
                    randomNumb = chooseRandomGerm();
                    iteration++;
                }
                GameObject germTile = Instantiate(germ[randomNumb], temp, Quaternion.identity, this.transform) as GameObject;
                iteration = 0;   

                germTile.name = "Germ: (" + i + "; " + j + ")";
                listOfGerms[i, j] = germTile;
            }
        }
    }

    public bool matchesAt(int col, int row, GameObject germ)
    {
        if (col > 1 && row > 1)
        {
            if (listOfGerms[col - 1, row].tag == germ.tag && listOfGerms[col - 2, row].tag == germ.tag)
                return true;
            if (listOfGerms[col, row - 1].tag == germ.tag && listOfGerms[col, row - 2].tag == germ.tag)
                return true;
        }
        else if (col <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (listOfGerms[col, row - 1].tag == germ.tag && listOfGerms[col, row - 2].tag == germ.tag)

                {
                    return true;
                }
            }

            if (col > 1)
            {
                if (listOfGerms[col - 1, row].tag == germ.tag && listOfGerms[col - 2, row].tag == germ.tag)
                {
                    return true;
                }

            }
        }
        return false;
    }

    public void fillBoardAgain()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(listOfGerms[i,j] == null)

                {
                    Vector2 temp = new Vector2(i, j);
                    int randomNumb = chooseRandomGerm();
                    
                            GameObject germTile = Instantiate(germ[randomNumb], temp, Quaternion.identity);                 

                    listOfGerms[i, j] = germTile;
                    germTile.transform.parent = this.transform;
                    germTile.name = "Germ: (" + i + "; " + j + ")";

                }
            }
        }
    }

    private bool findCurrentMatchesOnBoard()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(listOfGerms[i,j] != null)
                {
                    if(listOfGerms[i,j].GetComponent<Germs>().checkMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator fillCooldown()
    {
        fillBoardAgain();
  
        yield return new WaitForSeconds(.4f);
        while (findCurrentMatchesOnBoard())
        {
            yield return new WaitForSeconds(.4f);

            destroyMatchGerm();
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (listOfGerms[x, y] != null)
                {
                    listOfGerms[x, y].GetComponent<Germs>().lastRow = listOfGerms[x, y].GetComponent<Germs>().germRow;
                    listOfGerms[x, y].GetComponent<Germs>().lastColumn = listOfGerms[x, y].GetComponent<Germs>().germColumn;
                    listOfGerms[x, y].transform.parent = this.transform;
                    listOfGerms[x, y].name = "Germ: (" + x + "; " + y+ ")";

                }
            }
        }

    }

    private IEnumerator decreaseRowCooldown()
    {
        int count = 0;
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++)
            {
                if(listOfGerms[i,j] == null)
                {
                    count++;
                }
                else if (count > 0)
                {
                    listOfGerms[i,j].GetComponent<Germs>().germRow -= count;
                    listOfGerms[i, j] = null;
                }
            }
            count = 0;
        }

        yield return new WaitForSeconds(.3f);
        StartCoroutine(fillCooldown());
    }



    public void destroyMatchGermAt(int col, int row)
    {
        if (listOfGerms[col, row].GetComponent<Germs>().checkMatched)
        {
            Destroy(listOfGerms[col, row]);
            listOfGerms[col, row] = null;
            Score++;
            scoreText.text = "Score: " + Score;
            Debug.Log("You have to destroy " + (maxScore - Score) + " Germ left");
        }
    }

    public void destroyMatchGerm()
    {
        for(int i = 0; i < width; i++)
            for(int j = 0; j < height; j++)
            {
                if(listOfGerms[i,j] != null)
                {
                    destroyMatchGermAt(i, j);
                   
                }
            }

        StartCoroutine(decreaseRowCooldown());       
    }

    public void setPrePosition()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (listOfGerms[x, y] != null)
                {
                    listOfGerms[x, y].GetComponent<Germs>().lastRow = listOfGerms[x, y].GetComponent<Germs>().germRow;
                    listOfGerms[x, y].GetComponent<Germs>().lastColumn = listOfGerms[x, y].GetComponent<Germs>().germColumn;
                    listOfGerms[x, y].transform.parent = this.transform;
                    listOfGerms[x, y].name = "Germ: (" + x + "; " + y + ")";

                }
            }
        }
    }

    public int chooseRandomGerm()
    {
        int germChoice = Random.Range(0, 7);
        return germChoice;
    }

    // Start is called before the first frame update
    void Start()
    {
        listOfGerms = new GameObject[width, height];
        createBoard();
        Debug.Log("You have to destroy " + (maxScore - Score) + " Germ left");
    }

    // Update is called once per frame
    void Update()
    {
        if(Score >= maxScore)
        {
            dimImage.SetActive(true);
            RetryButton.SetActive(true);
            Pause = true;
        }

    }
}
