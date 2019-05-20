//
//  ScapeClientUnity.h
//  Unity-iPhone
//
//  Created by bils on 09/04/2018.
//

#import <ScapeKit/ScapeKit.h>
#import <ScapeKit/ScapeKit-Swift.h>

@interface ScapeSessionUnity : NSObject<SCKScapeSessionObserver>

+ (ScapeSessionUnity*_Nullable)sharedInstance;

- (void) startScapeFetch;

- (void) stopScapeFetch;

- (void) getMeasurements;

- (void) setARFrame:(ARFrame *)frame;

- (void) onScapeMeasurementsRequested:(nullable SCKScapeSession *)session
                           timestamp:(double)timestamp;

- (void) onScapeSessionError:(nullable SCKScapeSession *)session
                      			state:(SCKScapeSessionState)state
                    			message:(nonnull NSString *)message;

- (void) onDeviceLocationMeasurementsUpdated:(nullable SCKScapeSession *)session
                               	measurements:(nullable SCKLocationMeasurements *)measurements;

- (void) onDeviceMotionMeasurementsUpdated:(nullable SCKScapeSession *)session
                             	measurements:(nullable SCKMotionMeasurements *)measurements;

- (void) onScapeMeasurementsUpdated:(nullable SCKScapeSession *)session
                      			measurements:(nullable SCKScapeMeasurements *)measurements;

- (void) onCameraTransformUpdated:(nullable SCKScapeSession *)session
                       			transform:(nullable NSArray<NSNumber *> *)transform;


@end

extern "C"
{
    void _getMeasurements();
}
