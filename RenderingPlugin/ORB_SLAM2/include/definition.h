//# defines, configures,

//video source size, original is set 640*480
#define OPENCV_VIDEO_W 540
#define OPENCV_VIDEO_H 540

//use local video source
#define LOCAL_VIDEO

//one click one frame
#define CLICK_FRAME

//ransac planar detection of the first scene
#define PLANAR_DETECT
#ifdef PLANAR_DETECT
//draw the coordinate estimated from RANSAC PCA of the first scene
//#define DRAW_COORD
#endif

//debuging print out
//#define DEBUG_PRINT

//debuging print to log file
//#define DEBUG_LOG

//returning the world to camera matrix align with mean and normal.
//#define RETURN_ALIGNED_W2C

//use pangolin as the map viewer, if so unity plugin would have compiled error, confliction may happen.
//#define PANGOLIN_VIEWER