using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
public class TransitionEffect : MonoBehaviour
{
    public Camera cam;
    public GameObject effectObj;
    public int xResoultion = 16;
    public int yResolution = 9;
    public float scale;
    public float speed;
    public Vector2Int currIndex;
    public bool isAnimating;

    private List<List<GameObject>> allCircles = new List<List<GameObject>>();
    private bool isTranstingOut = false;

    private void Start()
    {
        for (int y = 0; y < yResolution; y++)
        {
            allCircles.Add(new List<GameObject>());
            for (int x = 0; x < xResoultion; x++)
            {
                Vector3 pos = new Vector3((x / (float)xResoultion) * Screen.width, (y / (float)yResolution) * Screen.height, 10);
                pos = cam.ScreenToWorldPoint(pos);
                allCircles[y].Add(Instantiate(effectObj, pos, Quaternion.identity));
                allCircles[y][x].transform.parent = transform;
                allCircles[y][x].transform.localScale = new Vector3(scale, scale, 1);
            }
        }
        isAnimating = true;
        isTranstingOut = false;
        currIndex = new Vector2Int(0, 0);
    }
    private void Update()
    {
        CheckTransition();
    }
    private void CheckTransition()
    {
        if (isAnimating)
        {
            float targetScale = isTranstingOut ? scale : 0;
            for (int y = 0; y < allCircles.Count; y++)
            {
                for (int x = 0; x < allCircles[y].Count; x++)
                {
                    MoveScale(allCircles[y][x], targetScale);
                }
            }
            if(allCircles[0][0].transform.localScale.x == targetScale)
            {
                isAnimating = false;
                isTranstingOut = !isTranstingOut;
            }
        }
    }
    private void MoveScale(GameObject currObj,float targetScale)
    {
        currObj.transform.localScale = new Vector3(Mathf.MoveTowards(currObj.transform.localScale.x, targetScale, Time.deltaTime * speed),
                    Mathf.MoveTowards(currObj.transform.localScale.x, targetScale, Time.deltaTime * speed), 1);
    }
    public void Transition(bool leave)
    {
        isAnimating = true;
        isTranstingOut = leave;
    }
}
