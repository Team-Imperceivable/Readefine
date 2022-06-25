using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsScript : MonoBehaviour
{
    [SerializeField] private float delayTime;
    [SerializeField] private float scrollSpeed;
    private RectTransform[] zones;
    private string storedWord = "Thanks For Playing!";

    // Start is called before the first frame update
    void Start()
    {
        zones = GetComponentsInChildren<RectTransform>();
        delayTime += Time.time;
    }

    void Update()
    {
        if(Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2"))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            foreach(RectTransform zone in zones)
            {
                if(zone.rect.Contains(mousePos))
                {
                    Text textBox = zone.gameObject.GetComponent<Text>();
                    string word = textBox.text;
                    textBox.text = storedWord;
                    storedWord = word;
                }
            }
        }

        if(Time.time > delayTime)
        {
            Vector3 change = new Vector3(0f, scrollSpeed, 0f);
            transform.position += change;
        }
    }
}
