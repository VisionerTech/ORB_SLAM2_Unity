#include <opencv2/core/core.hpp>
#include <opencv2/imgproc/imgproc.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <mutex>

#include <System.h>

//#define OPENCV_VIDEO_W 480
//#define OPENCV_VIDEO_H 540

//#define DEBUG_LOG

using namespace cv;
using namespace std;

extern "C"
{
	class SLAM_AR{
	public:
		SLAM_AR();
		~SLAM_AR();

		//SLAM system
		ORB_SLAM2::System* SLAM_system_ptr;

		//remap Mats.
		cv::Mat mx1, my1, mx2, my2;
		Mat mx1_half, my1_half, mx2_half, my2_half;

		//frame
		cv::Mat frame_left, frame_right;
		//frame downsample
		cv::Mat frame_down_left, frame_down_right;
		//frame rectify
		cv::Mat frame_rectify_left, frame_rectify_right;

		//timer for slam tracking
		double tframe;

		//model view matrix
		cv::Mat mTcw;
		double mModelview_matrix[16];

		//domain plane mean and vector in the world coord
		double mPlane_mean[3];
		double mPlane_normal[3];

		//mutex and locks
		std::mutex mMutex;
		std::mutex mMutexFinish;
		bool mbFinishRequested;
		bool mbFinished;

		//init function
		bool init();

		void update(Mat& pSrc, Mat& pSrc_right);
		void run();
		void requestFinish();
		bool checkFinish();
		void setFinish();
		bool isFinished();

		////return the 4*4 gl model view matrix
		double* get_modelview_matrix();

		////return the domain plane mean and normal in the world coord
		double* get_plane_mean();
		double* get_plane_normal();

#ifdef DEBUG_LOG
		ofstream log_file;
#endif
	};
}