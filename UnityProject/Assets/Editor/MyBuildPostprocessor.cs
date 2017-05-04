using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

public class MyBuildPostprocessor
{
	[PostProcessBuild]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
	{
		if(target == BuildTarget.iOS)
			OnPostprocessBuildIOS(pathToBuiltProject);
	}

	private static void OnPostprocessBuildIOS(string pathToBuiltProject)
	{
		File.Copy("../RenderingPlugin/UnityPluginInterface.h", Path.Combine(pathToBuiltProject, "Libraries/UnityPluginInterface.h"), true);
		File.Copy("../RenderingPlugin/RenderingPluginGLES.cpp", Path.Combine(pathToBuiltProject, "Libraries/RenderingPluginGLES.cpp"), true);

		// TODO: for now project api is not exposed, so you need to add files manually
	}
}
