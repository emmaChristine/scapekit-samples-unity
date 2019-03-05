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
        [SCKLog logdebug:@"SCKScapeClientUnity" msg:@"Create SCKScapeClientUnity client"];
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
    [SCKLog logdebug:@"SCKScapeClientUnity" msg:@"Start SCKScapeClientUnity client"];

    if(!_ScapeClientUnity.apiKey.length) {
        [SCKLog logdebug:@"SCKScapeClientUnity" msg:@"Cannot start SCKScapeClientUnity client, reason: apiKey is empty"];
        return;
    }

    _ScapeClientUnity.scapeClient = [[[[SCKScape scapeClientBuilder] withApiKey:_ScapeClientUnity.apiKey] withDebugSupport:_ScapeClientUnity.isDebugEnabled] build];
    [_ScapeClientUnity.scapeClient setScapeClientObserver:self];
    [_ScapeClientUnity.scapeClient start];
}

- (void) stop
{
    [SCKLog logdebug:@"SCKScapeClientUnity" msg:@"Stop SCKScapeClientUnity client"];

    [_ScapeClientUnity.scapeClient stop];
}

- (bool) isStarted
{
    return [_ScapeClientUnity.scapeClient isStarted];
}

- (void) terminate
{
    [_ScapeClientUnity.scapeClient terminate];
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

    void _log(const char* tag, const char* message) 
    {
        std::cout << __func__ << "/n";
        NSString* tagStr = [NSString stringWithUTF8String:tag];
        NSString* messageStr = [NSString stringWithUTF8String:message];
        [SCKLog logdebug:tagStr msg:messageStr];
    }
}


@end
