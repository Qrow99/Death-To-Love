// Contributor(s): Eric Park, Nathan More
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
public class HealthUI : MonoBehaviour
{
    //[SerializeField] private Image healthBar;
    //[SerializeField] private Image[] healthTicks;

    
    [SerializeField] private FloatAsset maxHealth; // Max health stored in float asset
    [SerializeField] private FloatAsset health;
    [SerializeField] private float ChipSpeed = 2f;
    [SerializeField] private Image FrontHealthBar;
    [SerializeField] private Image BackHealthBar;
    private float LerpTimer;
    //[SerializeField] private float lerpSpeed = 3f;

    private float healthBarStep; // Determines how many health points each tick represents

    private void Start()
    { 
        //healthBarStep = maxHealth.Value / healthTicks.Length; // Sets healthBarStep based on max health and num of ticks
    }
    private void Update()
    {
        
        if(health.Value > maxHealth.Value)
        {
            health.Value = maxHealth.Value; //Prevents overhealing 
        }
        updateHealthUI();
        //FillBar();
    }
    private void FillBar()
    {
        /*
        for(int i = 0; i < healthTicks.Length; i++)
        {
            healthTicks[i].enabled = !DisplayHealthPoint(health.Value, i); 
        }*/
    }

    private void updateHealthUI()
    {
        float percentComplete;
        float fillFront = FrontHealthBar.fillAmount;
        float fillBack = BackHealthBar.fillAmount;
        float percentHealth = health.Value / maxHealth.Value;
        
        if(fillBack > percentHealth) //damage was taken
        {
            FrontHealthBar.fillAmount = percentHealth;
            BackHealthBar.color = Color.red;
            LerpTimer += Time.deltaTime;
            percentComplete = LerpTimer / ChipSpeed;
            percentComplete *= percentComplete;
            BackHealthBar.fillAmount = Mathf.Lerp(fillBack, percentHealth ,percentComplete);
            
        }
        
        if(fillFront < percentHealth)
        {
            BackHealthBar.color = Color.green;
            BackHealthBar.fillAmount = percentHealth;
            LerpTimer += Time.deltaTime;
            percentComplete = LerpTimer / ChipSpeed;
            percentComplete *= percentComplete;
            FrontHealthBar.fillAmount = Mathf.Lerp(fillFront, BackHealthBar.fillAmount, percentComplete);
        }
        
        if(fillFront == fillBack)
        {
            LerpTimer = 0f;
        }
    }

    bool DisplayHealthPoint(float health, int pointNumber)
    {
        return ((pointNumber * healthBarStep) >= health);
    }
}
