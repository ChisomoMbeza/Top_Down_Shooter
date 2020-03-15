using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{

    [Header("User Experience")]
    public Image Exp_Progress_Bar;
    public Text Level_Text;

    [Header("Ammo Count")]
    public Text Ammo_Count;

    private The_Player Player;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<The_Player>();
    }

    public void Show_Experience(float percentageLevel, int playerLevel)
    {
        Level_Text.text = "Level: " +  playerLevel.ToString();
        Exp_Progress_Bar.fillAmount = percentageLevel;
    }

    public void Show_Ammo(int Current_Ammo, int Total_Ammo)
    {
        Ammo_Count.text = "Ammo: " + Current_Ammo + "/" + Total_Ammo;
    }
}
