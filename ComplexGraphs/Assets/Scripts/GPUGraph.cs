using System;
using UnityEngine;
using UnityEngine.LowLevel;

public class GPUGraph: MonoBehaviour
{
    const int maxResolution = 1000;
    
    static readonly int 
        positionId = Shader.PropertyToID("_Positions"),
        resolutionId = Shader.PropertyToID("_Resolution"),
        stepId = Shader.PropertyToID("_Step"),
        timeId = Shader.PropertyToID("_Time"),
        transitionProgressId = Shader.PropertyToID("_TransitionProgress");
    
    [SerializeField, Range(10,maxResolution)] 
    int resolution = 10;

    [SerializeField]
    FunctionLibrary.FunctionName function;

    public enum TransitionMode { Cycle, Random }

    [SerializeField] 
    TransitionMode transitionMode;

    [SerializeField, Min(0f)] float functionDuration = 1f, transitionDuration = 1f;

    [SerializeField] ComputeShader computeShader;

    [SerializeField] Material material;
    [SerializeField] Mesh mesh;

    
    float duration;
    bool transitioning;
    FunctionLibrary.FunctionName transitionFunction;

    ComputeBuffer positionBuffer;

    void OnEnable() {
        positionBuffer = new ComputeBuffer(maxResolution * maxResolution, 3 * 4); //Vector3: 3 floats; float = 4 bytes
    }

    void OnDisable() {
        positionBuffer.Release();
        positionBuffer = null;
    }

    void UpdateFunctionOnGPU() {
        float step = 2f / resolution;
        computeShader.SetInt(resolutionId, resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time);
        if (transitioning) {
            computeShader.SetFloat(transitionProgressId, Mathf.SmoothStep(0f, 1f, duration/transitionDuration));
        }

        var kernelIndex = (int)function + (int)(transitioning ? transitionFunction : function) * 5;
        computeShader.SetBuffer(kernelIndex, positionId, positionBuffer);

        int groups = Mathf.CeilToInt(resolution / 8f);
        computeShader.Dispatch(kernelIndex,groups,groups,1);
        
        material.SetBuffer(positionId, positionBuffer);
        material.SetFloat(stepId, step);
        Bounds bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, resolution * resolution);
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
        UpdateFunctionOnGPU();
    }

    void PickNextFunction() {
        function = transitionMode == TransitionMode.Cycle ?
            FunctionLibrary.GetNextFunction(function) :
            FunctionLibrary.GetRandomFunctionOtherThan(function);
    }
}