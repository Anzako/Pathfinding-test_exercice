using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI sliderText;
    [SerializeField] private CharactersController characterController;

    void Start()
    {
        slider.onValueChanged.AddListener((v) =>
        {
            sliderText.text = v.ToString("0");
            characterController.ChangeGuideCharacter(Mathf.CeilToInt(v) - 1);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
