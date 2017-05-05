
## ORB_SLAM2 Unity

[![IMAGE ALT TEXT HERE](http://img.youtube.com/vi/0sJTb0Xm9ss/0.jpg)](https://www.youtube.com/watch?v=0sJTb0Xm9ss)

This is a sample project for using [ORB_SLAM2 stereo](https://github.com/raulmur/ORB_SLAM2) for inside-out tracking of AR/VR scenario with VisionerTech VMG-PROV setup. We compile ORB_SLAM2 in windows and use it as a rendering plugin for Unity. A loose coupling IMU approach for camera rotation running only in Unity side, on the other hand, camera position is provided by ORB_SLAM2. "/RenderingPlugin/" holds a visual studio 2013 project building ORB_SLAM2, camera capturing, IMU data, rendeing setup and other things to a .dll for Unity. "/UnityProject/" holds a Unity project.

## Hardware Requirement:

1.  Recommended specs: Intel Core i5-4460/8G RAM/GTX 660/at least two USB3.0/
2.  Windows x64 version.(tested on win7/win10)

## How to Run Unity project
1.  If no Visual Studio 2013 is installed, install [X64 version of Visual C++ Redistributable Packages for Visual Studio 2013](https://www.microsoft.com/en-us/download/details.aspx?id=40784).
2.  Replace the calibration files in "/save_param/" to your VMG-PROV setup.
3.  Open the project with Unity Editor X64 version(tested with Unity 5.4.0f3 (64-bit)).
4.  Open the "scene" scene.
5.  "StereoCamera" Object holds a pair of Camera and real world image background. "WorldPlaneIMU" object holds a virtual world surface plane and a cylinder. If you are adding your own objects, please do add them under WorldPlaneIMU.
6.  Run it and you will see stereo image pairs.
7.  Click "Button" to start SLAM system for tracking the camera pose.
8.  If you want to build a running .exe, please do build x64 version.

## How to Compile Rendering Plugin
1.  "/RenderingPlugin/VisualStudio2013/RenderingPlugin.sln" holds the Visual Studio 2013 project compiling the dll for Unity. Please do use Visual Studio 2013 to compile, dependencies like OSB_SLAM2 is compiled in a Visual Studio 2013 environment(it takes some time to port from Linux), it's no guarantee of success with other compilers.
2.  Install OpenCV 3(tested with OpenCV 3.1.0) and config RenderingPlugin project to your environment.
3.  "/ORB_SLAM2/" holds prebuild libs of ORB_SLAM2.
4.  Build the Release X64 version. The RenderingPlugin.dll is copied to "/UnityProject/Assets/Plugins/x86_64/".
