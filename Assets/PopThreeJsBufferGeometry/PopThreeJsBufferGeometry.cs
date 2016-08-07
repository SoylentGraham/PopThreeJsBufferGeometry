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
public class BufferGeometry4_Attribute_Float
{
	public BufferGeometry4_Attribute_Float(int VectorSize)
	{ 
		itemSize = VectorSize;
	}

	public float[]	array;
	public string	type = "Float32Array";
	public bool		normalized = false;
	public int		itemSize = 3;
};


[Serializable]
public class BufferGeometry4_Attribute_Int
{
	public BufferGeometry4_Attribute_Int(int VectorSize=1)
	{ 
		itemSize = VectorSize;
	}

	public int[]	array;
	public string	type = "Int32Array";
	public bool		normalized = false;
	public int		itemSize = 1;
};


[Serializable]
public class BufferGeometry4_Attributes
{
	public BufferGeometry4_Attribute_Float	position;	//	3x
	public BufferGeometry4_Attribute_Float	normal;		//	3x
	public BufferGeometry4_Attribute_Float	uv;			//	2x
	public BufferGeometry4_Attribute_Float	uv2;		//	2x
	public BufferGeometry4_Attribute_Float	color;		//	3x
};


[Serializable]
public class BufferGeometry4_Data
{
	public BufferGeometry4_Attribute_Int	indicies;
	public BufferGeometry4_Attributes		attributes;
};


/*
var TYPED_ARRAYS = {
			'Int8Array': Int8Array,
			'Uint8Array': Uint8Array,
			'Uint8ClampedArray': Uint8ClampedArray,
			'Int16Array': Int16Array,
			'Uint16Array': Uint16Array,
			'Int32Array': Int32Array,
			'Uint32Array': Uint32Array,
			'Float32Array': Float32Array,
			'Float64Array': Float64Array
		};
*/


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

	void WriteAttribute(ref BufferGeometry4_Attribute_Float Attribute, Vector3[] VectorArray)
	{ 
		if ( VectorArray == null )
		{
			Attribute = null;
			return;
		}

		int VectorSize = 3;
		//	todo: expand array for merging multiple meshes
		Attribute = new BufferGeometry4_Attribute_Float(VectorSize);
		float[] FloatArray = new float[VectorArray.Length * VectorSize];
		for ( int i=0;	i<VectorArray.Length;	i++ )
		{ 
			var Pos3 = VectorArray[i];
			FloatArray[(i*VectorSize)+0] = Pos3.x;
			FloatArray[(i*VectorSize)+1] = Pos3.y;
			FloatArray[(i*VectorSize)+2] = Pos3.z;
		}
		Attribute.array = FloatArray;
	}

	void WriteAttribute(ref BufferGeometry4_Attribute_Float Attribute, Vector2[] VectorArray)
	{ 
		if ( VectorArray == null )
		{
			Attribute = null;
			return;
		}

		int VectorSize = 2;
		//	todo: expand array for merging multiple meshes
		Attribute = new BufferGeometry4_Attribute_Float(VectorSize);
		float[] FloatArray = new float[VectorArray.Length * VectorSize];
		for ( int i=0;	i<VectorArray.Length;	i++ )
		{ 
			var Pos3 = VectorArray[i];
			FloatArray[(i*VectorSize)+0] = Pos3.x;
			FloatArray[(i*VectorSize)+1] = Pos3.y;
		}
		Attribute.array = FloatArray;
	}


	//	for this json it's RGB, not RGBA
	void WriteAttribute(ref BufferGeometry4_Attribute_Float Attribute, Color[] VectorArray)
	{ 
		if ( VectorArray == null )
		{
			Attribute = null;
			return;
		}

		int VectorSize = 3;
		//	todo: expand array for merging multiple meshes
		Attribute = new BufferGeometry4_Attribute_Float(VectorSize);
		float[] FloatArray = new float[VectorArray.Length * VectorSize];
		for ( int i=0;	i<VectorArray.Length;	i++ )
		{ 
			var Pos3 = VectorArray[i];
			FloatArray[(i*VectorSize)+0] = Pos3.r;
			FloatArray[(i*VectorSize)+1] = Pos3.g;
			FloatArray[(i*VectorSize)+2] = Pos3.b;
		}
		Attribute.array = FloatArray;
	}


	public BufferGeometry4(Mesh mesh,int Version=0)
	{ 
		metadata = new BufferGeometry4_Meta();
		metadata.version = Version;
		data = new BufferGeometry4_Data();

		data.attributes = new BufferGeometry4_Attributes();
	
		//	serialise data
		WriteAttribute( ref data.attributes.position, mesh.vertices );
		WriteAttribute( ref data.attributes.normal, mesh.normals );
		WriteAttribute( ref data.attributes.uv, mesh.uv );
		WriteAttribute( ref data.attributes.uv2, mesh.uv2 );
		WriteAttribute( ref data.attributes.color, mesh.colors );

		//	write triangle indexes - currently just a direct copy
		data.indicies = new BufferGeometry4_Attribute_Int();
		data.indicies.array = mesh.triangles;
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
