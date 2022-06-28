using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditsScript : MonoBehaviour
{
    [SerializeField] private float delayTime;
    [SerializeField] private float scrollSpeed;
    [SerializeField] private Canvas parentCanvas;
    [SerializeField] private float endY;
    private RectTransform[] zones;

    // Start is called before the first frame update
    void Start()
    {
        delayTime += Time.time;
    }

    void Update()
    {
        if(Time.time > delayTime)
        {
            Vector3 change = new Vector3(0f, scrollSpeed, 0f);
            transform.position += change;
        }

        if(transform.position.y > endY || Input.GetButtonDown("Cancel"))
        {
            SceneManager.LoadSceneAsync("Menu");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 endPos = new Vector3(0f, -endY, 0f);
        endPos += transform.position;
        Gizmos.DrawWireCube(endPos, parentCanvas.pixelRect.size);
    }
}
