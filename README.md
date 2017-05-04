
## ORB_SLAM2 Unity

This is a sample project for using [ORB_SLAM2 stereo](https://github.com/raulmur/ORB_SLAM2) for inside-out tracking of AR/VR scenario with VisionerTech VMG-PROV setup. We compile ORB_SLAM2 in windows and use it as a rendering plugin for Unity. A loose coupling IMU approach for camera rotation running only in Unity side, on the other hand, camera position is provided by ORB_SLAM2. "/RenderingPlugin/" holds a visual studio 2013 project building ORB_SLAM2, camera capturing, IMU data, rendeing setup and other things to a .dll for Unity. "/UnityProject/" holds a Unity project.

## Hardware Requirement:

1.  recommended specs: Intel Core i5-4460/8G RAM/GTX 660/at least two USB3.0/
2.  windows x64 version.(tested on win7/win10)

## How to Run Unity project
1.  if no Visual Studio 2013 is installed, install [X64 version of Visual C++ Redistributable Packages for Visual Studio 2013](https://www.microsoft.com/en-us/download/details.aspx?id=40784).
2.  Open the project with Unity Editor X64 version(tested with Unity 5.4.0f3 (64-bit)).
3.  Open the "scene" scene.
4.  Run it and you will see stereo image pairs.
5.  Click "Button" to init SLAM system.

## How to Compile Rendering Plugin
