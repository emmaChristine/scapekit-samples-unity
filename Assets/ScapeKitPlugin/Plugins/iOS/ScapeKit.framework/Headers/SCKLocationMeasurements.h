// AUTOGENERATED FILE - DO NOT MODIFY!
// This file generated by Djinni from location.djinni

#import "SCKLatLng.h"
#import <Foundation/Foundation.h>

/**
 * (public)
 * The GPS, compass and barotemeter measurements
 */
@interface SCKLocationMeasurements : NSObject
- (nonnull instancetype)initWithTimestamp:(nullable NSNumber *)timestamp
                                   latLng:(nullable SCKLatLng *)latLng
                      coordinatesAccuracy:(nullable NSNumber *)coordinatesAccuracy
                                 altitude:(nullable NSNumber *)altitude
                         altitudeAccuracy:(nullable NSNumber *)altitudeAccuracy
                                  heading:(nullable NSNumber *)heading
                          headingAccuracy:(nullable NSNumber *)headingAccuracy
                                   course:(nullable NSNumber *)course
                                    speed:(nullable NSNumber *)speed;
+ (nonnull instancetype)locationMeasurementsWithTimestamp:(nullable NSNumber *)timestamp
                                                   latLng:(nullable SCKLatLng *)latLng
                                      coordinatesAccuracy:(nullable NSNumber *)coordinatesAccuracy
                                                 altitude:(nullable NSNumber *)altitude
                                         altitudeAccuracy:(nullable NSNumber *)altitudeAccuracy
                                                  heading:(nullable NSNumber *)heading
                                          headingAccuracy:(nullable NSNumber *)headingAccuracy
                                                   course:(nullable NSNumber *)course
                                                    speed:(nullable NSNumber *)speed;

/** The time at which this location was determined */
@property (nonatomic, readonly, nullable) NSNumber * timestamp;

/** Geographical lat_lng */
@property (nonatomic, readonly, nullable) SCKLatLng * latLng;

/** The radius of uncertainty for the location, measured in meters */
@property (nonatomic, readonly, nullable) NSNumber * coordinatesAccuracy;

/** altitude */
@property (nonatomic, readonly, nullable) NSNumber * altitude;

/** Altitude accuracy */
@property (nonatomic, readonly, nullable) NSNumber * altitudeAccuracy;

/** heading representing the direction of the north */
@property (nonatomic, readonly, nullable) NSNumber * heading;

/** Heading accuracy */
@property (nonatomic, readonly, nullable) NSNumber * headingAccuracy;

/** Course */
@property (nonatomic, readonly, nullable) NSNumber * course;

/** Speed */
@property (nonatomic, readonly, nullable) NSNumber * speed;

@end
