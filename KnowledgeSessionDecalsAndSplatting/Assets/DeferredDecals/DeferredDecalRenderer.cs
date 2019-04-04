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

	internal HashSet<DeferredDecal> m_Decals = new HashSet<DeferredDecal>();

	public void AddDecal(DeferredDecal d) {
		RemoveDecal(d);
		m_Decals.Add(d);
	}

	public void RemoveDecal(DeferredDecal d) {
		m_Decals.Remove(d);
	}
}

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class DeferredDecalRenderer : MonoBehaviour {
	
	private Mesh m_CubeMesh;

	private CommandBuffer buffer;
	private MaterialPropertyBlock properties;
	private int strengthId;
	private int gbufferNormalsId;

	private void OnUse() {
		// It already has been initialised
		if (buffer != null)
			return;

		// Get cube primitive used for rendering
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        m_CubeMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        DestroyImmediate(gameObject);

        buffer = new CommandBuffer();
        buffer.name = "Deferred decals";

        properties = new MaterialPropertyBlock();
		strengthId = Shader.PropertyToID("_Strength");
		gbufferNormalsId = Shader.PropertyToID("_GbufferNormals");
	}

	public void OnEnable() {
		OnUse();
	}

	public void OnDisable() {
		OnUse();
		buffer.Clear();
	}

	public void OnRenderObject() {
		OnUse();
		Camera.current.RemoveCommandBuffer(CameraEvent.BeforeLighting, buffer);
		Camera.current.AddCommandBuffer(CameraEvent.BeforeLighting, buffer);

		// Clear previous render
		buffer.Clear();

		// Blit the Gbuffer normals so we can read from them in the shader
		buffer.GetTemporaryRT(gbufferNormalsId, -1, -1);
		buffer.Blit(BuiltinRenderTextureType.GBuffer2, gbufferNormalsId);

		RenderTargetIdentifier[] renderTargets = {
			BuiltinRenderTextureType.GBuffer0, // Albedo
			BuiltinRenderTextureType.GBuffer1, // Specular + Roughness
			BuiltinRenderTextureType.GBuffer2 // Normals
		};

		buffer.SetRenderTarget (renderTargets, BuiltinRenderTextureType.CameraTarget);
		foreach (var decal in DeferredDecalSystem.instance.m_Decals) {
			if (decal.m_Material == null)
				continue;
			
			properties.SetFloat(strengthId, decal.strength);
			buffer.DrawMesh(m_CubeMesh, decal.transform.localToWorldMatrix, decal.m_Material, 0, -1, properties);
		}

		buffer.ReleaseTemporaryRT(gbufferNormalsId);
	}
}