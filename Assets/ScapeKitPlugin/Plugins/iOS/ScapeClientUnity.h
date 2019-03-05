//
//  ScapeClientUnity.h
//  Unity-iPhone
//
//  Created by bils
//
//

#import <ScapeKit/ScapeKit.h>
#import <ScapeKit/ScapeKit-Swift.h>

@interface ScapeClientUnity : NSObject<SCKScapeClientObserver>

@property (strong, nonatomic) id<SCKScapeClient> _Nonnull scapeClient;
@property (nonatomic) NSString* apiKey;
@property (nonatomic) BOOL isDebugEnabled;

+ (ScapeClientUnity*_Nullable)sharedInstance;

- (void) withApiKey:(NSString*_Nonnull)apiKey;

- (void) withDebugSupport:(bool)isSupported;

- (void) start;

- (void) stop;

- (bool) isStarted;

- (void) terminate;

- (void)onClientStarted:(id <SCKScapeClient> _Nonnull)scapeClient;

- (void)onClientStopped:(id <SCKScapeClient> _Nonnull)scapeClient;

- (void)onClientFailed:(id <SCKScapeClient> _Nonnull)scapeClient errorMessage:(NSString * _Nonnull)errorMessage;

extern "C"
{
    void _withApiKey(const char*_Nonnull apiKey);

    void _withDebugSupport(bool isSupported);

    void _start();

    void _stop();

    bool _isStarted();

    void _terminate();

    void _log(const char*_Nonnull tag, const char*_Nonnull message);
}

@end
