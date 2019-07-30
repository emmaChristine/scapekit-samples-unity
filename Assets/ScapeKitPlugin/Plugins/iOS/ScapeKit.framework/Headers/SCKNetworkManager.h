// AUTOGENERATED FILE - DO NOT MODIFY!
// This file generated by Djinni from network.djinni

#import "SCKNetworkBody.h"
#import "SCKNetworkHeaders.h"
#import "SCKNetworkRequest.h"
#import <Foundation/Foundation.h>
@class SCKNetworkManager;
@protocol SCKNetworkListener;


/** internal - do not use */
@interface SCKNetworkManager : NSObject

+ (nullable SCKNetworkManager *)create;

- (void)traceRequest:(BOOL)enable;

- (void)request:(nonnull SCKNetworkRequest *)request;

- (void)cancel;

- (void)get:(nonnull SCKNetworkHeaders *)headers
   listener:(nullable id<SCKNetworkListener>)listener;

- (void)post:(nonnull SCKNetworkBody *)body
     headers:(nonnull SCKNetworkHeaders *)headers
    listener:(nullable id<SCKNetworkListener>)listener;

- (void)patch:(nonnull SCKNetworkBody *)body
      headers:(nonnull SCKNetworkHeaders *)headers
     listener:(nullable id<SCKNetworkListener>)listener;

- (void)put:(nonnull SCKNetworkBody *)body
    headers:(nonnull SCKNetworkHeaders *)headers
   listener:(nullable id<SCKNetworkListener>)listener;

- (void)del:(nonnull SCKNetworkHeaders *)headers
   listener:(nullable id<SCKNetworkListener>)listener;

- (void)setTimeout:(int32_t)timeout;

@end
