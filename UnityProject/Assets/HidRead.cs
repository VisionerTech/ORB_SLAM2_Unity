using UnityEngine;
using System;
using System.Collections.Generic;
//using System.Data;
//using System.Drawing;
//using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections;
using System.Text.RegularExpressions;
using System.ComponentModel;

public class HidRead : MonoBehaviour {

	static string NAME = "HidReadGO";
	private static HidRead _instance;
	public static HidRead Instance()
	{
		if (_instance == null) {
			GameObject go = GameObject.Find (NAME);
			if(go == null)
			{
				go = new GameObject (NAME);
				DontDestroyOnLoad (go);
				go.AddComponent<HidRead> ();
			}
			HidRead comp = go.GetComponent<HidRead> ();
			if (comp == null) {
				go.AddComponent<HidRead> ();
			}
			_instance = go.GetComponent<HidRead> ();
		}

		return _instance;
	}

    //以下是调用windows的API的函数

    // HIDP_CAPS
    [StructLayout(LayoutKind.Sequential)]
    internal struct HIDP_CAPS
    {
        public System.UInt16 Usage;					// USHORT
        public System.UInt16 UsagePage;				// USHORT
        public System.UInt16 InputReportByteLength;
        public System.UInt16 OutputReportByteLength;
        public System.UInt16 FeatureReportByteLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
        public System.UInt16[] Reserved;				// USHORT  Reserved[17];			
        public System.UInt16 NumberLinkCollectionNodes;
        public System.UInt16 NumberInputButtonCaps;
        public System.UInt16 NumberInputValueCaps;
        public System.UInt16 NumberInputDataIndices;
        public System.UInt16 NumberOutputButtonCaps;
        public System.UInt16 NumberOutputValueCaps;
        public System.UInt16 NumberOutputDataIndices;
        public System.UInt16 NumberFeatureButtonCaps;
        public System.UInt16 NumberFeatureValueCaps;
        public System.UInt16 NumberFeatureDataIndices;
    }

    //获得GUID
    [DllImport("hid.dll")]
    public static extern void HidD_GetHidGuid(ref Guid HidGuid);
    Guid guidHID = Guid.Empty;

    [DllImport("hid.dll", SetLastError = true)]
    internal static extern int HidD_GetPreparsedData(
        int hObject,								// IN HANDLE  HidDeviceObject,
        ref int pPHIDP_PREPARSED_DATA);

    //Get Device Detail Information
    [DllImport("hid.dll", SetLastError = true)]
    internal static extern int HidP_GetCaps(
        int pPHIDP_PREPARSED_DATA,					// IN PHIDP_PREPARSED_DATA  PreparsedData,
        ref HIDP_CAPS myPHIDP_CAPS);				// OUT PHIDP_CAPS  Capabilities

    //过滤设备，获取需要的设备
    [DllImport("setupapi.dll", SetLastError = true)]
    public static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, uint Enumerator, IntPtr HwndParent, DIGCF Flags);
    IntPtr hDevInfo;
    //获取设备，true获取到
    [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern Boolean SetupDiEnumDeviceInterfaces(IntPtr hDevInfo, IntPtr devInfo, ref Guid interfaceClassGuid, UInt32 memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);
    public struct SP_DEVICE_INTERFACE_DATA
    {
        public int cbSize;
        public Guid interfaceClassGuid;
        public int flags;
        UIntPtr reserved;
    }

    // 获取接口的详细信息 必须调用两次 第1次返回长度 第2次获取数据 
    [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoSet, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData,
        int deviceInterfaceDetailDataSize, ref int requiredSize, SP_DEVINFO_DATA deviceInfoData);
    [StructLayout(LayoutKind.Sequential)]
    public class SP_DEVINFO_DATA
    {
        public int cbSize = Marshal.SizeOf(typeof(SP_DEVINFO_DATA));
        public Guid classGuid = Guid.Empty; // temp
        public int devInst = 0; // dumy
        UIntPtr reserved = UIntPtr.Zero;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal struct SP_DEVICE_INTERFACE_DETAIL_DATA
    {
        internal int cbSize;
        internal short devicePath;
    }

    public enum DIGCF
    {
        DIGCF_DEFAULT = 0x1,
        DIGCF_PRESENT = 0x2,
        DIGCF_ALLCLASSES = 0x4,
        DIGCF_PROFILE = 0x8,
        DIGCF_DEVICEINTERFACE = 0x10
    }

    //获取设备文件
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern int CreateFile(
        string lpFileName,                            // file name
        uint dwDesiredAccess,                        // access mode
        uint dwShareMode,                            // share mode
        uint lpSecurityAttributes,                    // SD
        uint dwCreationDisposition,                    // how to create
        uint dwFlagsAndAttributes,                    // file attributes
        uint hTemplateFile                            // handle to template file
        );
    //读取设备文件
    [DllImport("Kernel32.dll", SetLastError = true)]
    private static extern bool ReadFile
    (
            IntPtr hFile,
            byte[] lpBuffer,
            uint nNumberOfBytesToRead,
            ref uint lpNumberOfBytesRead,
            IntPtr lpOverlapped
     );

    //释放设备
    [DllImport("hid.dll")]
    static public extern bool HidD_FreePreparsedData(ref IntPtr PreparsedData);
    //关闭访问设备句柄，结束进程的时候把这个加上保险点
    [DllImport("kernel32.dll")]
    static public extern int CloseHandle(int hObject);
    [DllImport("setupapi.dll", SetLastError = true)]
    internal static extern IntPtr SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

    // ----------------------------------------------------------------------------------------------------------------------------------------------- //



    //代码暂时没有整理，传入参数是设备序号,
    //有些USB设备其实有很多HID设备，就是一个接口上有几个设备，这个时候需要
    //用index++来逐个循环，直到获取设备返回false后，跳出去，把获取的设备
    //路径全记录下来就好了，我这里知道具体设备号，所以没有循环，浪费我时间

    //定于句柄序号和一些参数，具体可以去网上找这些API的参数说明，后文我看能不能把资料也写上去
    int HidHandle = -1;
    public const uint GENERIC_READ = 0x80000000;
    public const uint GENERIC_WRITE = 0x40000000;
    public const uint FILE_SHARE_READ = 0x00000001;
    public const uint FILE_SHARE_WRITE = 0x00000002;
    public const int OPEN_EXISTING = 3;

    string InDriveName = "", devicePathName = "";
    private bool UsBMethod(string VID, string PID)
    {
        HidD_GetHidGuid(ref guidHID);
        hDevInfo = SetupDiGetClassDevs(ref guidHID, 0, IntPtr.Zero, DIGCF.DIGCF_PRESENT | DIGCF.DIGCF_DEVICEINTERFACE);
        {
            int errCode = Marshal.GetLastWin32Error();
            string errorMessage = new Win32Exception(Marshal.GetLastWin32Error()).Message;
            Debug.LogError(errorMessage);
        }
        int bufferSize = 0;
        ArrayList HIDUSBAddress = new ArrayList();

        int index = 0;
        while (true)
        {
            //获取设备，true获取到
            SP_DEVICE_INTERFACE_DATA DeviceInterfaceData = new SP_DEVICE_INTERFACE_DATA();
            DeviceInterfaceData.cbSize = Marshal.SizeOf(DeviceInterfaceData);

            bool result = false;
            result = SetupDiEnumDeviceInterfaces(hDevInfo, IntPtr.Zero, ref guidHID, (UInt32)index, ref DeviceInterfaceData);
            if (!result)
            {
                int errCode = Marshal.GetLastWin32Error();
                string errorMessage = new Win32Exception(Marshal.GetLastWin32Error()).Message;
                Debug.LogError(errorMessage);
            }

            //第一次调用出错，但可以返回正确的Size 
            SP_DEVINFO_DATA strtInterfaceData = new SP_DEVINFO_DATA();
            strtInterfaceData.cbSize = Marshal.SizeOf(strtInterfaceData);
            result = SetupDiGetDeviceInterfaceDetail(hDevInfo, ref DeviceInterfaceData, IntPtr.Zero, 0, ref bufferSize, strtInterfaceData);
            //第二次调用传递返回值，调用即可成功
            IntPtr detailDataBuffer = Marshal.AllocHGlobal(bufferSize);
            SP_DEVICE_INTERFACE_DETAIL_DATA detailData = new SP_DEVICE_INTERFACE_DETAIL_DATA();
            detailData.cbSize = Marshal.SizeOf(typeof(SP_DEVICE_INTERFACE_DETAIL_DATA));
            //detailData.cbSize = 4 + Marshal.SystemDefaultCharSize;
            // 64位平台下，cbSize为8
            if (IntPtr.Size == 8)
                detailData.cbSize = 8;
            Marshal.StructureToPtr(detailData, detailDataBuffer, false);
            result = SetupDiGetDeviceInterfaceDetail(hDevInfo, ref DeviceInterfaceData, detailDataBuffer, bufferSize, ref bufferSize, strtInterfaceData);
            if (!result)
            {
                int errCode = Marshal.GetLastWin32Error();
                string errorMessage = new Win32Exception(Marshal.GetLastWin32Error()).Message;
                Debug.LogError(errorMessage);
            }
            //获取设备路径访
            IntPtr pdevicePathName = (IntPtr)((int)detailDataBuffer + 4);
            devicePathName = Marshal.PtrToStringAuto(pdevicePathName);
            Debug.Log(devicePathName);


            if (CheckDeviceName(devicePathName, VID, PID))
            //                    if (devicePathName.IndexOf(DriveName.ToLower()) > 0)
            {
                InDriveName = devicePathName;
                HIDUSBAddress.Add(devicePathName);
                break;
            }
            //HIDUSBAddress.Add(devicePathName);

            index++;
            if(index > 10)
            break;
        }

        //连接设备文件
        int aa = CT_CreateFile(devicePathName);
        if (HIDUSBAddress.Count > 0 && aa == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    string giVIDCfg = "vid_";
    string giPIDCfg = "pid_";
    //check VID,PID,DID
    private bool CheckDeviceName(string Name, string VID, string PID)
    {
        string tmpVID = "";
        string tmpPID = "";

        int pivcount = 0;//position of the VID,PID,DID in Name

        Regex VD = new Regex(giVIDCfg, RegexOptions.IgnoreCase);
        Regex PD = new Regex(giPIDCfg, RegexOptions.IgnoreCase);


        MatchCollection matches_VD = VD.Matches(Name);
        MatchCollection matches_PD = PD.Matches(Name);

        if (matches_VD.Count > 0)
        {
            char[] c = Name.ToCharArray();
            foreach (Match match in matches_VD)
            {
                GroupCollection groups = match.Groups;
                pivcount = groups[0].Index + match.Length;
                for (int i = pivcount; i < pivcount + 4; i++)
                {
                    tmpVID = tmpVID + c[i];//Get VID name
                }
            }
        }
        if (matches_PD.Count > 0)
        {
            char[] c = Name.ToCharArray();
            foreach (Match match in matches_PD)
            {
                GroupCollection groups = match.Groups;
                pivcount = groups[0].Index + match.Length;
                for (int i = pivcount; i < pivcount + 4; i++)
                {
                    tmpPID = tmpPID + c[i];//Get PID name
                }
            }
        }

        //check VID,PID,DID same?
        if ((tmpVID == VID) && (tmpPID == PID))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //建立和设备的连接
    public int CT_CreateFile(string DeviceName)
    {
        HidHandle = CreateFile(
            DeviceName,
            GENERIC_READ,// | GENERIC_WRITE,//读写，或者一起
            FILE_SHARE_READ,// | FILE_SHARE_WRITE,//共享读写，或者一起
            0,
            OPEN_EXISTING,
            0,
            0);
        if (HidHandle == -1)
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }


    int GetReportLenth()
    {
        int myPtrToPreparsedData = -1;
        int result1 = HidD_GetPreparsedData(HidHandle, ref myPtrToPreparsedData);
        HIDP_CAPS myHIDP_CAPS = new HIDP_CAPS();
        int result2 = HidP_GetCaps(myPtrToPreparsedData, ref myHIDP_CAPS);
        int reportLength = myHIDP_CAPS.InputReportByteLength;
        return reportLength;
    }
		

    private Thread thread;
	private List<byte> databuff = new List<byte>();
	private List<byte> finalDatabuff = new List<byte> ();
    void Start()
    {
        Debug.Log("start!");
        quit = false;
        if (UsBMethod("03eb", "2043"))
        {
            Debug.Log("open");
            thread = new Thread(USBDataRead);
            thread.IsBackground = true;
            thread.Start();
        }
        else
        {
            throw new UnityException("hid获取失败");
        }
    }

    float roll, pitch, yaw;
    Vector3 targetEulerAngles = Vector3.zero;
    float speed = 100f;
	 Transform targetTrans{ get; set;}
    void Update()
    {
//
//        //if (targetTrans == null)
//        //    return;
//        targetTrans = transform;
//
//        targetEulerAngles.x = roll - 90f;
//        //targetEulerAngles.x = roll;
//        targetEulerAngles.y = yaw;
//        targetEulerAngles.z = pitch;
//
//        Quaternion targetQuat = Quaternion.Euler(targetEulerAngles);
//		targetTrans.rotation = Quaternion.Lerp(targetTrans.transform.rotation, targetQuat, Time.deltaTime * speed);
    }

	public float GetRoll()
	{
		return roll;
	}

	public float GetPitch()
	{
		return pitch;
	}

	public float GetYaw()
	{
		return yaw;
	}

	short acc_x;
	short acc_y;
	short acc_z;
 

    //根据CreateFile拿到的设备handle访问文件，并返回数据
    bool readHead;
    int read_data_cnt = -1, startIndex;
    List<byte> temBuffer = new List<byte>();
    int index = -1;
    public void USBDataRead()
    {
        read_data_cnt += 1;

        int reportLength = 0;

        if (HidHandle != -1)
        {
            reportLength = GetReportLenth();
        }
        Byte[] m_rd_data = new Byte[reportLength];
        while (!quit)
        {
                uint read = 0;
                //注意字节的长度，我这里写的是8位，其实可以通过API获取具体的长度，这样安全点，
                //具体方法我知道，但是没有写，过几天整理完代码，一起给出来
                Array.Clear(m_rd_data, 0, m_rd_data.Length);
                bool isread = ReadFile((IntPtr)HidHandle, m_rd_data, (uint)reportLength, ref read, IntPtr.Zero);

                //这里已经是拿到的数据了
                Byte[] m_rd_dataout = new Byte[read];
                Array.Copy(m_rd_data, m_rd_dataout, read);


                if (readHead)
                    startIndex = 1;
                else
                    startIndex = 0;

                for (int i = startIndex; i < m_rd_data.Length; i++)
                {

                    if (i < m_rd_data.Length - 1)
                    {
                        if (m_rd_data[i].ToString("x") == "88" && m_rd_data[i + 1].ToString("x") == "af" && !readHead)
                            readHead = true;
                    }


                    if (readHead)
                    {
                        if (finalDatabuff.Count == reportLength - 1)
                        {
                            finalDatabuff.RemoveAt(0);
                            //for (int w = 0; w < finalDatabuff.Count; w++)
                            //    Debug.Log(w + "::" + finalDatabuff[w].ToString("x"));

						acc_x = (short)((short)(finalDatabuff[3] << 8) + finalDatabuff[4]);
						acc_y = (short)((short)(finalDatabuff[5] << 8) + finalDatabuff[6]);
						acc_z = (short)((short)(finalDatabuff[7] << 8) + finalDatabuff[8]);

                            roll = (float)((short)((short)(finalDatabuff[23] << 8) + finalDatabuff[24]) / 100f);
                            pitch = (float)((short)(((short)finalDatabuff[21] << 8) + finalDatabuff[22]) / 100f);
                            yaw = (float)(((finalDatabuff[25] << 8) + finalDatabuff[26]) / 10f);

                            finalDatabuff.Clear();
                            readHead = false;

//						Debug.Log ("acc_x: "+acc_x);
//						Debug.Log ("acc_y: "+acc_y);
//						Debug.Log ("acc_z: "+acc_z);
                            //Debug.Log(read_data_cnt + ":" + str);
                            //str = "读取数据";
                        }

                        if (finalDatabuff.Count != reportLength - 1)
                        {
                            finalDatabuff.Add(m_rd_data[i]);
                        }
                    }
                 }
        }


    }

    bool quit;
    void OnApplicationQuit()
    {
        quit = true;
        thread.Abort();
        //释放设备资源(hDevInfo是SetupDiGetClassDevs获取的)
        SetupDiDestroyDeviceInfoList(hDevInfo);
        //关闭连接(HidHandle是Create的时候获取的)
        CloseHandle(HidHandle);
    }


}
