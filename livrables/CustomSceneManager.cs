using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CustomSceneManager : MonoBehaviour
{

    public static CustomSceneManager instance;

    private void Awake()
    {
        if (instance != null) Destroy(instance.gameObject); 
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);      
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }



    public void GoToProject(){
        SceneManager.LoadScene("SampleScene");
    }

    public void Quit(){
        Application.Quit();
    }
}
