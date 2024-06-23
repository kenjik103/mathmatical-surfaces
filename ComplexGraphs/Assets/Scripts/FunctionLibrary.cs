using UnityEngine;
using static UnityEngine.Mathf; 
public static class FunctionLibrary
{
    public delegate float Function(float x, float t);

    public enum FunctionName
    {
        Wave, MulitWave, Ripple
    }

    static Function[] functions = {Wave, MultiWave, Ripple};
    
    public static Function GetFunction(FunctionName name) {
        return functions[(int)name];
    }
    
    public static float Wave(float x, float t) {
        return Sin(Mathf.PI * (x + t));
    }
    
    public static float MultiWave(float x, float t) {
        float y = Sin(Mathf.PI * (x + 0.5f * t));
        y += Sin(2f * PI * (x + t)) * 0.5f;
        return y * (2f/3f); //scale function down to fit (-1,1) range 
    }

    public static float Ripple(float x, float t) {
        float d = Abs(x);
        float y = Sin( PI * (4f * d - t));
        return y / (1f + 10f * d);
    }
}