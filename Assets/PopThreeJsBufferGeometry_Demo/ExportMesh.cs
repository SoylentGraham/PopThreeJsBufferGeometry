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


	public void SaveMeshFile()
	{
		PopThreeJsBufferGeometry.SaveMeshFile( MeshSource.mesh );
	}

	

}
