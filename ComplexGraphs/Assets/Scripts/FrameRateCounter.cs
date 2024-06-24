using System;
using UnityEngine;
using TMPro;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI display;
    
    public enum DisplayMode { FPS, MS }

    [SerializeField] DisplayMode displayMode = DisplayMode.FPS;
    
    [SerializeField, Range(0.2f, 2f)] float sampleDuration = 1f;

    int frames;
    float duration, bestDuration = float.MaxValue, worstDuration;
    void Update() {
        float frameDuration = Time.unscaledDeltaTime;
        frames += 1;
        duration += frameDuration;

        if (frameDuration < bestDuration) {
            bestDuration = frameDuration;
        }
        if (frameDuration > worstDuration) {
            worstDuration = frameDuration;
        }

        if (duration >= sampleDuration) {
            if (displayMode == DisplayMode.FPS) {
                display.SetText("FPS\n{0:0}\n{1:0}\n{2:0}", frames / duration, 1f / bestDuration, 1f / worstDuration);
            } else {
                display.SetText("MS\n{0:1}\n{1:1}\n{2:1}", 1000f * duration / frames, 1000f * bestDuration, 1000f * worstDuration);
            }
            duration = 0f;
            frames = 0;
            bestDuration = float.MaxValue;
            worstDuration = 0f;
        }
    }
}