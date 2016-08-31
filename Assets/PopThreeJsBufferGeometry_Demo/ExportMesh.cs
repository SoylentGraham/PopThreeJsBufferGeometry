using UnityEngine;
using System.Collections;

using System;


public class ExportMesh : MonoBehaviour {

	public MeshFilter		MeshSource;

	void Start()
	{
		if ( MeshSource == null )
			MeshSource = GetComponent<MeshFilter>();
	}

	#if UNITY_EDITOR
	public void SaveMeshFile()
	{
		PopThreeJsBufferGeometry.SaveMeshFile( MeshSource.mesh, Matrix4x4.identity );
	}
	#endif

	

}
