using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

public class DeferredDecalSystem {

	public static DeferredDecalSystem m_Instance;
	public static DeferredDecalSystem instance {
		get {
			if (m_Instance == null)
				m_Instance = new DeferredDecalSystem();
			return m_Instance;
		}
	}

	internal HashSet<Decal> m_Decals = new HashSet<Decal>();

	public void AddDecal(Decal d) {
		RemoveDecal(d);
		m_Decals.Add(d);
	}

	public void RemoveDecal(Decal d) {
		m_Decals.Remove(d);
	}
}

[ExecuteInEditMode]
public class DeferredDecalRenderer : MonoBehaviour {
	
	private Dictionary<Camera, CommandBuffer> m_Cameras = new Dictionary<Camera, CommandBuffer>();
	private Mesh m_CubeMesh;

	public void Awake() {
		// Get cube primitive used for rendering
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        m_CubeMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        DestroyImmediate(gameObject);
	}

	public void OnDisable() {
		foreach (var cam in m_Cameras) {
			if (cam.Key) {
				cam.Key.RemoveCommandBuffer (CameraEvent.BeforeLighting, cam.Value);
			}
		}
	}

	public void OnRenderObject() {
		var act = gameObject.activeInHierarchy && enabled;
		if (!act) {
			OnDisable();
			return;
		}

		var cam = Camera.current;
		if (!cam)
			return;

		CommandBuffer buf = null;
		if (m_Cameras.ContainsKey(cam)) {
			buf = m_Cameras[cam];
			buf.Clear ();
		} else {
			buf = new CommandBuffer();
			buf.name = "Deferred decals";
			m_Cameras[cam] = buf;

			cam.AddCommandBuffer (CameraEvent.BeforeLighting, buf);
		}
		
		RenderTargetIdentifier[] renderTargets = {
			BuiltinRenderTextureType.GBuffer0, // Albedo
			BuiltinRenderTextureType.GBuffer1, // Specular + Roughness
			BuiltinRenderTextureType.GBuffer2 // Normals
		};

		buf.SetRenderTarget (renderTargets, BuiltinRenderTextureType.CameraTarget);
		foreach (var decal in DeferredDecalSystem.instance.m_Decals) {
			buf.DrawMesh (m_CubeMesh, decal.transform.localToWorldMatrix, decal.m_Material);
		}
	}
}