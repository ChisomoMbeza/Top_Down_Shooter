using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class The_Player : Entity
{
    
    private int level;
    private float currentLevelExperience;
    private float experienceToLevel;

    private GameUI GameUI;


    private void Start()
    {

        GameUI = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameUI>();
        LevelUp();

    }

    public void Add_Experience(float xp)
    {
        currentLevelExperience += xp;
        if (currentLevelExperience >= experienceToLevel)
        {
            currentLevelExperience -= experienceToLevel;
            LevelUp();
        }
        GameUI.Show_Experience(currentLevelExperience / experienceToLevel, level);
    }

    private void LevelUp()
    {
        level++;
        experienceToLevel = level * 50 + Mathf.Pow(level * 2,2);

        Add_Experience(0);
    }
}
