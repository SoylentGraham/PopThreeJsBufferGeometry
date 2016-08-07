using UnityEngine;
using System.Collections;
using System;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class BufferGeometry4_Meta
{
	public int		version = 0;
	public string	type = "BufferGeometry";
	public string	generator = "PopThreeJsBufferGeometry";
};


[Serializable]
public class BufferGeometry4_Attributes
{
	public float[]	position;
	public float[]	normal;
	public float[]	uv;
	public float[]	uv2;
	public float[]	color;
 
};

[Serializable]
public class BufferGeometry4_Data
{
	public float[]						index;
	public BufferGeometry4_Attributes	attributes;
};


//	https://github.com/mrdoob/three.js/blob/d8b24a1d2d4887a5db77d7112e5ba1539377c905/src/core/Geometry.js#L224   
//	https://github.com/mrdoob/three.js/wiki/JSON-Geometry-format-4
/*
 {
    "metadata": {
        "version": 4,
        "type": "BufferGeometry",
        "generator": "BufferGeometryExporter"
    },
    "data": {
        "attributes": {
            "position": {
                "itemSize": 3,
                "type": "Float32Array",
                "array": [50,50,50,...]
            },
            "normal": {
                "itemSize": 3,
                "type": "Float32Array",
                "array": [1,0,0,...]
            },
            "uv": {
                "itemSize": 2,
                "type": "Float32Array",
                "array": [0,1,...]
            }
        },
        "boundingSphere": {
            "center": [0,0,0],
            "radius": 86.60254037844386
        }
    }
}
*/
public class BufferGeometry4
{
	public BufferGeometry4_Meta		metadata;
	public BufferGeometry4_Data		data;

	public BufferGeometry4(Mesh mesh,int Version=0)
	{ 
		metadata = new BufferGeometry4_Meta();
		metadata.version = Version;
		data = new BufferGeometry4_Data();
	}
};


public class PopThreeJsBufferGeometry
{
	static public string GetMeshJsonString(Mesh mesh)
	{
		var Geo = new BufferGeometry4( mesh );
		return JsonUtility.ToJson( Geo, true );
	}
	
	static public void SaveMeshFile(Mesh mesh)
	{
		string path = UnityEditor.EditorUtility.SaveFilePanel( "Save mesh json", "", mesh.name + ".json", "json" );
		if( path.Length != 0 )
		{
			var Json = PopThreeJsBufferGeometry.GetMeshJsonString( mesh );
			var FileHandle = File.CreateText( path );
			FileHandle.Write(Json);
			FileHandle.Close();
			PopBrowseToFile.ShowFile( path );
		}
	}

#if UNITY_EDITOR
	[MenuItem("CONTEXT/MeshFilter/Export Mesh as .json...")]
	public static void SaveMeshInPlace (MenuCommand menuCommand) {
		MeshFilter mf = menuCommand.context as MeshFilter;
		Mesh mesh = mf.sharedMesh;
		SaveMeshFile( mesh );
	}
#endif
}
