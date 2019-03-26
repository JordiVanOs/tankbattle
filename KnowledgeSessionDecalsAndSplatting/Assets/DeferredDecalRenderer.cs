using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

// See _ReadMe.txt

public class DeferredDecalSystem
{
	static DeferredDecalSystem m_Instance;
	static public DeferredDecalSystem instance {
		get {
			if (m_Instance == null)
				m_Instance = new DeferredDecalSystem();
			return m_Instance;
		}
	}

	internal HashSet<Decal> m_Decals = new HashSet<Decal>();

	public void AddDecal (Decal d)
	{
		RemoveDecal (d);
		m_Decals.Add (d);
	}
	public void RemoveDecal (Decal d)
	{
		m_Decals.Remove (d);
	}
}

[ExecuteInEditMode]
public class DeferredDecalRenderer : MonoBehaviour
{
	public Mesh m_CubeMesh;
	private Dictionary<Camera,CommandBuffer> m_Cameras = new Dictionary<Camera,CommandBuffer>();

	public void OnDisable()
	{
		foreach (var cam in m_Cameras)
		{
			if (cam.Key)
			{
				cam.Key.RemoveCommandBuffer (CameraEvent.BeforeLighting, cam.Value);
			}
		}
	}

	public void OnWillRenderObject()
	{
		var act = gameObject.activeInHierarchy && enabled;
		if (!act)
		{
			OnDisable();
			return;
		}

		var cam = Camera.current;
		if (!cam)
			return;

		CommandBuffer buf = null;
		if (m_Cameras.ContainsKey(cam))
		{
			buf = m_Cameras[cam];
			buf.Clear ();
		}
		else
		{
			buf = new CommandBuffer();
			buf.name = "Deferred decals";
			m_Cameras[cam] = buf;

			// set this command buffer to be executed just before deferred lighting pass
			// in the camera
			cam.AddCommandBuffer (CameraEvent.BeforeLighting, buf);
		}

		//@TODO: in a real system should cull decals, and possibly only
		// recreate the command buffer when something has changed.

		var system = DeferredDecalSystem.instance;

		// copy g-buffer normals into a temporary RT
		var normalsID = Shader.PropertyToID("_NormalsCopy");
		buf.GetTemporaryRT (normalsID, -1, -1);
		buf.Blit (BuiltinRenderTextureType.GBuffer2, normalsID);
		
		RenderTargetIdentifier[] mrt = {BuiltinRenderTextureType.GBuffer0, BuiltinRenderTextureType.GBuffer2};
		buf.SetRenderTarget (mrt, BuiltinRenderTextureType.CameraTarget);
		foreach (var decal in system.m_Decals) {
			buf.DrawMesh (m_CubeMesh, decal.transform.localToWorldMatrix, decal.m_Material);
		}

		// release temporary normals RT
		buf.ReleaseTemporaryRT (normalsID);
	}
}
