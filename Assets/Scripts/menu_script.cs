using UnityEngine;
using UnityEngine.SceneManagement;

public class menu_script : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayClicked()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }
}
