using System;
using System.Collections;
using System.Net.Mime;
using UnityEngine;
using Object = UnityEngine.Object;

public class Graph: MonoBehaviour
{
    [SerializeField] 
    Transform pointPrefab;

    [SerializeField, Range(10,100)] 
    int resolution = 10;

    [SerializeField]
    FunctionLibrary.FunctionName function;

    public enum TransitionMode { Cycle, Random }

    [SerializeField] 
    TransitionMode transitionMode;

    [SerializeField, Min(0f)] float functionDuration = 1f;
    
    Transform[] points;
    float duration;
    
    void Awake() {
        float step = 2f / resolution;
        Vector3 scale = Vector3.one * step;
        points = new Transform[resolution * resolution];
        for (int i = 0; i < points.Length; i++) {
            Transform point = points[i] = Instantiate(pointPrefab, transform, false);
            point.localScale = scale;
        }
    }

    void Update() {
        duration += Time.deltaTime;
        if (duration >= functionDuration) {
            duration -= functionDuration;
            PickNextFunction();
        }
        UpdateFunction();
    }

    void PickNextFunction() {
        function = transitionMode == TransitionMode.Cycle ?
            FunctionLibrary.GetNextFunction(function) :
            FunctionLibrary.GetRandomFunctionOtherThan(function);
    }

    void UpdateFunction() {
        FunctionLibrary.Function f = FunctionLibrary.GetFunction(function);
        float time = Time.time;
        float step = 2f / resolution;
        float v = 0.5f * step - 1f;
        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++) {
            if (x == resolution) {
                x = 0;
                z++;
                v = (z + 0.5f) * step - 1f;
            }
            float u = (x + 0.5f) * step - 1f;
            points[i].localPosition = f(u, v, time);
        }
    }
}