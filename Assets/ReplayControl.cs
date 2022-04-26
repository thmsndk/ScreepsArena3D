using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ReplayControl : MonoBehaviour
{
    private Label tickLabel;
    private Slider tickSlider;

    private Button prevButton;
    private Button prev10Button;

    private Button playPauseButton;

    private Button nextButton;
    private Button next10Button;

    private Slider replaySpeedSlider;
    private Label replaySpeedLabel;

    public Action<int> OnPlayPause;
    public Action<int> OnPrevious;
    public Action<int> OnNext;

    public Action<int> OnSliderTick;
    public Action<int> OnTick;

    private bool isPlaying = false;

    private Coroutine tickIncrementer;

    private float ticksPerScond = 2f;//5f;//0.5f; // TODO: change tickspeed from UI

    private void OnEnable()
    {
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
        
        tickLabel= rootVisualElement.Q<Label>("TickLabel");




        tickSlider = rootVisualElement.Q<Slider>("TickSlider");
        //slider.pageSize = 10;
        tickSlider.value = 0;
        tickSlider.highValue = 2000; // can be 10k for advanced arenas, or less if replay ends earlier

        replaySpeedSlider = rootVisualElement.Q<Slider>("ReplaySpeed");
        replaySpeedSlider.lowValue = 0.5f;
        replaySpeedSlider.value = ticksPerScond;

        replaySpeedLabel = replaySpeedLabel.Q<Label>("ReplaySpeedLabel"); ;
        UpdateSpeedReplayLabel(ticksPerScond);

        // TODO: listen on slider value change, and tick if play is not pressed.

        //playPauseButton.clicked += () =>
        //{
        //    // TODO: raise event
        //    Debug.Log("Play/Pause was clicked");
        //};

        prevButton = rootVisualElement.Q<Button>("PrevButton");
        prevButton.RegisterCallback<ClickEvent>(ev =>
        {
            Debug.Log("Prev was clicked");
            tickSlider.SetValueWithoutNotify(Math.Max(0, tickSlider.value - 1));
            UpdateTickLabel(tickSlider.value);
            OnPrevious?.Invoke((int)tickSlider.value);

            Debug.Log("Tick:  " + tickSlider.value);
            OnTick?.Invoke((int)tickSlider.value);
            if (isPlaying)
            {
                isPlaying = false;
                playPauseButton.text = "Play";
                StopCoroutine(tickIncrementer);
            }

        });
        prev10Button = rootVisualElement.Q<Button>("Prev10Button");
        prev10Button?.RegisterCallback<ClickEvent>(ev =>
        {
            Debug.Log("Prev was clicked");
            tickSlider.SetValueWithoutNotify(Math.Max(0, tickSlider.value - 10));
            UpdateTickLabel(tickSlider.value);
            OnPrevious?.Invoke((int)tickSlider.value);

            Debug.Log("Tick:  " + tickSlider.value);
            OnTick?.Invoke((int)tickSlider.value);
            if (isPlaying)
            {
                isPlaying = false;
                playPauseButton.text = "Play";
                StopCoroutine(tickIncrementer);
            }

        });

        nextButton = rootVisualElement.Q<Button>("NextButton");
        nextButton?.RegisterCallback<ClickEvent>(ev =>
        {
            Debug.Log("Next was clicked");
            tickSlider.SetValueWithoutNotify(Math.Min(tickSlider.highValue, tickSlider.value + 1));
            UpdateTickLabel(tickSlider.value);
            OnNext?.Invoke((int)tickSlider.value);

            Debug.Log("Tick:  " + tickSlider.value);
            OnTick?.Invoke((int)tickSlider.value);
        });

        next10Button = rootVisualElement.Q<Button>("Next10Button");
        next10Button?.RegisterCallback<ClickEvent>(ev =>
        {
            Debug.Log("Next was clicked");
            tickSlider.SetValueWithoutNotify(Math.Min(tickSlider.highValue, tickSlider.value + 10));
            UpdateTickLabel(tickSlider.value);
            OnNext?.Invoke((int)tickSlider.value);

            Debug.Log("Tick:  " + tickSlider.value);
            OnTick?.Invoke((int)tickSlider.value);
        });

        playPauseButton = rootVisualElement.Q<Button>("PlayPauseButton");
        playPauseButton.text = "Play";
        playPauseButton.RegisterCallback<ClickEvent>(ev =>
        {
            Debug.Log("Play/Pause was clicked");
            OnPlayPause?.Invoke((int)tickSlider.value);

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

        tickSlider.RegisterCallback<ClickEvent>(ev =>
        {
            Debug.Log("slider was clicked " + tickSlider.value + " " + tickSlider.pageSize);
            OnSliderTick?.Invoke((int)tickSlider.value);
        });


        replaySpeedSlider.RegisterValueChangedCallback(ev =>
        {
            Debug.Log("replaySpeed slider was clicked " + replaySpeedSlider.value);
            ticksPerScond = replaySpeedSlider.value;
            UpdateSpeedReplayLabel(ticksPerScond);
        }); 
    }

    private IEnumerator AutoIncrementTick()
    {
        while (tickSlider.value < tickSlider.highValue)
        {
            Debug.Log("Ticking:  " + tickSlider.value);
            Debug.Log("ReplaySpeed:  " + 1/replaySpeedSlider.value);
            OnTick?.Invoke((int)tickSlider.value);

            yield return new WaitForSecondsRealtime(1 / replaySpeedSlider.value);
            tickSlider.SetValueWithoutNotify(tickSlider.value + 1);
            UpdateTickLabel(tickSlider.value);
        }
    }

    public void SetMaxTick(float tick)
    {
        tickSlider.highValue = tick;
        UpdateTickLabel(tickSlider.value);
    }

    public void SetCurrentTick(float tick)
    {
        tickSlider.SetValueWithoutNotify(tick);
        UpdateTickLabel(tick);
    }

    private void UpdateTickLabel(float tick)
    {
        tickLabel.text = $"{tick}/{tickSlider.highValue}";
    }

    private void UpdateSpeedReplayLabel(float value)
    {
        replaySpeedLabel.text = $"Replay Speed: {value}[t/s]";
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
