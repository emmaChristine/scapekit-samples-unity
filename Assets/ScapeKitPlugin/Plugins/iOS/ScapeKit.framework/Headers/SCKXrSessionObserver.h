// AUTOGENERATED FILE - DO NOT MODIFY!
// This file generated by Djinni from xr.djinni

#import "SCKXrFrame.h"
#import "SCKXrPlane.h"
#import "SCKXrTrackingState.h"
#import <Foundation/Foundation.h>
@protocol SCKXrSession;


/** (public) */
@protocol SCKXrSessionObserver

- (void)onPlaneDetected:(nullable id<SCKXrSession>)session
                  plane:(nonnull SCKXrPlane *)plane;

- (void)onPlaneUpdated:(nullable id<SCKXrSession>)session
                 plane:(nonnull SCKXrPlane *)plane;

- (void)onPlaneRemoved:(nullable id<SCKXrSession>)session
                 plane:(nonnull SCKXrPlane *)plane;

- (void)onTrackingStateUpdated:(nullable id<SCKXrSession>)session
                         state:(SCKXrTrackingState)state;

- (void)onFrameUpdated:(nullable id<SCKXrSession>)session
                 frame:(nonnull SCKXrFrame *)frame;

@end
