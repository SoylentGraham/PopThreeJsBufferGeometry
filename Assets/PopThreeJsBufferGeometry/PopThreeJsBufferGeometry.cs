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
	public BufferGeometry4_Attribute_Int	index;
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

	delegate void	ModifyVector3(ref Vector3 v);
	delegate void	ModifyVector2(ref Vector2 v);
	delegate void	ModifyColor(ref Color v);

	void WriteAttribute(ref BufferGeometry4_Attribute_Float Attribute, Vector3[] VectorArray,ModifyVector3 Modify=null)
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
			if ( Modify != null )
				Modify( ref Pos3 );

			FloatArray[(i*VectorSize)+0] = Pos3.x;
			FloatArray[(i*VectorSize)+1] = Pos3.y;
			FloatArray[(i*VectorSize)+2] = Pos3.z;
		}
		Attribute.array = FloatArray;
	}

	void WriteAttribute(ref BufferGeometry4_Attribute_Float Attribute, Vector2[] VectorArray,ModifyVector2 Modify=null)
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
			if ( Modify != null )
				Modify( ref Pos3 );

			FloatArray[(i*VectorSize)+0] = Pos3.x;
			FloatArray[(i*VectorSize)+1] = Pos3.y;
		}
		Attribute.array = FloatArray;
	}


	//	for this json it's RGB, not RGBA
	void WriteAttribute(ref BufferGeometry4_Attribute_Float Attribute, Color[] VectorArray,ModifyColor Modify=null)
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
			if ( Modify != null )
				Modify( ref Pos3 );

			FloatArray[(i*VectorSize)+0] = Pos3.r;
			FloatArray[(i*VectorSize)+1] = Pos3.g;
			FloatArray[(i*VectorSize)+2] = Pos3.b;
		}
		Attribute.array = FloatArray;
	}

	void PadNullAttribute(ref BufferGeometry4_Attribute_Float Attribute)
	{
		if ( Attribute != null )
			return;
		Attribute = new BufferGeometry4_Attribute_Float(1);
	}


	public BufferGeometry4(Mesh mesh,Matrix4x4 Transform,int Version=0)
	{ 
		metadata = new BufferGeometry4_Meta();
		metadata.version = Version;
		data = new BufferGeometry4_Data();

		data.attributes = new BufferGeometry4_Attributes();

		ModifyVector3 ReorientatePosition = (ref Vector3 p) =>
		{
			p = Transform.MultiplyPoint(p);

			//	+y is up in threejs (by default)
			//p.y = -p.y;
			//	xz seems to be wrong too. Experimenting suggests just z needs correcting
			//p.x = -p.x;
			p.z = -p.z;
		};	

		//	serialise data
		//	note: we write them all
		WriteAttribute( ref data.attributes.position, mesh.vertices, ReorientatePosition );
		WriteAttribute( ref data.attributes.normal, mesh.normals );
		WriteAttribute( ref data.attributes.uv, mesh.uv );
		WriteAttribute( ref data.attributes.uv2, mesh.uv2 );
		WriteAttribute( ref data.attributes.color, mesh.colors );

		//	gr: if an attrib is null, unity still serialises an "empty" class. if its empty.. three js will throw an exception... so make empty attribs
		PadNullAttribute( ref data.attributes.position );
		PadNullAttribute( ref data.attributes.normal );
		PadNullAttribute( ref data.attributes.uv );
		PadNullAttribute( ref data.attributes.uv2 );
		PadNullAttribute( ref data.attributes.color );

		//	write triangle indexes - currently just a direct copy
		data.index = new BufferGeometry4_Attribute_Int();
		data.index.array = mesh.triangles;
	}
};


public class PopThreeJsBufferGeometry
{
	static public string GetMeshJsonString(Mesh mesh,Matrix4x4 Transform)
	{
		var Geo = new BufferGeometry4( mesh, Transform );
		return JsonUtility.ToJson( Geo, true );
	}

#if UNITY_EDITOR
	static public void SaveMeshFile(Mesh mesh,Matrix4x4 Transform)
	{
		string path = UnityEditor.EditorUtility.SaveFilePanel( "Save mesh json", "", mesh.name + ".json", "json" );
		if( path.Length != 0 )
		{
			var Json = PopThreeJsBufferGeometry.GetMeshJsonString( mesh, Transform );
			var FileHandle = File.CreateText( path );
			FileHandle.Write(Json);
			FileHandle.Close();
			PopBrowseToFile.ShowFile( path );
		}
	}
#endif

#if UNITY_EDITOR
	[MenuItem("CONTEXT/MeshFilter/Export Mesh as .json...")]
	public static void SaveMeshInPlace (MenuCommand menuCommand) {
		MeshFilter mf = menuCommand.context as MeshFilter;
		Mesh mesh = mf.sharedMesh;
		SaveMeshFile( mesh, Matrix4x4.identity );
	}
#endif


#if UNITY_EDITOR
	[MenuItem("CONTEXT/MeshFilter/Export Mesh as .json baked in world space...")]
	public static void SaveMeshInPlaceBaked (MenuCommand menuCommand) {
		MeshFilter mf = menuCommand.context as MeshFilter;
		Mesh mesh = mf.sharedMesh;

		var Mtx = mf.transform.localToWorldMatrix;

		SaveMeshFile( mesh, Mtx );
	}
	#endif
}
