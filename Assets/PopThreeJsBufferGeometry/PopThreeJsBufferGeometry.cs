using UnityEngine;
using System.Collections;


public class BufferGeometry
{
	public string		Name;

	public BufferGeometry(Mesh mesh)
	{ 
		Name = mesh.name;
	}
};


public class PopThreeJsBufferGeometry
{
	static public string GetMeshJsonString(Mesh mesh)
	{
		var Geo = new BufferGeometry( mesh );
		return JsonUtility.ToJson( Geo, true );
	}
	
}
