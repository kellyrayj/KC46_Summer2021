using UnityEngine;

public class LightControl : MonoBehaviour, IDial
{
    [SerializeField] Light linkedLight;
    [SerializeField] float maxLightIntensity = 20f;
    [SerializeField] float maxMaterialEmission = 1f;
    [SerializeField] Transform lightCoilMaterial;
    [SerializeField] Transform lightBulb;

    public void DialChanged(float dialvalue)
    {
        //Debug.Log(dialvalue);


        var requiredLightPercentage = (dialvalue / 360f) *100f;
        Debug.Log($"percentage power {requiredLightPercentage}");

        var lightIntensity = maxLightIntensity / 100f * requiredLightPercentage;
        var lightmaterialEmission = maxMaterialEmission / 100f * (requiredLightPercentage/4);

        Debug.Log($"light material property {lightmaterialEmission}");
        lightCoilMaterial.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Vector4(15f, 5f, 0f) * lightmaterialEmission);
        lightBulb.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Vector4(15f, 5f, 0f) * lightmaterialEmission/2);

        linkedLight.intensity = lightIntensity;
    }

    
}
