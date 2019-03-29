using UnityEngine;

public class hud : MonoBehaviour
{
	public Camera Camera;
	public Transform ObjectToTrace;

	public RectTransform CrosshairParentRect;
	public RectTransform Crosshair;

	private void LateUpdate()
	{
		Vector2 pos = Camera.WorldToViewportPoint(ObjectToTrace.position);
		pos.x = CrosshairParentRect.sizeDelta.x * pos.x;
		pos.y = CrosshairParentRect.sizeDelta.y * pos.y;
		Crosshair.anchoredPosition = pos;
	}
}