
#include "ScapeKit/SCKDebugSession.h"
#import "ScapeClientUnity.h"

extern "C" {

void _setLogConfig(int level, int output)
{
	id <SCKDebugSession> debugSession = [[[ScapeClientUnity sharedInstance] scapeClient] debugSession];
	if(debugSession != nullptr) {
        [debugSession setLogConfig:SCKLogLevel(level) logOutput:SCKLogOutput(output)];
	}
    else {
    	[SCKLog log:SCKLogLevelLogError tag:@"SCKDebugSession" msg:@"_setDebugConfig no debug session found, did you run client with debug support enabled?"];
    }
}

void _mockGPSCoordinates(double latitude, double longitude)
{
    id <SCKDebugSession> debugSession = [[[ScapeClientUnity sharedInstance] scapeClient] debugSession];
    if(debugSession != nullptr) {
        [debugSession mockGPSCoordinates:latitude longitude:longitude];
    }
    else {
        [SCKLog log:SCKLogLevelLogError tag:@"SCKDebugSession" msg:@"_mockGPSCoordinates no debug session found, did you run client with debug support enabled?"];
    }

}
void _log(int level, const char* tag, const char* message) 
{
    NSString* tagStr = [NSString stringWithUTF8String:tag];
    NSString* messageStr = [NSString stringWithUTF8String:message];
    [SCKLog log:(SCKLogLevel)level tag:tagStr msg:messageStr];
}

void _saveImages(bool save)
{
    id <SCKDebugSession> debugSession = [[[ScapeClientUnity sharedInstance] scapeClient] debugSession];
    if(debugSession != nullptr) {
        [debugSession saveImages:save];
    }
    else {
        [SCKLog log:SCKLogLevelLogError tag:@"SCKDebugSession" msg:@"_saveImages no debug session found, did you run client with debug support enabled?"];
    }
}

}

