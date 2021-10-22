using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Germs : MonoBehaviour
{
    Vector2 firstPosTouch;
    Vector2 finaltPosTouch;
    public Vector2 temp;

    public bool checkMatched = false;

    public int  moveX;
    public int  moveY;

    public int lastColumn;
    public int lastRow;

    public int germColumn;
    public int germRow;
    public GameObject otherGerm;
    public boardCreateScript Board;

    public float swipeAngle = 0;
    public float swipeResist = 1f;


    // Start is called before the first frame update
    void Start()
    {
        Board = FindObjectOfType<boardCreateScript>();
        moveX = (int)transform.position.x;
        moveY = (int)transform.position.y;
        germRow = moveY;
        germColumn = moveX;
        lastRow = germRow;
        lastColumn = germColumn;
    }

    private void OnMouseDown()
    {
        if(Board.Score <= Board.maxScore && Board.Pause == false)
        firstPosTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
  
    }

    private void OnMouseUp()
    {
        if (Board.Score <= Board.maxScore && Board.Pause == false)
        {
            finaltPosTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            angleCal();
        }
    }

    public IEnumerator checkMove()
    {
        yield return new WaitForSeconds(.5f);

        if (otherGerm != null) { 
            if (!checkMatched && !otherGerm.GetComponent<Germs>().checkMatched)
            {
                otherGerm.GetComponent<Germs>().germRow = germRow;
                otherGerm.GetComponent<Germs>().germColumn = germColumn;
                germRow = lastRow;
                germColumn = lastColumn;
            }
            else
            {
                Board.destroyMatchGerm();
            }
            otherGerm = null;
    }
    }

    private void angleCal()
    {
        if (Mathf.Abs(finaltPosTouch.y - firstPosTouch.y) > swipeResist || Mathf.Abs(finaltPosTouch.x - firstPosTouch.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finaltPosTouch.y - firstPosTouch.y, finaltPosTouch.x - firstPosTouch.x) * 180 / Mathf.PI;

            moveGerms();
        }
    }
    void moveGerms()
    {
          if ((swipeAngle > 135 || swipeAngle <= -135) && germColumn > 0)
        {
            otherGerm = Board.listOfGerms[germColumn - 1, germRow];
            otherGerm.GetComponent<Germs>().germColumn += 1;
            germColumn -= 1;
        }

        else if (swipeAngle > 45 && swipeAngle <= 135 && germRow < Board.height - 1)
        {
            otherGerm = Board.listOfGerms[germColumn, germRow + 1];
            otherGerm.GetComponent<Germs>().germRow -= 1;
            germRow += 1;
        }

       

        else if (swipeAngle < -45 && swipeAngle >= -135 && germRow > 0)
        {
            otherGerm = Board.listOfGerms[germColumn, germRow - 1];
            otherGerm.GetComponent<Germs>().germRow += 1;
            germRow -= 1;
        }
        else if(swipeAngle > -45 && swipeAngle <= 45 && germColumn < Board.width - 1)
        {
            otherGerm = Board.listOfGerms[germColumn + 1, germRow];
            otherGerm.GetComponent<Germs>().germColumn -= 1;
            germColumn += 1;
        }
        StartCoroutine(checkMove());

    }

    void findMatch()
    {
        if(germColumn > 0 && germColumn < Board.width -1 )
        {
            GameObject left = Board.listOfGerms[germColumn - 1, germRow];
            GameObject right = Board.listOfGerms[germColumn + 1, germRow];
            if (left != null && right != null && left != this.gameObject && right != this.gameObject)
            {
                if (left.tag == this.gameObject.tag && right.tag == this.gameObject.tag)
                {
                    right.GetComponent<Germs>().checkMatched = true;
                    left.GetComponent<Germs>().checkMatched = true;
                    checkMatched = true;
                }
            }
        }
        if (germRow > 0 && germRow < Board.height - 1 )
        {
            GameObject up = Board.listOfGerms[germColumn, germRow + 1];
            GameObject down = Board.listOfGerms[germColumn, germRow - 1];
            if (up != null && down != null && up != this.gameObject && down != this.gameObject)
            {
                if (up.tag == this.gameObject.tag && down.tag == this.gameObject.tag)
                {
                    up.GetComponent<Germs>().checkMatched = true;
                    down.GetComponent<Germs>().checkMatched = true;
                    checkMatched = true;
                }
            }
        }
    }

   

    // Update is called once per frame
    void Update()
    {

        findMatch();
        if(checkMatched)
        {
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            sprite.color = new Color(0, 0, 0, 1);
        }


        moveX = germColumn;
        moveY = germRow;
        if(Mathf.Abs(moveX - transform.position.x) > 0.1)
        {
            temp = new Vector2(moveX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, temp, 0.6f);
            if(Board.listOfGerms[germColumn,germRow] != this.gameObject)
            {
                Board.listOfGerms[germColumn, germRow] = this.gameObject;
            }
        }
        else
        {
            temp = new Vector2(moveX, transform.position.y);
            transform.position = temp;
           
        }
        if (Mathf.Abs(moveY - transform.position.y) > 0.1)
        {
            temp = new Vector2(transform.position.x, moveY);
            transform.position = Vector2.Lerp(transform.position, temp, .4f);
            if (Board.listOfGerms[germColumn, germRow] != this.gameObject)
            {
                Board.listOfGerms[germColumn, germRow] = this.gameObject;
            }
        }
        else
        {
            temp = new Vector2(transform.position.x, moveY);
            transform.position = temp;
      
        }
    }
}
