// AUTOGENERATED FILE - DO NOT MODIFY!
// This file generated by Djinni from scape.djinni

#import <Foundation/Foundation.h>
@protocol SCKScapeSessionObserver;
@protocol SCKVideoFrame;


/**
 * (public)
 * ScapeKit's main service: gets measurements from the raw device's built-in sensors as well as Scape Vision Engine
 */
@interface SCKScapeSession : NSObject

/**
 * (public)
 * Start measurements fetching at continuous interval
 * Combine device's sensors-based measurements (gyroscope, accelerometer, gps, magnetometer)  
 * with Scape Vision Engine to get hyper-accurate measurements 
 */
- (void)startFetch:(nullable id<SCKScapeSessionObserver>)observer;

/**
 * (public)
 * Stop measurements fetching at continuous interval
 */
- (void)stopFetch;

/**
 * (public)
 * Get the measurements 
 * Combine device's sensors-based measurements (gyroscope, accelerometer, gps, magnetometer)  
 * with Scape Vision Engine to get hyper-accurate measurements
 */
- (void)getMeasurements:(nullable id<SCKScapeSessionObserver>)observer;

/**
 * (public)
 * Set the video frame manually
 */
- (void)setRawVideoFrame:(nullable id<SCKVideoFrame>)frame;

/**
 * (public)
 * Set the Y channel manually
 */
- (void)setYChannelPtr:(int64_t)pointer
                 width:(int32_t)width
                height:(int32_t)height;

/**
 * (public)
 * Set the camera intrinsics manually
 */
- (void)setCameraIntrinsics:(double)xFocalLength
               yFocalLength:(double)yFocalLength
            xPrincipalPoint:(double)xPrincipalPoint
            yPrincipalPoint:(double)yPrincipalPoint;

/**
 * (public)
 * Set camera transform manually
 */
- (void)setCameraTransform:(nonnull NSArray<NSNumber *> *)transform;

@end
