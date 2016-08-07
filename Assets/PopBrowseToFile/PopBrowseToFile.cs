using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.InteropServices;



public class PopBrowseToFile : MonoBehaviour {
 
	static class Win32
    {

        [DllImport("shell32.dll", ExactSpelling=true)]
        public static extern void ILFree(IntPtr pidlList);

        [DllImport("shell32.dll", CharSet=CharSet.Unicode, ExactSpelling=true)]
        public static extern IntPtr ILCreateFromPathW(string pszPath);

        [DllImport("shell32.dll", ExactSpelling=true)]
        public static extern int SHOpenFolderAndSelectItems(IntPtr pidlList, uint cild, IntPtr children, uint dwFlags);
    }	


	//	from http://stackoverflow.com/a/12262552
	public static void ShowFile(string fullPath)
    {
        fullPath = Path.GetFullPath(fullPath);

        IntPtr pidlList = Win32.ILCreateFromPathW(fullPath);
        if (pidlList != IntPtr.Zero)
		{
            try
            {
                // Open parent folder and select item
                Marshal.ThrowExceptionForHR(Win32.SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0));
            }
            finally
            {
                Win32.ILFree(pidlList);
            }
		}
    }

 

}
