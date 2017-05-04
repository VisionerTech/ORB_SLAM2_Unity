#pragma once

#ifndef __VIDEOSOURCE_H
#define __VIDEOSOURCE_H

//video input library to open cameras. unity side don't need to input camera index like 0 1.
#define VIDEO_INPUT_LIB 1
#if VIDEO_INPUT_LIB
#endif

#if VIDEO_INPUT_LIB
#include "videoInput.h"
#endif

//IMU data in image
#define IMU_IN_IMAGE 1
#if IMU_IN_IMAGE
#endif

#if IMU_IN_IMAGE
#include "VMG_IMU_Paritycheck\imu_parity_check.h"
#endif

#define OPENCV 1
#if OPENCV
#endif

#if OPENCV
#include <opencv2\opencv.hpp>
#include <opencv2\core\core.hpp>
#include <opencv2\highgui\highgui.hpp>
#include <opencv2\imgproc\imgproc.hpp>

//image size for ar mod rendering
//current PTAM-demo machine camera setup is flipped 90.
#define OPENCV_VIDEO_W 1080
#define OPENCV_VIDEO_H 1080

//image size for tracking, usually smaller for faster frame rate.
#define TRACK_IMAGE_W 540
#define TRACK_IMAGE_H 540

//#define LOCAL_VIDEO
#ifdef LOCAL_VIDEO
//#define FRAME_BY_FRAME
#endif

//camera fliped 90 for current 
//#define CAMERA_FLIP



class VideoSource
{
public:
	//members
#ifdef VIDEO_INPUT_LIB
	videoInput VI;
	int left_index;
	int right_index;
#else
	cv::VideoCapture cap;
	cv::VideoCapture cap_right;
#endif

	cv::Mat src;
	cv::Mat src_right;

	cv::Mat src_flip;
	cv::Mat src_flip_right;

	cv::Mat frame_rectify;
	cv::Mat frame_rectify_right;
	
#if TRACK_IMAGE_W != OPENCV_VIDEO_W
	cv::Mat frame_rectify_down;
	cv::Mat frame_rectify_right_down;
#endif

	cv::Mat mx1, my1, mx2, my2;
	cv::Mat mx1_f, my1_f, mx2_f, my2_f;

	//public functions
	VideoSource();
#ifdef VIDEO_INPUT_LIB
	bool open_camera();
#else
	bool open_webcam(int index_0, int index_1);
#endif
	bool read_calib();
	~VideoSource();
	cv::Mat get_left_rgba();
	cv::Mat get_right_rgba();

#ifdef IMU_IN_IMAGE
	void get_imu(std::vector<float>& imu_vec, std::vector<float>& key_vec);
#endif
};


#endif


#endif