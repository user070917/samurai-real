using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HUD : MonoBehaviour
{
    public enum InfoType {Health, Time};
    public InfoType type;

    Slider mySlider;

    void Awake()
    {
        mySlider = GetComponent<Slider>();
    }

    private void LateUpdate()
    {
        switch(type)
        {
            case InfoType.Health: // 체력바 처리 
                {
                    float curHealth = GameManager.instance.health;
                    float maxHealth = GameManager.instance.maxhealth;
                    mySlider.value = curHealth / maxHealth;
                    break;
                }
        }
    }
}
