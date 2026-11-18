using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class tutorial_controller : MonoBehaviour
{
    private TextMeshPro[] components;

    public bool HideTutorial = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        components = gameObject.GetComponentsInChildren<TextMeshPro>();
        foreach (TextMeshPro component in components) component.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator HideText()
    {
        for (float i = 1; i > 0; i-=0.05f)
        {
            foreach (TextMeshPro component in components)
                component.alpha = i;
            yield return new WaitForSeconds(0.1f);
        }
        foreach (TextMeshPro component in components) component.enabled = false;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (HideTutorial) return;
        
        if (collision.gameObject.name == "Player")
        {
            foreach (TextMeshPro component in components) component.enabled = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (HideTutorial) return;

        if (collision.gameObject.name == "Player")
        {
            HideTutorial = true;
            StartCoroutine(HideText());
        }
    }
}
