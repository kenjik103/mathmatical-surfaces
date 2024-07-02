using System;
using UnityEngine;
using UnityEngine.LowLevel;

public class GPUGraph: MonoBehaviour
{
    [SerializeField, Range(10,200)] 
    int resolution = 10;

    [SerializeField]
    FunctionLibrary.FunctionName function;

    public enum TransitionMode { Cycle, Random }

    [SerializeField] 
    TransitionMode transitionMode;

    [SerializeField, Min(0f)] float functionDuration = 1f, transitionDuration = 1f;

    [SerializeField] ComputeShader computeShader;

    static readonly int positionId = Shader.PropertyToID("_Position"),
    resolutionId = Shader.PropertyToID("_Resolution"),
    stepId = Shader.PropertyToID("_Step"),
    timeId = Shader.PropertyToID("_Time");
    
    float duration;
    bool transitioning;
    FunctionLibrary.FunctionName transitionFunction;

    ComputeBuffer positionBuffer;

    void OnEnable() {
        positionBuffer = new ComputeBuffer(resolution * resolution, 3 * 4);
    }

    void OnDisable() {
        positionBuffer.Release();
        positionBuffer = null;
    }

    void UpdateGPU() {
        float step = 2f / resolution;
        computeShader.SetInt(resolutionId, resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time);
        
        computeShader.SetBuffer(0, positionId, positionBuffer);

        int groups = Mathf.CeilToInt(resolution / 8f);
        computeShader.Dispatch(0,groups,groups,1);
    }

    void Update() {
        duration += Time.deltaTime;
        if (transitioning && duration >= transitionDuration) {
            duration -= transitionDuration;
            transitioning = false;
        } else if (duration >= functionDuration) {
            duration -= functionDuration;
            transitioning = true;
            transitionFunction = function;
            PickNextFunction();
        }
        UpdateGPU();
    }

    void PickNextFunction() {
        function = transitionMode == TransitionMode.Cycle ?
            FunctionLibrary.GetNextFunction(function) :
            FunctionLibrary.GetRandomFunctionOtherThan(function);
    }
}