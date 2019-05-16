//
//  ScapeClientUnity.mm
//  Unity-iPhone
//
//  Created by bils on 09/04/2018.
//

#import "ScapeSessionUnity.h"
#import "ScapeClientUnity.h"
#import "NSObject+ObjectMap.h"

#include <iostream>
#include <string>

static ScapeSessionUnity* _ScapeSessionUnity;

@implementation ScapeSessionUnity

+ (ScapeSessionUnity*)sharedInstance
{
    if(!_ScapeSessionUnity)
    {
        _ScapeSessionUnity = [[ScapeSessionUnity alloc] init];
    }
    return _ScapeSessionUnity;
}

- (void)getMeasurements
{   
    SCKScapeSession * scapeSession = [[[ScapeClientUnity sharedInstance] scapeClient] scapeSession];
    [scapeSession getMeasurements:_ScapeSessionUnity];
}

- (void)setARFrame:(ARFrame *)frame
{
    SCKScapeSession * scapeSession = [[[ScapeClientUnity sharedInstance] scapeClient] scapeSession];
    [scapeSession setARFrame:frame];
}

- (void) startScapeFetch
{
    SCKScapeSession * scapeSession = [[[ScapeClientUnity sharedInstance] scapeClient] scapeSession];
    [scapeSession startFetch:_ScapeSessionUnity];
}

- (void) stopScapeFetch
{
    SCKScapeSession * scapeSession = [[[ScapeClientUnity sharedInstance] scapeClient] scapeSession];
    [scapeSession stopFetch];
}

- (void) onScapeMeasurementsRequested:(nullable SCKScapeSession *)session
                           timestamp:(double)timestamp;
{
  (void)session;

    dispatch_async(dispatch_get_global_queue( DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), ^(void){
        
        NSString* tsStr = [[NSNumber numberWithDouble:timestamp] stringValue];
        
        UnitySendMessage("ScapeSession", "OnScapeMeasurementsRequested", [tsStr UTF8String]);  
    });  
}


- (void)onScapeSessionError:(nullable SCKScapeSession *)session
                      state:(SCKScapeSessionState)state
                    message:(nonnull NSString *)message;
{
    (void)session;

    dispatch_async(dispatch_get_global_queue( DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), ^(void){

        NSString* jsonError = [NSString stringWithFormat:@"{\n\t\"state\":%d,\n\t\"message\":\"%@\"}", (int)state, message];
        
        UnitySendMessage("ScapeSession", "OnScapeSessionError", [jsonError UTF8String]);    
    });

}

- (void)onDeviceLocationMeasurementsUpdated:(nullable SCKScapeSession *)session
                               measurements:(nullable SCKLocationMeasurements *)measurements;
{
    dispatch_async(dispatch_get_global_queue( DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), ^(void){
        NSData *jsonData = [measurements JSONData];
        const char * locationMeasurementsJsonStr = [[[NSString alloc] initWithData:jsonData  encoding:NSUTF8StringEncoding] UTF8String];
        
        UnitySendMessage("ScapeSession", "OnDeviceLocationMeasurementsUpdated", locationMeasurementsJsonStr);
    });
}

- (void)onDeviceMotionMeasurementsUpdated:(nullable SCKScapeSession *)session
                             measurements:(nullable SCKMotionMeasurements *)measurements;
{
    dispatch_async(dispatch_get_global_queue( DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), ^(void){
        NSData *jsonData = [measurements JSONData];
        const char * motionMeasurementsJsonStr = [[[NSString alloc] initWithData:jsonData  encoding:NSUTF8StringEncoding] UTF8String];
        
        UnitySendMessage("ScapeSession", "OnDeviceMotionMeasurementsUpdated", motionMeasurementsJsonStr);
    });  
}

- (void)onScapeMeasurementsUpdated:(nullable SCKScapeSession *)session
                      measurements:(nullable SCKScapeMeasurements *)measurements;
{
    dispatch_async(dispatch_get_global_queue( DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), ^(void){
        NSData *jsonData = [measurements JSONData];
        const char * scapeMeasurementsJsonStr = [[[NSString alloc] initWithData:jsonData  encoding:NSUTF8StringEncoding] UTF8String];
        
        UnitySendMessage("ScapeSession", "OnScapeMeasurementsUpdated", scapeMeasurementsJsonStr);
    });  
}

- (void)onCameraTransformUpdated:(nullable SCKScapeSession *)session
                       transform:(nullable NSArray<NSNumber *> *)transform;
{
    dispatch_async(dispatch_get_global_queue( DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), ^(void){
        NSData *jsonData = [transform JSONData];
        const char * transformJsonStr = [[[NSString alloc] initWithData:jsonData  encoding:NSUTF8StringEncoding] UTF8String];
        
        UnitySendMessage("ScapeSession", "OnCameraTransformUpdated", transformJsonStr);
    });  
}

@end

void _getMeasurements() 
{
    [SCKLog log:SCKLogLevelLogInfo tag:@"SCKScapeSessionUnity" msg:@" C _getMeasurements"];
    
    [[ScapeSessionUnity sharedInstance] getMeasurements];
}

