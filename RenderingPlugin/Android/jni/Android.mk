LOCAL_PATH := $(call my-dir)

include $(CLEAR_VARS)

LOCAL_MODULE := RenderingPlugin
LOCAL_SRC_FILES := ../../../RenderingPlugin/RenderingPlugin.cpp
LOCAL_LDLIBS := -llog -lGLESv2
LOCAL_ARM_MODE := arm
LOCAL_CFLAGS := -DUNITY_ANDROID

include $(BUILD_SHARED_LIBRARY)
