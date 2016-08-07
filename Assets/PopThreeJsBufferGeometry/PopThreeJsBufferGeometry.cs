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
	public float[]	position;	//	3x
	public float[]	normal;		//	3x
	public float[]	uv;			//	2x
	public float[]	uv2;		//	2x
	public float[]	color;		//	3x
};

[Serializable]
public class BufferGeometry4_Data
{
	public int[]						index;
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

	void WriteFloatArray(ref float[] FloatArray, Vector3[] VectorArray)
	{ 
		if ( VectorArray == null )
		{
			FloatArray = null;
			return;
		}

		int VectorSize = 3;
		//	todo: expand array for merging multiple meshes
		FloatArray = new float[VectorArray.Length * VectorSize];
		for ( int i=0;	i<VectorArray.Length;	i++ )
		{ 
			var Pos3 = VectorArray[i];
			FloatArray[(i*VectorSize)+0] = Pos3.x;
			FloatArray[(i*VectorSize)+1] = Pos3.y;
			FloatArray[(i*VectorSize)+2] = Pos3.z;
		}
	}

	void WriteFloatArray(ref float[] FloatArray, Vector2[] VectorArray)
	{ 
		if ( VectorArray == null )
		{
			FloatArray = null;
			return;
		}
	
		int VectorSize = 2;
		//	todo: expand array for merging multiple meshes
		FloatArray = new float[VectorArray.Length * VectorSize];
		for ( int i=0;	i<VectorArray.Length;	i++ )
		{ 
			var Pos2 = VectorArray[i];
			FloatArray[(i*VectorSize)+0] = Pos2.x;
			FloatArray[(i*VectorSize)+1] = Pos2.y;
		}
	}

	//	for this json it's RGB, not RGBA
	void WriteFloatArray(ref float[] FloatArray, Color[] VectorArray)
	{ 
		if ( VectorArray == null )
		{
			FloatArray = null;
			return;
		}
	
		int VectorSize = 3;
		//	todo: expand array for merging multiple meshes
		FloatArray = new float[VectorArray.Length * VectorSize];
		for ( int i=0;	i<VectorArray.Length;	i++ )
		{ 
			var Pos2 = VectorArray[i];
			FloatArray[(i*VectorSize)+0] = Pos2.r;
			FloatArray[(i*VectorSize)+1] = Pos2.g;
			FloatArray[(i*VectorSize)+2] = Pos2.b;
		}
	}

	public BufferGeometry4(Mesh mesh,int Version=0)
	{ 
		metadata = new BufferGeometry4_Meta();
		metadata.version = Version;
		data = new BufferGeometry4_Data();
		data.attributes = new BufferGeometry4_Attributes();

		//	serialise data
		WriteFloatArray( ref data.attributes.position, mesh.vertices );
		WriteFloatArray( ref data.attributes.normal, mesh.normals );
		WriteFloatArray( ref data.attributes.uv, mesh.uv );
		WriteFloatArray( ref data.attributes.uv2, mesh.uv2 );
		WriteFloatArray( ref data.attributes.color, mesh.colors );

		//	write triangle indexes - currently just a direct copy
		data.index = mesh.triangles;
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
