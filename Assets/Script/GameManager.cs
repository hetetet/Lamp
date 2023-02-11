using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    enum jobs { none, tres_hunter, explorer, speleologist, np_staff, env_list };
    jobs job = jobs.none;
    public Button[] buttons;
    public GameObject[] jobSymbols;
    public GameObject panel;
    private void Start()
    {

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    public void selectJob(int index)
    {
        if(index>0)
            jobSymbols[index-1].SetActive(true);
        panel.SetActive(false);
    }
}
