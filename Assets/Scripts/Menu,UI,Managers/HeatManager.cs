using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
public class HeatManager : MonoBehaviour
{
    //Heat System: 
    //UI Fields
    [SerializeField] private GameObject _heatPanel;
    [SerializeField] private TextMeshProUGUI _heatNumber;

    //Variable Fields
    //the heat timer that counts down when the player does not land hits
    private float CurrentHeatFalloff = 0.0f;
    //current amount of total heat
    private int CurrentHeatNum = 0;
    //player's current level of heat
    private int heatLevel = 0;
    //Rate at which the heat decreases if the player does not land a hit
    [SerializeField] private float HeatFalloff;
    //how much the heat increases on a successful hit
    [SerializeField] private int HeatPerHit = 1; 
    //threshhold to enter heat level 1
    [SerializeField] private int _heatlevel1 = 60;
    //threshhold to enter heat level 2
    [SerializeField] private int _heatlevel2 = 180;
    private PlayerController[] players;

    public void Start()
    {
        players = new PlayerController[2];
        if(GameManagerScript.Instance.IsMultiplayer == true)
        {
            players[0] = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            players[1] = GameObject.FindGameObjectWithTag("Player2").GetComponent<PlayerController>();
        }
        else
        {
            players[0] = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }
        HeatFalloff /= 10;
    }

    public void FixedUpdate()
    {
        CurrentHeatFalloff -= HeatFalloff * Time.deltaTime;
        if (CurrentHeatFalloff <= 0f && _heatPanel.activeSelf)
        {
            ResetHeat();
        }
    }

    public void increaseHeat()
    {
        CurrentHeatNum += HeatPerHit; 
        if(!_heatPanel.activeSelf)
        {
            _heatPanel.SetActive(true);
        }
        _heatNumber.SetText(CurrentHeatNum.ToString());
        if (CurrentHeatNum >= _heatlevel1 && heatLevel == 0)
        {
            heatLevel = 1;
            _heatNumber.color = Color.cyan;
            
        }
        else if (CurrentHeatNum >= _heatlevel2 && heatLevel == 1)
        {
            heatLevel = 2;
            _heatNumber.color = Color.magenta;
        }

        AkSoundEngine.SetRTPCValue("Player_Combo", CurrentHeatNum);
        AkSoundEngine.SetRTPCValue("Heat_Level", heatLevel);
        applyBuffs();

        CurrentHeatFalloff = 1f;
    }

    public int CurrentHeatLevel()
    {
        return heatLevel;
    }

    public void ResetHeat()
    {
        AkSoundEngine.PostEvent("Lose_Combo", gameObject);
        heatLevel = 0;
        applyBuffs();
        CurrentHeatNum = 0;
        _heatNumber.color = Color.white;
        _heatNumber.SetText(CurrentHeatNum.ToString());
        _heatPanel.SetActive(false);
    }

    public void applyBuffs()
    {
        foreach (PlayerController player in players)
        {
            if(player)
            {
                player.BuffPlayer(heatLevel);
            }
        }
    }
}
