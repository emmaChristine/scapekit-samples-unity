// AUTOGENERATED FILE - DO NOT MODIFY!
// This file generated by Djinni from debug.djinni

#import "SCKLatLng.h"
#import "SCKLogLevel.h"
#import "SCKLogOutput.h"
#import <Foundation/Foundation.h>


/**
 * (public)
 * ScapeKit provides a configurable DebugSession instance - only for Debugging purposes- that 
 * can be used to filter which type of logs: `LogLevel` to print and where: `LogOutput` to print them.
 *
 * In the case of `LogOutput.FILE` a new .log file will be generated for each application launch, in the application's local 
 * storage cache.
 */
@protocol SCKDebugSession

/** Configure which type of logs to print, and where to print them. */
- (void)setLogConfig:(SCKLogLevel)logLevel
           logOutput:(SCKLogOutput)logOutput;

/** Enable/disable the debug overlay. */
- (void)setOverlayVisibility:(BOOL)isVisible;

/** spoof gps coordinates of device */
- (void)mockGPSCoordinates:(double)latitude
                 longitude:(double)longitude;

/** return mocked coordinate, if set */
- (nullable SCKLatLng *)getMockedGPSCoordinates;

@end