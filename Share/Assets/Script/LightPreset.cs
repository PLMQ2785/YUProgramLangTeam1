using UnityEngine;

//메뉴에서 "create" 옵션을 추가하여 우클릭 시 (Create > Scriptables > Light Preset) 생성할 수 있도록 함
[System.Serializable]
[CreateAssetMenu(fileName = "Light Preset", menuName = "Scriptables/Light Preset", order = 1)]
public class LightPreset : ScriptableObject
{
    public Gradient AmbientColour;
    public Gradient DirectionalColour;
    public Gradient FogColour;
}