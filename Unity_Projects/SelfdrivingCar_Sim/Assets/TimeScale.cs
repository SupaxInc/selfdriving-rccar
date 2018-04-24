using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeScale : MonoBehaviour {
    
    private Slider timeSlider;
    public Text sliderText;
    
	void Start ()
    {
        timeSlider = GetComponent<Slider>();
        timeSlider.value = 1.0f;
        timeSlider.onValueChanged.AddListener(UpdateTimeScale);
        UpdateTimeScale(timeSlider.value);
    }
	
    

    public void UpdateTimeScale(float value)
    {
        Time.timeScale = value;
        sliderText.text = string.Format("{0:0.0}x", value);
    }
}
