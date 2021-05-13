using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ReplayControl : MonoBehaviour
{
    private Label tickLabel;

    private Button prevButton;
    private Button playPauseButton;
    private Button nextButton;

    private Slider slider;

    public Action<int> OnPlayPause;
    public Action<int> OnPrevious;
    public Action<int> OnNext;

    public Action<int> OnSliderTick;

    public Action<int> OnTick;

    private bool isPlaying = false;

    private Coroutine tickIncrementer;

    private int ticksPerScond = 10;

    private void OnEnable()
    {
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
        
        tickLabel= rootVisualElement.Q<Label>("TickLabel");

        prevButton = rootVisualElement.Q<Button>("PrevButton");
        playPauseButton = rootVisualElement.Q<Button>("PlayPauseButton");
        playPauseButton.text = "Play";

        nextButton = rootVisualElement.Q<Button>("NextButton");
        slider = rootVisualElement.Q<Slider>("TickSlider");
        //slider.pageSize = 10;
        slider.value = 0;
        slider.highValue = 2000;

        //playPauseButton.clicked += () =>
        //{
        //    // TODO: raise event
        //    Debug.Log("Play/Pause was clicked");
        //};

        prevButton.RegisterCallback<ClickEvent>(ev =>
        {
            Debug.Log("Prev was clicked");
            slider.SetValueWithoutNotify(slider.value - 1);
            UpdateTickLabel(slider.value);
            OnPrevious?.Invoke((int)slider.value);
        });

        playPauseButton.RegisterCallback<ClickEvent>(ev =>
        {
            Debug.Log("Play/Pause was clicked");
            OnPlayPause?.Invoke((int)slider.value);

            if (!isPlaying)
            {
                playPauseButton.text = "Pause";
                tickIncrementer = StartCoroutine(AutoIncrementTick());
            }
            else {
                playPauseButton.text = "Play";
                StopCoroutine(tickIncrementer);
            }

            isPlaying = !isPlaying;

        });

        nextButton.RegisterCallback<ClickEvent>(ev =>
        {
            Debug.Log("Next was clicked");
            slider.SetValueWithoutNotify(slider.value + 1);
            UpdateTickLabel(slider.value);
            OnNext?.Invoke((int)slider.value);
        });

        slider.RegisterCallback<ClickEvent>(ev =>
        {
            Debug.Log("slider was clicked " + slider.value + " " + slider.pageSize);
            OnSliderTick?.Invoke((int)slider.value);

        });
    }

    private IEnumerator AutoIncrementTick()
    {
        while (slider.value < slider.highValue)
        {
            slider.SetValueWithoutNotify(slider.value + 1);
            UpdateTickLabel(slider.value);
            Debug.Log("Ticking:  " + slider.value);
            OnTick?.Invoke((int)slider.value);
            yield return new WaitForSecondsRealtime(1 / ticksPerScond);
        }
    }

    public void SetMaxTick(float tick)
    {
        slider.highValue = tick;
        UpdateTickLabel(slider.value);
    }

    public void SetCurrentTick(float tick)
    {
        slider.SetValueWithoutNotify(tick);
        UpdateTickLabel(tick);
    }

    private void UpdateTickLabel(float tick)
    {
        tickLabel.text = $"{tick}/{slider.highValue}";
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
