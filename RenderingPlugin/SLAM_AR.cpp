#include "SLAM_AR.h"
#ifdef DEBUG_LOG
#include <iostream>
#include <fstream>
typedef void(__stdcall * DebugCallback) (const char * str);
DebugCallback gDebugCallback;
#endif

#ifdef DEBUG_LOG
void DebugInUnity(std::string message)
{
	if (gDebugCallback)
	{
		gDebugCallback(message.c_str());
	}
}


extern "C" void __declspec(dllexport) RegisterDebugCallback(DebugCallback callback)
{
	if (callback)
	{
		gDebugCallback = callback;
	}
}

#endif

SLAM_AR::SLAM_AR()
{
	mbFinishRequested = false;
	mbFinished = false;
#ifdef DEBUG_LOG
	log_file.open("log.txt");
#endif
}

SLAM_AR::~SLAM_AR()
{
	delete SLAM_system_ptr;
#ifdef DEBUG_LOG
	log_file.close();
#endif
}

bool SLAM_AR::init()
{
	string path_save_param = "./save_param/calib_para.yml";
	string path_vocabulary = "./Vocabulary/ORBvoc.bin";
	string path_setting = "./stereo_unity.yaml";

	mx1.create(OPENCV_VIDEO_H, OPENCV_VIDEO_W, CV_16S);
	my1.create(OPENCV_VIDEO_H, OPENCV_VIDEO_W, CV_16S);
	mx2.create(OPENCV_VIDEO_H, OPENCV_VIDEO_W, CV_16S);
	my2.create(OPENCV_VIDEO_H, OPENCV_VIDEO_W, CV_16S);

	cv::FileStorage fs(path_save_param, CV_STORAGE_READ);
	fs["MX1"] >> mx1;
	fs["MX2"] >> mx2;
	fs["MY1"] >> my1;
	fs["MY2"] >> my2;
	fs.release();

	//converting the calibration maps to half
	cv::Mat mx, my;
	cv::convertMaps(mx1, my1, mx, my, CV_32FC1);

	cv::Mat mx_r, my_r;
	cv::convertMaps(mx2, my2, mx_r, my_r, CV_32FC1);

	resize(mx, mx1_half, Size(mx.cols / 2, mx.rows / 2), CV_INTER_LINEAR);
	mx1_half = mx1_half / 2.0f;

	resize(my, my1_half, Size(my.cols / 2, my.rows / 2), CV_INTER_LINEAR);
	my1_half = my1_half / 2.0f;

	resize(mx_r, mx2_half, Size(mx_r.cols / 2, mx_r.rows / 2), CV_INTER_LINEAR);
	mx2_half = mx2_half / 2.0f;

	resize(my_r, my2_half, Size(my_r.cols / 2, my_r.rows / 2), CV_INTER_LINEAR);
	my2_half = my2_half / 2.0f;
	//


	SLAM_system_ptr = new ORB_SLAM2::System(path_vocabulary, path_setting, ORB_SLAM2::System::STEREO, true);

	tframe = 0.0f;

	//seting the modelvew matrix to identity 
	mModelview_matrix[0] = 1.0f;
	mModelview_matrix[1] = 0.0f;
	mModelview_matrix[2] = 0.0f;
	mModelview_matrix[3] = 0.0f;

	mModelview_matrix[4] = 0.0f;
	mModelview_matrix[5] = 1.0f;
	mModelview_matrix[6] = 0.0f;
	mModelview_matrix[7] = 0.0f;

	mModelview_matrix[8] = 0.0f;
	mModelview_matrix[9] = 0.0f;
	mModelview_matrix[10] = 1.0f;
	mModelview_matrix[11] = 0.0;

	mModelview_matrix[12] = 0.0f;
	mModelview_matrix[13] = 0.0f;
	mModelview_matrix[14] = 0.0f;
	mModelview_matrix[15] = 1.0;

	//setting the domain plane mean and normal to zeros
	mPlane_mean[0] = 0.0f;
	mPlane_mean[1] = 0.0f;
	mPlane_mean[2] = 0.0f;

	mPlane_normal[0] = 0.0f;
	mPlane_normal[1] = 0.0f;
	mPlane_normal[2] = 0.0f;

	return true;
}

void SLAM_AR::update(Mat& pSrc, Mat& pSrc_right)
{
	unique_lock<mutex> lock(mMutex);
	pSrc.copyTo(frame_left);
	pSrc_right.copyTo(frame_right);

//	if (!frame_left.empty() && !frame_right.empty())
//	{
//		imshow("left_update", frame_left);
//		imshow("right_update", frame_right);
//		waitKey();
//	}


}

void SLAM_AR::run()
{

	while (1)
	{

		if (frame_left.empty() || frame_right.empty())
		{
			waitKey(30);
			continue;
		}

		cv::resize(frame_left, frame_down_left, cv::Size(OPENCV_VIDEO_W, OPENCV_VIDEO_H));
		cv::resize(frame_right, frame_down_right, cv::Size(OPENCV_VIDEO_W, OPENCV_VIDEO_H));

		cv::remap(frame_down_left, frame_rectify_left, mx1_half, my1_half, CV_INTER_LINEAR);
		cv::remap(frame_down_right, frame_rectify_right, mx2_half, my2_half, CV_INTER_LINEAR);

		tframe += 0.0003f;


		//imshow("left", frame_rectify_left);
		//imshow("right", frame_rectify_right);
		//waitKey(30);

#ifdef DEBUG_LOG
		log_file << "tframe " << tframe << endl;
		if (!frame_rectify_left.empty() && !frame_rectify_right.empty())
		{
			log_file << "image left" << frame_rectify_left.size() << endl;
			log_file << "image right" << frame_rectify_right.size() << endl;
		}
		else
		{
			log_file << "frame source empty" << endl;
		}

#endif

#ifdef COMPILEDWITHC11
		std::chrono::steady_clock::time_point t1 = std::chrono::steady_clock::now();
#else
		std::chrono::monotonic_clock::time_point t1 = std::chrono::monotonic_clock::now();
#endif

		mTcw = SLAM_system_ptr->TrackStereo(frame_rectify_left, frame_rectify_right, tframe);


		//imshow("left", frame_rectify_left);
		//imshow("right", frame_rectify_right);
		//waitKey(30);

#ifdef COMPILEDWITHC11
		std::chrono::steady_clock::time_point t2 = std::chrono::steady_clock::now();
#else
		std::chrono::monotonic_clock::time_point t2 = std::chrono::monotonic_clock::now();
#endif

		double ttrack = std::chrono::duration_cast<std::chrono::duration<double> >(t2 - t1).count();

#ifdef DEBUG_LOG
		log_file <<"tracking state "<< SLAM_system_ptr->mpTracker->mState << endl;
#endif

		if (!mTcw.empty())
			{
				//std::stringstream sstrMat;
				//sstrMat << mTcw;
				//DebugInUnity(sstrMat.str());

				cv::Mat Rcw(3, 3, CV_32F);
				cv::Mat tcw(3,1,CV_32F);

				Rcw = mTcw.rowRange(0, 3).colRange(0, 3);
				tcw = mTcw.rowRange(0, 3).col(3);

				mModelview_matrix[0] = Rcw.at<float>(0, 0);
				mModelview_matrix[1] = Rcw.at<float>(1, 0);
				mModelview_matrix[2] = Rcw.at<float>(2, 0);
				mModelview_matrix[3] = 0.0;
				
				mModelview_matrix[4] = Rcw.at<float>(0, 1);
				mModelview_matrix[5] = Rcw.at<float>(1, 1);
				mModelview_matrix[6] = Rcw.at<float>(2, 1);
				mModelview_matrix[7] = 0.0;
				
				mModelview_matrix[8] = Rcw.at<float>(0, 2);
				mModelview_matrix[9] = Rcw.at<float>(1, 2);
				mModelview_matrix[10] = Rcw.at<float>(2, 2);
				mModelview_matrix[11] = 0.0;
				
				mModelview_matrix[12] = tcw.at<float>(0);
				mModelview_matrix[13] = tcw.at<float>(1);
				mModelview_matrix[14] = tcw.at<float>(2);
				mModelview_matrix[15] = 1.0;

				cv::Vec3f plane_mean = SLAM_system_ptr->GetPlaneMean();
				mPlane_mean[0] = plane_mean[0];
				mPlane_mean[1] = plane_mean[1];
				mPlane_mean[2] = plane_mean[2];
				cv::Vec3f plane_normal = SLAM_system_ptr->GetPlaneNormal();
				mPlane_normal[0] = plane_normal[0];
				mPlane_normal[1] = plane_normal[1];
				mPlane_normal[2] = plane_normal[2];
			}


		//imshow("left", frame_rectify_left);
		//imshow("right", frame_rectify_right);
		//waitKey();


		if (checkFinish())
			break;

		//log_file << "after checkfinish()" << endl;
	}

	setFinish();

	SLAM_system_ptr->Shutdown();
}

void SLAM_AR::requestFinish()
{
	unique_lock<mutex> lock(mMutexFinish);
	mbFinishRequested = true;
}

bool SLAM_AR::checkFinish()
{
	unique_lock<mutex> lock(mMutexFinish);
	return mbFinishRequested;
}

void SLAM_AR::setFinish()
{
	unique_lock<mutex> lock(mMutexFinish);
	mbFinished = true;
}

bool SLAM_AR::isFinished()
{
	unique_lock<mutex> lock(mMutexFinish);
	return mbFinished;
}

double* SLAM_AR::get_modelview_matrix()
{
	unique_lock<mutex> lock(mMutex);
	return mModelview_matrix;
}

double* SLAM_AR::get_plane_mean()
{
	unique_lock<mutex> lock(mMutex);
	return mPlane_mean;
}

double* SLAM_AR::get_plane_normal()
{
	unique_lock<mutex> lock(mMutex);
	return mPlane_normal;
}