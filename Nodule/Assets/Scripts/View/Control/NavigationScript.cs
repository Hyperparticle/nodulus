using UnityEngine;
using System.Collections;

public class NavigationScript : MonoBehaviour
{
    void Start()
    {
        var pos = transform.localPosition;
        transform.Translate(20 * Vector3.left);

        LeanTween.moveLocal(gameObject, pos, 1f)
            .setEase(LeanTweenType.easeOutSine);
    }
}
