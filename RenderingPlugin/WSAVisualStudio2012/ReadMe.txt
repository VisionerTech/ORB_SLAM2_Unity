This is a RenderingPlugin for Windows Store Apps. 
There's already prebuilt plugin version in Unity project. (Assets\Plugins\Metro\x86, Assets\Plugins\Metro\ARM)
Also - precompiled shaders are located here - UnityProject\Assets\StreamingAssets.
Also checkout - UnityProject\Assets\Editor\MyBuildPostprocessor.cs, it contains some bits, how Windows Store Apps source files are modified after building from Editor.

If you want to recompile RenderingPlugin or shaders it's simply enough to hit Build Solution, and both RenderingPlugin.dll and precompiled shaders will be copied to appropriate folders in Unity project.
For more information, go to Project->Properties->BuildEvents->Post-Build event.


It's also possible to setup RenderingPlugin project for debugging, do the following.
* Open UnityProject
* Build Windows Store Apps, it doesn't matter which project type you choose, but in this case choose XAML C#
* If you're using 4.3 or higher, choose SDK 8.0.
* Open generated solution
* Right click on solution in Solution Explorer
* Add->Existing Project, add RenderingPlugin\WSAVisualStudio2012\RenderingPlugin.vcxproj
* Right click on "Plugin Render API Example", Project Dependencies, select RenderingPlugin
* That's it, for ex., place a breakpoint in CreateD3D11Resources

* You can now change shaders and sources in RenderingPlugin project, and test it on the fly.
* Note: you can also debug RenderingPlugin source files

Known issues
* It should be also possible to debug shaders with integrated Graphics Debugger, but it seems Visual Studio 2012 requires Vertex Shader.pdb or Pixel Shader.pdb, but don't generate them.



