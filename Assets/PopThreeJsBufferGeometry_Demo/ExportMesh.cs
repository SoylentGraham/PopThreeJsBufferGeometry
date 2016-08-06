using UnityEngine;
using System.Collections;

using System;
using System.IO;
using System.Runtime.InteropServices;

public class ExportMesh : MonoBehaviour {

	public MeshFilter		MeshSource;

	void Start()
	{
		if ( MeshSource == null )
			MeshSource = GetComponent<MeshFilter>();
	}



	public void SaveMeshFile()
	{
		string path = UnityEditor.EditorUtility.SaveFilePanel( "Save mesh json", "", MeshSource.mesh.name + ".json", "json" );
		if( path.Length != 0 )
		{
			var Json = PopThreeJsBufferGeometry.GetMeshJsonString( MeshSource.mesh );
			var FileHandle = File.CreateText( path );
			FileHandle.Write(Json);
			FileHandle.Close();
			SelectInFileExplorer( path );
		}
	}

	//	from http://stackoverflow.com/a/12262552
	private void SelectInFileExplorer(string fullPath)
    {
        if (string.IsNullOrEmpty(fullPath))
            throw new ArgumentNullException("fullPath");

        fullPath = Path.GetFullPath(fullPath);

        IntPtr pidlList = NativeMethods.ILCreateFromPathW(fullPath);
        if (pidlList != IntPtr.Zero)
            try
            {
                // Open parent folder and select item
                Marshal.ThrowExceptionForHR(NativeMethods.SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0));
            }
            finally
            {
                NativeMethods.ILFree(pidlList);
            }
    }

    static class NativeMethods
    {

        [DllImport("shell32.dll", ExactSpelling=true)]
        public static extern void ILFree(IntPtr pidlList);

        [DllImport("shell32.dll", CharSet=CharSet.Unicode, ExactSpelling=true)]
        public static extern IntPtr ILCreateFromPathW(string pszPath);

        [DllImport("shell32.dll", ExactSpelling=true)]
        public static extern int SHOpenFolderAndSelectItems(IntPtr pidlList, uint cild, IntPtr children, uint dwFlags);
    }

}
