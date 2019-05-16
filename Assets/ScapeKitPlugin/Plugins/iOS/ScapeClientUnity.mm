//
//  ScapeClientUnity.mm
//  Unity-iPhone
//
//  Created by bils
//
//

#import "ScapeClientUnity.h"

#include "IUnityGraphics.h"
#include "IUnityInterface.h"

#include <iostream>

UIViewController* UnityGetGLViewController();
UIWindow* UnityGetMainWindow();

static ScapeClientUnity* _ScapeClientUnity;

@implementation ScapeClientUnity

+ (ScapeClientUnity*)sharedInstance
{
    if(!_ScapeClientUnity)
    {
        _ScapeClientUnity = [[ScapeClientUnity alloc] init];
        [SCKLog log:SCKLogLevelLogInfo tag:@"SCKScapeClientUnity" msg:@"Create SCKScapeClientUnity client"];
    }
    return _ScapeClientUnity;
}

- (void) withApiKey:(NSString*_Nonnull)apiKey
{
    _ScapeClientUnity.apiKey = apiKey;
}

- (void) withDebugSupport:(bool)isSupported
{
    _ScapeClientUnity.isDebugEnabled = isSupported;
}

- (void) start
{
    [SCKLog log:SCKLogLevelLogInfo tag:@"SCKScapeClientUnity" msg:@"Start SCKScapeClientUnity client"];

    if(!_ScapeClientUnity.apiKey.length) {
        [SCKLog log:SCKLogLevelLogInfo tag:@"SCKScapeClientUnity" msg:@"Cannot start SCKScapeClientUnity client, reason: apiKey is empty"];
        return;
    }

    _ScapeClientUnity.scapeClient = [[[[SCKScape scapeClientBuilder] withApiKey:_ScapeClientUnity.apiKey] withDebugSupport:_ScapeClientUnity.isDebugEnabled] build];
    [_ScapeClientUnity.scapeClient startWithObserver:self];
}

- (void) stop
{
    [SCKLog log:SCKLogLevelLogInfo tag:@"SCKScapeClientUnity" msg:@"Stop SCKScapeClientUnity client"];

    [_ScapeClientUnity.scapeClient stopWithObserver:self];
}

- (bool) isStarted
{
    return [_ScapeClientUnity.scapeClient isStarted];
}

- (void) terminate
{
    [_ScapeClientUnity.scapeClient terminateWithObserver:self];
}

- (void)onClientStarted:(id <SCKScapeClient> _Nonnull)scapeClient
{
    (void)scapeClient;
    UnitySendMessage("ScapeClient", "OnClientStarted", "ignore" );
}

- (void)onClientStopped:(id <SCKScapeClient> _Nonnull)scapeClient
{
    (void)scapeClient;
    UnitySendMessage("ScapeClient", "OnClientStopped", "ignore" );
}

- (void)onClientFailed:(id <SCKScapeClient> _Nonnull)scapeClient errorMessage:(NSString * _Nonnull)errorMessage
{
    (void)scapeClient;
    const char *errorCtr = [errorMessage UTF8String];
    UnitySendMessage("ScapeClient", "OnClientFailed", errorCtr);
}

extern "C" {
    void _withApiKey(const char* apiKey)
    {
        std::cout << __func__ << "/n";
        NSString* apiKeyStr = [NSString stringWithUTF8String:apiKey];
        [[ScapeClientUnity sharedInstance] withApiKey:apiKeyStr];
    }

    void _withDebugSupport(bool isDebugSupported)
    {
        [[ScapeClientUnity sharedInstance] withDebugSupport:isDebugSupported];
    }

    void _start()
    {
        std::cout << __func__ << "/n";
        [[ScapeClientUnity sharedInstance] start];
    }

    void _stop()
    {
        std::cout << __func__ << "/n";
        [[ScapeClientUnity sharedInstance] stop];
    }

    bool _isStarted()
    {
        std::cout << __func__ << "/n";
        return [[ScapeClientUnity sharedInstance] isStarted];
    }

    void _terminate()
    {
        std::cout << __func__ << "/n";
        [[ScapeClientUnity sharedInstance] terminate];
    }
}


@end
