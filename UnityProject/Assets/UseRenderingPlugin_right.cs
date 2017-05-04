// on OpenGL ES there is no way to query texture extents from native texture id
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
#define UNITY_GLES_RENDERER
#endif


using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class UseRenderingPlugin_right : MonoBehaviour {

	#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport ("__Internal")]
	#else
	[DllImport ("RenderingPlugin")]
	#endif
	private static extern void SetTimeFromUnity(float t);

	// We'll also pass native pointer to a texture in Unity.
	// The plugin will fill texture data from native code.
	#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport ("__Internal")]
	#else
	[DllImport ("RenderingPlugin")]
	#endif
	#if UNITY_GLES_RENDERER
	private static extern void SetTextureFromUnity(System.IntPtr texture, int w, int h);
	#else
	private static extern void SetTextureFromUnity_right(System.IntPtr texture);
	#endif

	#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport ("__Internal")]
	#else
	[DllImport("RenderingPlugin")]
	#endif
	private static extern void SetUnityStreamingAssetsPath([MarshalAs(UnmanagedType.LPStr)] string path);

	#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport ("__Internal")]
	#else
	[DllImport("RenderingPlugin")]
	#endif
	private static extern IntPtr GetRenderEventFunc();

	//native function for open web cam.
	#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport ("__Internal")]
	#else
	[DllImport ("RenderingPlugin", EntryPoint="get_map_x2")]
	#endif
	private static extern IntPtr get_map_x2();
	
	//native function for open web cam.
	#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport ("__Internal")]
	#else
	[DllImport ("RenderingPlugin", EntryPoint="get_map_y2")]
	#endif
	private static extern IntPtr get_map_y2();

	private int width = 1080;
	private int height = 1080;
	
	private Texture2D texture;
	private float[] map_x2;
	private float[] map_y2;

	public UseRenderingPlugin render_sc_left;


	// Use this for initialization
	IEnumerator Start () {

		Debug.Log("left init "+render_sc_left.init_success); 

		if (render_sc_left.init_success) {
			texture = new Texture2D (width, height, TextureFormat.RGFloat, false);
			Material mat = GetComponent<Renderer> ().material;
			mat.SetTexture ("_Texture2", texture);

			map_x2 = new float[width * height];
			map_y2 = new float[width * height];
			//		
			IntPtr ptr = get_map_x2 ();
			Marshal.Copy (ptr, map_x2, 0, width * height);
		
			ptr = get_map_y2 ();
			Marshal.Copy (ptr, map_y2, 0, width * height);

//			Debug.Log ("map x2 0: " + map_x2 [1000]);
//			Debug.Log ("map x2 1: " + map_x2 [1001]);
//			Debug.Log ("map x2 2: " + map_x2 [2]);
//			Debug.Log ("map x2 3: " + map_x2 [3]);
//			Debug.Log ("map y2 0: " + map_y2 [1000]);
//			Debug.Log ("map y2 1: " + map_y2 [1001]);
//			Debug.Log ("map y2 2: " + map_y2 [2]);
//			Debug.Log ("map y2 3: " + map_y2 [3]);

			for (int i = 0; i < width; ++i) {
				for (int j = 0; j < height; ++j) {
					Color color;
					color.r = map_x2 [j * width + i] / (float)width;
					color.g = map_y2 [j * width + i] / (float)height;
					//                    color.r = ((float)i - 0.00f) / (float)width;
					//                    color.g = ((float)j - 0.0f) / (float)height;
					color.b = 0.0f;
					color.a = 1.0f;
					texture.SetPixel (i, j, color);
				}
			}

			texture.Apply ();
		}

		SetUnityStreamingAssetsPath(Application.streamingAssetsPath);

		CreateTextureAndPassToPlugin();
		yield return StartCoroutine("CallPluginAtEndOfFrames");
	}


	private void CreateTextureAndPassToPlugin()
	{
		// Create a texture
		Texture2D tex = new Texture2D(width,height,TextureFormat.ARGB32,false);
		tex.wrapMode = TextureWrapMode.Clamp;
		// Set point filtering just so we can see the pixels clearly
		tex.filterMode = FilterMode.Bilinear;
		// Call Apply() so it's actually uploaded to the GPU
		tex.Apply();
		
		// Set texture onto our matrial
		GetComponent<Renderer>().material.mainTexture = tex;
		
		// Pass texture pointer to the plugin
		#if UNITY_GLES_RENDERER
		SetTextureFromUnity (tex.GetNativeTexturePtr(), tex.width, tex.height);
		#else
		SetTextureFromUnity_right (tex.GetNativeTexturePtr());
		#endif
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private IEnumerator CallPluginAtEndOfFrames()
	{
		while (true) {
			float time1 = Time.realtimeSinceStartup;
			
			// Wait until all frame rendering is done
			yield return new WaitForEndOfFrame();
			
			// Set time for the plugin
			SetTimeFromUnity (Time.timeSinceLevelLoad);
			
			// Issue a plugin event with arbitrary integer identifier.
			// The plugin can distinguish between different
			// things it needs to do based on this ID.
			// For our simple plugin, it does not matter which ID we pass here.
			GL.IssuePluginEvent(GetRenderEventFunc(), 1);
			
			float time2 = Time.realtimeSinceStartup;
			float interval = time2 - time1;
//			Debug.Log("time_right: "+interval*1000+"ms.");
		}
	}

}
