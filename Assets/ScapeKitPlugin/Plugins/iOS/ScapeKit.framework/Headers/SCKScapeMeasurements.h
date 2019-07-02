// AUTOGENERATED FILE - DO NOT MODIFY!
// This file generated by Djinni from scape.djinni

#import "SCKLatLng.h"
#import "SCKScapeMeasurementsStatus.h"
#import "SCKScapeOrientation.h"
#import <Foundation/Foundation.h>

/**
 * (public) -
 * The Scape Vision Engine measurements
 */
@interface SCKScapeMeasurements : NSObject
- (nonnull instancetype)initWithTimestamp:(nullable NSNumber *)timestamp
                                   latLng:(nullable SCKLatLng *)latLng
                                  heading:(nullable NSNumber *)heading
                              orientation:(nullable SCKScapeOrientation *)orientation
                        rawHeightEstimate:(nullable NSNumber *)rawHeightEstimate
                          confidenceScore:(nullable NSNumber *)confidenceScore
                       measurementsStatus:(SCKScapeMeasurementsStatus)measurementsStatus;
+ (nonnull instancetype)scapeMeasurementsWithTimestamp:(nullable NSNumber *)timestamp
                                                latLng:(nullable SCKLatLng *)latLng
                                               heading:(nullable NSNumber *)heading
                                           orientation:(nullable SCKScapeOrientation *)orientation
                                     rawHeightEstimate:(nullable NSNumber *)rawHeightEstimate
                                       confidenceScore:(nullable NSNumber *)confidenceScore
                                    measurementsStatus:(SCKScapeMeasurementsStatus)measurementsStatus;

/** The time at which these scape measurements were determined */
@property (nonatomic, readonly, nullable) NSNumber * timestamp;

/** Current camera-based device's coordinates computed from the camera's device */
@property (nonatomic, readonly, nullable) SCKLatLng * latLng;

/** Current heading computed from the camera's device */
@property (nonatomic, readonly, nullable) NSNumber * heading;

/** Current orientation computed from the camera's device */
@property (nonatomic, readonly, nullable) SCKScapeOrientation * orientation;

/** Current height estimate computed from the camera's device: can be used to estimate where the ground is located */
@property (nonatomic, readonly, nullable) NSNumber * rawHeightEstimate;

/** Score from 1 to 5 indicating how likely the current area is recognized by Scape Vision Engine */
@property (nonatomic, readonly, nullable) NSNumber * confidenceScore;

/** Current status */
@property (nonatomic, readonly) SCKScapeMeasurementsStatus measurementsStatus;

@end
