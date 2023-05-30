using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CustomLightManager : MonoBehaviour
{
    //public Color ambientLight;
    float lastLightTime;

    [Range(0, 1)]
    public float lightTime;

    public static float LightTime;

    public Gradient ambientLight;
    public Gradient lightColor;
    public AnimationCurve lightIntensity;

    public Light mainLight;

    // public bool flipMainLightDirHalfway;
    public Vector2 firstLightDir;
    public Vector2 secondaryLightDir;

    public float lightRotation;

    public float speed;
    // float lightAdd;

    // Start is called before the first frame update
    void Start()
    {
        // lightAdd = -0.25f;
    }

    // Update is called once per frame
    void Update()
    {
        //lightAdd += speed * Time.deltaTime;
        //lightTime = SCurve(Mathf.Clamp01(lightAdd), 0.65f);

        if (lightTime == lastLightTime || mainLight == null) return;

        Refresh();

        lastLightTime = lightTime;
    }


    private void OnValidate()
    {
        Refresh();
    }

    public void Refresh()
    {
        Shader.SetGlobalColor("_AmbientA", ambientLight.Evaluate(lightTime));

        LightTime = lightTime;

        if (mainLight == null) return;

        mainLight.color = lightColor.Evaluate(lightTime);
        mainLight.intensity = Mathf.Max(0f, lightIntensity.Evaluate(lightTime));

        //if ((lastLightTime < 0.5f) != (lightTime < 0.5f))
        //    if (flipMainLightDirHalfway && lightTime > 0.5f)
        //        mainLight.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        //    else
        //        mainLight.transform.localRotation = Quaternion.identity;
        Vector2 lightDirAdd = (lightTime > 0.5f ? secondaryLightDir : firstLightDir);
        mainLight.transform.localRotation = Quaternion.Euler(new Vector3(lightTime * lightRotation + lightDirAdd.x, lightDirAdd.y, 0f));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = lightColor != null ? lightColor.Evaluate(lightTime) : Color.white;
        Gizmos.DrawCube(transform.position, Vector3.one);
        if (mainLight != null) Gizmos.DrawLine(transform.position, transform.position + mainLight.transform.forward * 2f);
    }

    public static float SCurve(float x, float k)
    {
        x = (x * 2f) - 1f;
        if (x < 0f) {
            x = Mathf.Abs(1f + x);
            return ((k * x) / (k - x + 1f)) * 0.5f;
        }
        else {
            k = -1f - k;
            return 0.5f + ((k * x) / (k - x + 1f)) * 0.5f;
        }
    }
}
