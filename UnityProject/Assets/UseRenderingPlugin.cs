// on OpenGL ES there is no way to query texture extents from native texture id
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
	#define UNITY_GLES_RENDERER
#endif


using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;


public class UseRenderingPlugin : MonoBehaviour
{
	// Native plugin rendering events are only called if a plugin is used
	// by some script. This means we have to DllImport at least
	// one function in some active script.
	// For this example, we'll call into plugin's SetTimeFromUnity
	// function and pass the current time so the plugin can animate.

#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport ("__Internal")]
#else
	[DllImport ("RenderingPlugin")]
#endif
	private static extern void SetTimeFromUnity(float t);

	//native function for open web cam.
#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport ("__Internal")]
#else
	[DllImport ("RenderingPlugin", EntryPoint="OpenCamera")]
#endif
	private static extern Boolean OpenCamera();

//	//native debug log print function call back
//	private delegate void DebugCallback(string message);
//	#if UNITY_IPHONE && !UNITY_EDITOR
//	[DllImport ("__Internal")]
//	#else
//	[DllImport ("RenderingPlugin", EntryPoint="RegisterDebugCallback")]
//	#endif
//	private static extern void RegisterDebugCallback(DebugCallback callback);
//
//	private static void DebugMethod(string message)
//	{
//		Debug.Log("UnmanagedCodeTitle: " + message);
//	}

//native function for init a SLAM system.
#if UNITY_IPHONE && !UNITY_EDITOR
[DllImport ("__Internal")]
#else
	[DllImport ("RenderingPlugin", EntryPoint="init_SLAM_AR")]
#endif
	private static extern Boolean init_SLAM_AR();

	//native function for reset the SLAM tracking system.
	#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport ("__Internal")]
	#else
	[DllImport ("RenderingPlugin", EntryPoint="reset_slam")]
	#endif
	private static extern Boolean reset_SLAM();
	

	//native function call for destroying web cam
#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport ("__Internal")]
#else
	[DllImport ("RenderingPlugin", EntryPoint="DestroyWebCam")]
#endif
	private static extern void DestroyWebCam();

//native function for open web cam.
#if UNITY_IPHONE && !UNITY_EDITOR
[DllImport ("__Internal")]
#else
	[DllImport ("RenderingPlugin", EntryPoint="get_map_x1")]
#endif
	private static extern IntPtr get_map_x1();

//native function for open web cam.
#if UNITY_IPHONE && !UNITY_EDITOR
[DllImport ("__Internal")]
#else
[DllImport ("RenderingPlugin", EntryPoint="get_map_y1")]
#endif
private static extern IntPtr get_map_y1();


//native function gets the opengl modelview matrix
#if UNITY_IPHONE && !UNITY_EDITOR
[DllImport ("__Internal")]
#else
[DllImport ("RenderingPlugin", EntryPoint="get_modelview_matrix")]
#endif
private static extern IntPtr get_modelview_matrix();


	//native function gets the domain plane mean in the world coord
	#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport ("__Internal")]
	#else
	[DllImport ("RenderingPlugin", EntryPoint="get_plane_mean")]
	#endif
	private static extern IntPtr get_plane_mean();

	//native function gets the domain plane normal in the world coord
	#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport ("__Internal")]
	#else
	[DllImport ("RenderingPlugin", EntryPoint="get_plane_normal")]
	#endif
	private static extern IntPtr get_plane_normal();

	//native function call for getting return imu quaterion vector length
	#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport ("__Internal")]
	#else
	[DllImport ("RenderingPlugin",EntryPoint="GetIMUQuaterionLength")]
	#endif
	private static extern int GetIMUQuaterionLength();

	//native function for getting return imu quaterion array
	#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport ("__Internal")]
	#else
	[DllImport ("RenderingPlugin", EntryPoint="GetQuaterionVector")]
	#endif
	private static extern IntPtr GetQuaterionVector();

	//native function call for getting return key press data vector length
	#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport ("__Internal")]
	#else
	[DllImport ("RenderingPlugin",EntryPoint="GetKeyDataVecLength")]
	#endif
	private static extern int GetKeyDataVecLength();

	//native function for getting return key data array
	#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport ("__Internal")]
	#else
	[DllImport ("RenderingPlugin", EntryPoint="GetKeyDataVec")]
	#endif
	private static extern IntPtr GetKeyDataVec();

	//native function for getting tracking quality
//	OK = 2
//	lost = 3
	#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport ("__Internal")]
	#else
	[DllImport ("RenderingPlugin", EntryPoint="GetTrackingState")]
	#endif
	private static extern int GetTrackingState();

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
	private static extern void SetTextureFromUnity(System.IntPtr texture);
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

	private GameObject StereoCamera;
	public GameObject WorldPlane;

	private int width = 1080;
	private int height = 1080;

	private Texture2D texture;
	private float[] map_x1;
	private float[] map_y1;

	public Boolean init_success = false;
	public UseRenderingPlugin_right render_sc_right;

//	private double[] mPosition = new double[3];
//	private double[] mOrientation = new double[4];
	private double[] mModelViewMatrix = new double[16];
	private Matrix4x4 W2C_matrix;
	private double[] mPlaneMean = new double[3];
	private double[] mPlaneNormal = new double[3];
	private Vector3 mPlaneMeanVec3;
	private Vector3 mPlaneNormalVec3;
	Vector3 camera_position_LH;

//	private HidRead _hidread;
//	private float imu_roll;
//	private float imu_pitch;
//	private float imu_yaw;

	//a quaterion transfrome from imu frame to v-slam frame
	private Quaternion q_trans_imu2slam;

	public GameObject planeParent;
	public GameObject DualCamera;

	public GameObject WoldPlaneIMU;

	Quaternion imu_q_w_last;
//	Vector3 camera_position_last;

	private float[] imu_q_array = new float[4];
	Quaternion imu_q_image;
	Quaternion imu_q_zero;

	private float[] key_array = new float[1];


	private int lost_state_count = 0;
	//threshold to activate a reset operation if lost is more than this.
	private int lost_threshold = 180;

	//a record of the last camera position, for the shell against lost, initialized in 0,0,0
	Vector3 last_cam_position = new Vector3(0,0,0);
	//a record of the last world 2 the newly reset world.
	Matrix4x4 last_to_reset = Matrix4x4.identity;
	//the imu quaterion in last lost;
	Quaternion last_lost_q;
	//the roatation from last lost to current reset;
	Quaternion last_lost_to_current_q;
	
	IEnumerator Start()
	{
//		RegisterDebugCallback(new DebugCallback(DebugMethod));

		//InvokeRepeating ("PrintIMU", 0f, 1f);
		StereoCamera = GameObject.Find ("StereoCamera");
		WorldPlane = GameObject.Find ("WorldPlane");
		//_hidread = GameObject.Find ("StereoCamera").GetComponent<HidRead> ();
		WoldPlaneIMU = GameObject.Find("WoldPlaneIMU");

		DualCamera = GameObject.Find ("DualCamera");
		planeParent = GameObject.Find ("GameObject");

		if (OpenCamera() && init_SLAM_AR()) {
			Debug.Log("init success!");

			//making a new texture to shader
			texture = new Texture2D (width, height, TextureFormat.RGFloat, false);
			Material mat = GetComponent<Renderer>().material;
			mat.SetTexture ("_Texture2", texture);
			
			//copying the calib params from opencv dll
			map_x1 = new float[width * height];
			map_y1 = new float[width * height];

			IntPtr ptr =  get_map_x1 ();
			Marshal.Copy (ptr, map_x1, 0, width * height);

			ptr = get_map_y1 ();
			Marshal.Copy (ptr, map_y1, 0, width * height);

//			Debug.Log("map x1 0: "+map_x1[1000]);
//			Debug.Log("map x1 1: "+map_x1[1001]);
//			Debug.Log("map x1 2: "+map_x1[2]);
//			Debug.Log("map x1 3: "+map_x1[3]);
//			Debug.Log("map y1 0: "+map_y1[1000]);
//			Debug.Log("map y1 1: "+map_y1[1001]);
//			Debug.Log("map y1 2: "+map_y1[2]);
//			Debug.Log("map y1 3: "+map_y1[3]);

			for (int i=0; i<width; ++i) {
				for(int j=0; j<height; ++j){
					Color color;
					color.r = map_x1[j*width+i]/(float)width;
					color.g = map_y1[j*width+i]/(float)height;
					//				color.r = ((float)i-0.0f)/(float)width;
					//				color.g = ((float)j-0.0f)/(float)height;
					color.b = 0.0f;
					color.a = 1.0f;
					texture.SetPixel(i,j,color);
				}
			}
			
			texture.Apply ();

			init_success = true;

			render_sc_right.enabled = true;


		} else {
			Debug.Log("init false");
		}

		SetUnityStreamingAssetsPath(Application.streamingAssetsPath);

		CreateTextureAndPassToPlugin();
		yield return StartCoroutine("CallPluginAtEndOfFrames");
	}

//	void PrintIMU()
//	{
//		Debug.Log("curr imu_roll :"+_hidread.GetRoll());
//		Debug.Log("curr imu_yaw :"+_hidread.GetYaw());
//		Debug.Log("curr imu_pitch :"+_hidread.GetPitch());
//	}

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
		SetTextureFromUnity (tex.GetNativeTexturePtr());
	#endif
	}


//	//reform a left handed system transform matrix from right handed
//	public static Matrix4x4 LHMatrixFromRHMatrix(Matrix4x4 rhm)
//	{
//		Matrix4x4 lhm = new Matrix4x4();;
//		
//		// Column 0.
//		lhm[0, 0] =  rhm[0, 0];
//		lhm[1, 0] =  rhm[1, 0];
//		lhm[2, 0] = -rhm[2, 0];
//		lhm[3, 0] =  rhm[3, 0];
//		
//		// Column 1.
//		lhm[0, 1] =  rhm[0, 1];
//		lhm[1, 1] =  rhm[1, 1];
//		lhm[2, 1] = -rhm[2, 1];
//		lhm[3, 1] =  rhm[3, 1];
//		
//		// Column 2.
//		lhm[0, 2] = -rhm[0, 2];
//		lhm[1, 2] = -rhm[1, 2];
//		lhm[2, 2] =  rhm[2, 2];
//		lhm[3, 2] = -rhm[3, 2];
//		
//		// Column 3.
//		lhm[0, 3] =  rhm[0, 3];
//		lhm[1, 3] =  rhm[1, 3];
//		lhm[2, 3] = -rhm[2, 3];
//		lhm[3, 3] =  rhm[3, 3];
//		
//		return lhm;
//	}


		//reform a left handed system transform matrix from right handed by negative y
		public static Matrix4x4 LHMatrixFromRHMatrix(Matrix4x4 rhm)
		{
			Matrix4x4 lhm = new Matrix4x4();;
			
			// Column 0.
			lhm[0, 0] =  rhm[0, 0];
			lhm[1, 0] = -rhm[1, 0];
			lhm[2, 0] =  rhm[2, 0];
			lhm[3, 0] =  rhm[3, 0];
			
			// Column 1.
			lhm[0, 1] = -rhm[0, 1];
			lhm[1, 1] =  rhm[1, 1];
			lhm[2, 1] = -rhm[2, 1];
			lhm[3, 1] =  rhm[3, 1];
			
			// Column 2.
			lhm[0, 2] =  rhm[0, 2];
			lhm[1, 2] = -rhm[1, 2];
			lhm[2, 2] =  rhm[2, 2];
			lhm[3, 2] =  rhm[3, 2];
			
			// Column 3.
			lhm[0, 3] =  rhm[0, 3];
			lhm[1, 3] =  -rhm[1, 3];
			lhm[2, 3] =  rhm[2, 3];
			lhm[3, 3] =  rhm[3, 3];
			
			return lhm;
		}

	//get position from transform matrix
	public static Vector3 PositionFromMatrix(Matrix4x4 m)
	{
		return m.GetColumn(3);
	}
	
	//get rotation quaternion from matrix
	public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
	{
		// Trap the case where the matrix passed in has an invalid rotation submatrix.
		if (m.GetColumn(2) == Vector4.zero) {
			Debug.Log("QuaternionFromMatrix got zero matrix.");
			return Quaternion.identity;
		}
		return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
	}

	private IEnumerator CallPluginAtEndOfFrames()
	{
		while (true) {

//			StereoCamera.transform.Rotate(Vector3.up * 50f * Time.deltaTime);

//			float time1 = Time.realtimeSinceStartup;

			// Wait until all frame rendering is done
			yield return new WaitForEndOfFrame();

			// Set time for the plugin
			SetTimeFromUnity (Time.timeSinceLevelLoad);

			// Issue a plugin event with arbitrary integer identifier.
			// The plugin can distinguish between different
			// things it needs to do based on this ID.
			// For our simple plugin, it does not matter which ID we pass here.
			GL.IssuePluginEvent(GetRenderEventFunc(), 0);

			Quaternion zero_slam_q = Quaternion.Euler (new Vector3(0,0,0));

			//get the tracking quality
			int tracking_quality = GetTrackingState ();
			if (tracking_quality == 3) {
				lost_state_count++;
			}

			//if a few lost, we record the imu quaterion to make a lost lost transfrom
			if (lost_state_count == 5) {
				Debug.Log ("10 losts");
//				last_lost_q = DualCamera.transform.rotation;
			}

			//perform a reset operation if lost for a long time(more than threshold frames)
			if (lost_state_count > lost_threshold) {
				Debug.Log ("lost and a shell reset");
//				last_cam_position = camera_position_LH;
//
//				last_lost_to_current_q = Quaternion.Inverse (last_lost_q) * DualCamera.transform.rotation;
//
//				last_to_reset = Matrix4x4.TRS (last_cam_position, last_lost_to_current_q, new Vector3 (1, 1, 1));

				StereoCamera.transform.position = camera_position_LH;
				//StereoCamera.transform.rotation = Quaternion.Euler (new Vector3(_hidread.GetRoll ()-90,_hidread.GetYaw (),_hidread.GetPitch ()));
				StereoCamera.transform.rotation = imu_q_image;

				DualCamera.transform.localPosition = new Vector3 (0, 0, 0);
				DualCamera.transform.localRotation = zero_slam_q;

				reset_SLAM ();
				lost_state_count = 0;

			}

			int quaterion_vec_length = GetIMUQuaterionLength ();
			int key_vec_length = GetKeyDataVecLength ();
//			Debug.Log ("key data length:"+key_vec_length);
//			Debug.Log ("quaterion vector length" + quaterion_vec_length);

//			float time2 = Time.realtimeSinceStartup;
//			float interval = time2 - time1;
//			Debug.Log("time_left: "+interval*1000+"ms.");

//			IntPtr PositionPtr = get_translation();
//			Marshal.Copy (PositionPtr, mPosition, 0, 3);
//			Debug.Log("position 0 :"+mPosition[0]);
//			Debug.Log("position 1 :"+mPosition[1]);
//			Debug.Log("position 2 :"+mPosition[2]);
////
//			IntPtr OrientationPtr = get_orientation();
//			Marshal.Copy (OrientationPtr, mOrientation, 0, 4);
//			Debug.Log("orientation 0 :"+mOrientation[0]);
//			Debug.Log("or ientation 1 :"+mOrientation[1]);
//			Debug.Log("orientation 2 :"+mOrientation[2]);
//			Debug.Log("orientation 2 :"+mOrientation[3]);

			IntPtr ModelViewMatrixPtr = get_modelview_matrix();
			Marshal.Copy (ModelViewMatrixPtr, mModelViewMatrix, 0, 16);
//			Debug.Log("modelview 0 :"+mModelViewMatrix[0]);
//			Debug.Log("modelview 1 :"+mModelViewMatrix[1]);
//			Debug.Log("modelview 2 :"+mModelViewMatrix[2]);
//			Debug.Log("modelview 3 :"+mModelViewMatrix[3]);
//			Debug.Log("modelview 4 :"+mModelViewMatrix[4]);
//			Debug.Log("modelview 5 :"+mModelViewMatrix[5]);
//			Debug.Log("modelview 6 :"+mModelViewMatrix[6]);
//			Debug.Log("modelview 7 :"+mModelViewMatrix[7]);
//			Debug.Log("modelview 8 :"+mModelViewMatrix[8]);
//			Debug.Log("modelview 9 :"+mModelViewMatrix[9]);
//			Debug.Log("modelview 10 :"+mModelViewMatrix[10]);
//			Debug.Log("modelview 11 :"+mModelViewMatrix[11]);
//			Debug.Log("modelview 12 :"+mModelViewMatrix[12]);
//			Debug.Log("modelview 13 :"+mModelViewMatrix[13]);
//			Debug.Log("modelview 14 :"+mModelViewMatrix[14]);
//			Debug.Log("modelview 15 :"+mModelViewMatrix[15]);

			//transfrom to column based
			W2C_matrix.m00 = (float)mModelViewMatrix [0];
			W2C_matrix.m01 = (float)mModelViewMatrix [4];
			W2C_matrix.m02 = (float)mModelViewMatrix [8];
			W2C_matrix.m03 = (float)mModelViewMatrix[12];
			W2C_matrix.m10 = (float)mModelViewMatrix [1];
			W2C_matrix.m11 = (float)mModelViewMatrix [5];
			W2C_matrix.m12 = (float)mModelViewMatrix [9];
			W2C_matrix.m13 = (float)mModelViewMatrix [13];
			W2C_matrix.m20 = (float)mModelViewMatrix [2];
			W2C_matrix.m21 = (float)mModelViewMatrix [6];
			W2C_matrix.m22 = (float)mModelViewMatrix [10];
			W2C_matrix.m23 = (float)mModelViewMatrix [14] ;
			W2C_matrix.m30 = 0;
			W2C_matrix.m31 = 0;
			W2C_matrix.m32 = 0;
			W2C_matrix.m33 = 1;
//

//			Debug.Log ("before w2c"+W2C_matrix);
//
//			//a last lost world to current newly reset world transfrom, to change it to the last lost world,only imu rotation is used in this transfrom.
//			W2C_matrix = last_to_reset * W2C_matrix;
//			Debug.Log ("after w2c"+W2C_matrix);

			Matrix4x4 C2W_matrix_RH = W2C_matrix.inverse;
			Vector3 camera_position_RH = PositionFromMatrix (C2W_matrix_RH);
			camera_position_LH = camera_position_RH;
			camera_position_LH.y = -camera_position_LH.y;

			//update the camera position with the last camera position offset for the shell against lost.
//			camera_position_LH = camera_position_LH + last_cam_position;

//			Debug.Log ("camera_position_LH "+camera_position_LH);
//			Vector3 pt = last_to_reset.MultiplyPoint3x4 (camera_position_LH);
//			Debug.Log ("pt "+pt);
//
//			Quaternion camera_rotation_RH = QuaternionFromMatrix (W2C_matrix);
//			camera_rotation_RH.y = -camera_rotation_RH.y; 


//			Matrix4x4 W2C_matrix_LH = LHMatrixFromRHMatrix(W2C_matrix);
//			Matrix4x4 C2W_matrix_LH = W2C_matrix_LH.inverse;
//
//			Vector3 arPosition = PositionFromMatrix(C2W_matrix_LH);
//			Quaternion arRotation = QuaternionFromMatrix(W2C_matrix_LH);
//
//			Vector3 scale = new Vector3(1, 1, 1);
//			Matrix4x4 W2C_matrix_LH = Matrix4x4.TRS(camera_position_RH, camera_rotation_RH, scale);
//			StereoCamera.transform.localPosition = camera_position_LH;

			DualCamera.transform.localPosition = Vector3.Slerp(DualCamera.transform.localPosition, camera_position_LH, Time.deltaTime*30);
//			camera_position_last = camera_position_LH;

//			Quaternion imu_q = Quaternion.Euler (new Vector3(_hidread.GetRoll (),_hidread.GetYaw (),_hidread.GetPitch ()));
//	
//			Vector3 translation = new Vector3(0,0,0);
//			Vector3 scale = new Vector3 (1, 1, 1);
//			Matrix4x4 m = Matrix4x4.TRS (translation, imu_q, scale);
//
//			Matrix4x4 trans_imu_first_2_current = Matrix4x4.Inverse (imu_init_m) * m;
//
////			Matrix4x4 slam_m_imu = imuframe_2_slameframe*m;
//			Matrix4x4 slam_m_imu = Matrix4x4.identity * trans_imu_first_2_current;
//
//			Quaternion rotation_q_imu = QuaternionFromMatrix (slam_m_imu);

//			float z =  (_hidread.GetPitch () - imu_pitch);
//			float y =  (_hidread.GetYaw () - imu_yaw);
//			float x =  ( _hidread.GetRoll () - imu_roll);

		
//			Quaternion zero_imu_q = Quaternion.Euler (new Vector3(imu_roll,imu_yaw,imu_pitch));
//
//			Quaternion trans = Quaternion.Inverse (zero_imu_q) * imu_q;
//

//
//			Quaternion current_q =   trans;
//			Vector3 current_euler = current_q.eulerAngles;
//			float temp_y = current_euler.y;
//			current_euler.y = current_euler.y;
//			current_euler.z = temp_y;
//			current_q = Quaternion.Euler (current_euler);

//			StereoCamera.transform.localRotation = Quaternion.Euler (new Vector3(_hidread.GetRoll ()-90,_hidread.GetYaw (),_hidread.GetPitch ()));
//			StereoCamera.transform.localRotation = Quaternion.Euler (new Vector3(x,y,z));
//			StereoCamera.transform.localRotation = current_q;
//			StereoCamera.transform.localRotation = rotation_q_imu;
//			StereoCamera.transform.localRotation = Quaternion.FromToRotation (new Vector3(imu_roll,imu_yaw,imu_pitch), new Vector3(_hidread.GetRoll (), _hidread.GetYaw () , _hidread.GetPitch ()));
//
//			StereoCamera.transform.localRotation = camera_rotation_RH;
			//StereoCamera.transform.Rotate (Vector3.up * 180f);
			//StereoCamera.transform.Rotate (Vector3.forward*180f);
			//StereoCamera.transform.Rotate (Vector3.right*180f);

			//imu key press 
			if (key_vec_length > 0) {
				//
				Debug.Log("HMD key pressed!");
				IntPtr ptrd_key_vec = GetKeyDataVec ();

				Marshal.Copy (ptrd_key_vec, key_array, 0, 1); 
				Debug.Log ("key:"+key_array[0]);
			}

			if (quaterion_vec_length > 4) {
				IntPtr ptr = GetQuaterionVector ();
				Marshal.Copy (ptr, imu_q_array, 0, 4);
//								Debug.Log ("w:" + imu_q_array [0]);
//								Debug.Log ("x:" + imu_q_array [1]);
//								Debug.Log ("y:" + imu_q_array [2]);
//								Debug.Log ("z:" + imu_q_array [3]);

				imu_q_image = new Quaternion (imu_q_array [1], -imu_q_array [2], -imu_q_array [3], imu_q_array [0]);

				//DualCamera.transform.rotation = imu_q_image;
//				DualCamera.transform.rotation = Quaternion.Slerp(imu_q_w_last, imu_q_image, Time.deltaTime);
				DualCamera.transform.rotation = Quaternion.Slerp(DualCamera.transform.rotation, imu_q_image, Time.deltaTime*80);
//				imu_q_w_last = imu_q_image;
				//DualCamera.transform.Rotate (Vector3.right * -90.0f);
//				DualCamera.transform.Rotate (Vector3.up * 180.0f);
			}

//			Quaternion imu_q_w = Quaternion.Euler( new Vector3 (_hidread.GetRoll () - 90, _hidread.GetYaw (), _hidread.GetPitch ()));
//			DualCamera.transform.rotation = Quaternion.Slerp(imu_q_w_last, imu_q_w, Time.deltaTime);
//
//			imu_q_w_last = imu_q_w;
		
//			Quaternion local_rotate_q = DualCamera.transform.localRotation;
//
//			Quaternion interp_rotate_q = Quaternion.Slerp (local_rotate_q ,camera_rotation_RH , 0.5f);
//
//			DualCamera.transform.localRotation = interp_rotate_q;


//			WorldPlane.transform.position = camera_position_LH;



			if (b_reseting == true) {
				StereoCamera.transform.position = camera_position_LH;
				//StereoCamera.transform.rotation = Quaternion.Euler (new Vector3(_hidread.GetRoll ()-90,_hidread.GetYaw (),_hidread.GetPitch ()));
				StereoCamera.transform.rotation = imu_q_image;

				DualCamera.transform.localPosition = new Vector3 (0, 0, 0);
				DualCamera.transform.localRotation = zero_slam_q;
				lost_state_count = 0;

				//set positino and rotation of the world plane
				IntPtr planeMeanPtr = get_plane_mean ();
				Marshal.Copy (planeMeanPtr, mPlaneMean, 0, 3);
				IntPtr planeNormalPtr = get_plane_normal ();
				Marshal.Copy (planeNormalPtr, mPlaneNormal, 0, 3);
				//			Debug.Log("mPlaneMean 0 :"+mPlaneMean[0]);
				//			Debug.Log("mPlaneMean 1 :"+mPlaneMean[1]);
				//			Debug.Log("mPlaneMean 2 :"+mPlaneMean[2]);
				//			Debug.Log("mPlaneNormal 0 :"+mPlaneNormal[0]);
				//			Debug.Log("mPlaneNormal 1 :"+mPlaneNormal[1]);
				//			Debug.Log("mPlaneNormal 2 :"+mPlaneNormal[2]);

				mPlaneMeanVec3.Set ((float)mPlaneMean[0], -(float)mPlaneMean[1], (float)mPlaneMean[2]);
				mPlaneNormalVec3.Set ((float)mPlaneNormal[0], -(float)mPlaneNormal[1], (float)mPlaneNormal[2]);

				//making the normal vector always up and face the camera.
				if (mPlaneNormalVec3.y < 0) {
					mPlaneNormalVec3 = -mPlaneNormalVec3;
				}

//				WorldPlane.transform.localPosition = mPlaneMeanVec3;
//				WorldPlane.transform.localRotation = Quaternion.FromToRotation (Vector3.up, mPlaneNormalVec3);
				//			WorldPlane.transform.Rotate (Vector3.right * 180f);
				//			WorldPlane.transform.Rotate (Vector3.up * 180f);


				//reseting the last camear position to zero vector
				last_cam_position.Set(0,0,0);

				//try to make a imu world position to virtual object.
				WoldPlaneIMU.transform.parent = StereoCamera.transform;
				WoldPlaneIMU.transform.localPosition = new Vector3 (0, -3, 4);
//				WoldPlaneIMU.transform.position = new Vector3 (0, 5, 0);
//				WoldPlaneIMU.transform.rotation = zero_slam_q;
//				WoldPlaneIMU.transform.RotateAround(DualCamera.transform.position, DualCamera.transform.up, DualCamera.transform.rotation.eulerAngles.y);
			}

			if (b_reseting == true) {
				b_reseting = false;
				WoldPlaneIMU.transform.SetParent (null, true);
			}

        }
	}



	void OnApplicationQuit()
	{
		DestroyWebCam ();
		Debug.Log("quit");
	}

	Matrix4x4 imu_init_m;
	Matrix4x4 imuframe_2_slameframe;

	Boolean b_reseting = false;

	public void OnButtonRestClick()
	{
//		imu_roll = _hidread.GetRoll ();
//		imu_yaw = _hidread.GetYaw ();
//		imu_pitch = _hidread.GetPitch ();
//		Debug.Log("init imu_roll :"+imu_roll);
//		Debug.Log("init imu_yaw :"+imu_yaw);
//		Debug.Log("init imu_pitch :"+imu_pitch);
//
//		Quaternion imu_q = Quaternion.Euler (new Vector3(imu_roll,imu_yaw,imu_pitch));
//
//		Vector3 translation = new Vector3(0,0,0);
//		Vector3 scale = new Vector3 (1, 1, 1);
//
//		imu_init_m = Matrix4x4.TRS (translation, imu_q, scale);
//
//		Matrix4x4 slam_frame = Matrix4x4.identity;
//
//		imuframe_2_slameframe = Matrix4x4.Inverse(imu_init_m) * slam_frame;
////
		imu_q_zero = imu_q_image;
//		planeParent.transform.position = StereoCamera.transform.position;
//		//planeParent.transform.rotation = Quaternion.Euler(new Vector3(imu_roll-90, imu_yaw, imu_pitch));
//		planeParent.transform.rotation = imu_q_zero;

		reset_SLAM ();
//		Debug.Log ("click");

		b_reseting = true;
	}
}
