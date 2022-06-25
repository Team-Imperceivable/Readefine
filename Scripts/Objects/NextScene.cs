using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    [Header("Dimensions")]
    [SerializeField] private float height;
    [SerializeField] private float width;
    [SerializeField] int finalLevelNumber;
    // Update is called once per frame
    void Update()
    {
        if(CheckPlayerInZone())
        {
            Debug.Log("Player Reached End!");
            LoadNext();
        }
    }

    private bool CheckPlayerInZone()
    {
        Collider2D collider = Physics2D.OverlapBox(transform.position, new Vector2(width, height), 0);
        if (collider != null && collider.tag.Equals("Player"))
        {
            return true;
        }
        return false;
    }

    private void LoadNext()
    {
        string activeSceneName = SceneManager.GetActiveScene().name;
        string[] sceneNameSplit = activeSceneName.Split('_');
        int levelNumber = System.Convert.ToInt32(sceneNameSplit[1]);
        if(levelNumber < finalLevelNumber)
        {
            activeSceneName = sceneNameSplit[0] + "_" + System.Convert.ToString(levelNumber + 1);
        } else
        {
            if (sceneNameSplit[0].Equals("Tutorial"))
            {
                activeSceneName = "Level_1";
            } else
            {
                activeSceneName = "Credits";
            }
        }
        SceneManager.LoadScene(activeSceneName);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector2(width, height));
    }
}
