using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(menuName = "Events/Float Event Channel")]
public class FloatEventChannelSO : ScriptableObject
{
    public UnityAction<float> OnEventRaised;

    public void RaiseEvent(float f)
    {
        OnEventRaised?.Invoke(f);
    }
}
