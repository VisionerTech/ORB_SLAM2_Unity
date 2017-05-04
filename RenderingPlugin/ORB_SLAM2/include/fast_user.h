#ifndef FAST_USER_H
#define FAST_USER_H

#include <vector>
#include <list>
#include <opencv/cv.h>

namespace cv
{

	void FAST_user(InputArray _img, std::vector<KeyPoint>& keypoints, int threshold, bool nonmax_suppression, uchar threshold_tab[512], uchar* _buf);

}


#endif