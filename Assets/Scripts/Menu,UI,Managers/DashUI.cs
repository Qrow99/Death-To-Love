using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashUI : MonoBehaviour
{
    [SerializeField] private float ChipSpeed = 2f;
    [SerializeField] private Image FrontDashBar;
    [SerializeField] private Image BackDashBar;
    private float LerpTimer;
    private float slideDuration;
    private float slideCooldown;
    private float slideRegenProgress;
    private bool regen = false;

    [SerializeField] private float flashRate = 0.2f;
    [SerializeField] private Color flashColor;
    private float flashTimer;
    private bool colored = false;
    // Start is called before the first frame update
    void Start()
    {
        flashTimer = flashRate;
    }

    // Update is called once per frame
    void Update()
    {
        if(slideRegenProgress >= slideCooldown)
        {
            slideRegenProgress = slideCooldown;
        }
        updateDashUI();
    }

    private void updateDashUI()
    {
        float percentComplete;
        float fillFront = FrontDashBar.fillAmount;
        float fillBack = BackDashBar.fillAmount;
        float percentSlide = slideRegenProgress / slideCooldown;
        if(fillBack > percentSlide)
        {
            FrontDashBar.fillAmount = percentSlide;
            BackDashBar.color = Color.red;
            LerpTimer += Time.deltaTime;
            percentComplete = LerpTimer / ChipSpeed;
            percentComplete *= percentComplete;
            BackDashBar.fillAmount = Mathf.Lerp(fillBack, percentSlide, percentComplete);
        }

        if (regen)
        {
            slideRegenProgress += Time.deltaTime;
            BackDashBar.color = Color.green;
            BackDashBar.fillAmount = percentSlide;
            LerpTimer += Time.deltaTime;
            percentComplete = LerpTimer / ChipSpeed;
            percentComplete *= percentComplete;
            FrontDashBar.fillAmount = Mathf.Lerp(fillFront, BackDashBar.fillAmount, percentComplete);
        }

        if(percentSlide < 1)
        {
            regen = true;
            FrontDashBar.color = Color.white;
        }
        else if(percentSlide == 1)
        {
            regen = false;
            flashTimer -= Time.deltaTime;
            if (colored == false && flashTimer <= 0)
            {
                FrontDashBar.color = flashColor;
                colored = true;
                flashTimer = flashRate;
            }
            else if (colored == true && flashTimer <= 0)
            {
                FrontDashBar.color = Color.white;
                colored = false;
                flashTimer = flashRate;
            }
        }
    }

    public void setSlideDuration(float Duration)
    {
        slideDuration = Duration;
    }

    public void setSlideCooldown(float Cooldown)
    {
        slideCooldown = Cooldown;
        slideRegenProgress = slideCooldown;
    }

    public void Dash()
    {
        slideRegenProgress = 0f;
        LerpTimer = 0f;
    }

}
