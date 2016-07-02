using System;
using UnityEngine;
using System.Collections;
using Assets.Scripts.View.Game;

public class ButtonScript : MonoBehaviour
{
    public float AnimationTime = 0.25f;
    public ButtonState ButtonState;

    public event Action<ButtonState> ButtonPressed;

    void OnMouseDown()
    {
        if (LeanTween.isTweening(gameObject)) {
            return;
        }

        if (ButtonPressed != null) {
            ButtonPressed(ButtonState);
        }

        Move();
    }

    private void Move()
    {
        var pos = transform.localPosition;

        LeanTween.moveLocalZ(gameObject, pos.z + 0.5f, AnimationTime)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() => MoveBack(pos));
    }

    private void MoveBack(Vector3 pos)
    {
        LeanTween.moveLocalZ(gameObject, pos.z, AnimationTime)
            .setEase(LeanTweenType.easeInOutSine);
    }
}

public enum ButtonState
{
    Left,
    Select,
    Right
}
