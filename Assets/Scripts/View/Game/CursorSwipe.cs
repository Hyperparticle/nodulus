using System;
using Core.Data;
using UnityEngine;
using View.Items;

namespace View.Game
{
	public class CursorSwipe : MonoBehaviour
	{
		// TODO: make configurable
		private const float Time = 1.8f;
		private const float Delay = 0f;
		private const float MoveDistance = 1.2f;

		private SpriteRenderer _spriteRenderer;
		private Color _initColor;
		private Vector3 _initPos;
			
		private int _colorTween;
		private int _moveTween;

		private void Awake()
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();

			_initPos = transform.localPosition;
			_initColor = _spriteRenderer.color;
			_spriteRenderer.color = Colorizer.Alpha(_spriteRenderer.color, 0f);
		}

		public void Show(Vector3 pos, Direction dir)
		{
			Hide(() => {
				transform.localPosition = _initPos + pos;
				var move = transform.localPosition + dir.Vector() * MoveDistance;

				_moveTween = LeanTween.moveLocal(gameObject, move, Time)
					.setDelay(Delay)
					.setEase(LeanTweenType.easeInOutSine)
					.setLoopCount(-1)
					.setOnCompleteOnStart(true)
					.setOnCompleteOnRepeat(true)
					.setOnComplete(() => {
						LeanTween.cancel(_colorTween);
						_colorTween = LeanTween.value(0f, _initColor.a, Time / 2f)
							.setDelay(Delay)
							.setEase(LeanTweenType.easeInOutSine)
							.setLoopPingPong(1)
							.setOnUpdate(a => _spriteRenderer.color = Colorizer.Alpha(_spriteRenderer.color, a))
							.id;
					})
					.id;	
			});
		}

		public void Hide(Action onComplete = null)
		{
			LeanTween.cancel(_moveTween);
			LeanTween.cancel(_colorTween);
			
			_colorTween = LeanTween.value(_spriteRenderer.color.a, 0f, Time / 8f)
				.setEase(LeanTweenType.easeInOutSine)
				.setOnUpdate(a => _spriteRenderer.color = Colorizer.Alpha(_spriteRenderer.color, a))
				.setOnComplete(onComplete)
				.id;
		}
	}
}
