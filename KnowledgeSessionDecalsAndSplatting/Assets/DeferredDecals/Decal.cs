using UnityEngine;

[ExecuteInEditMode]
public class Decal : MonoBehaviour {

	private static Color gizmoColour = new Color(0.0f, 0.7f, 1f, 0.0f);
	private static Color gizmoColourWire = new Color(0.0f, 0.7f, 1f, 0.2f);
	private static Color gizmoColourSelected = new Color(0.0f, 0.7f, 1f, 0.3f);
	private static Color gizmoColourWireSelected = new Color(0.0f, 0.7f, 1f, 0.5f);

	[Range(0.0f, 1.0f)]
	public float strength = 1.0f;

	public Material m_Material;

	public void OnEnable() {
		DeferredDecalSystem.instance.AddDecal(this);
	}

	public void Start() {
		DeferredDecalSystem.instance.AddDecal(this);
	}

	public void OnDisable() {
		DeferredDecalSystem.instance.RemoveDecal(this);
	}

	private void DrawGizmo(bool selected) {
		Gizmos.color = selected ? gizmoColourSelected : gizmoColour;
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawCube (Vector3.zero, Vector3.one);
		Gizmos.color = selected ? gizmoColourWireSelected : gizmoColourWire;
		Gizmos.DrawWireCube (Vector3.zero, Vector3.one);		
	}

	public void OnDrawGizmos() {
		DrawGizmo(false);
	}

	public void OnDrawGizmosSelected() {
		DrawGizmo(true);
	}
}
