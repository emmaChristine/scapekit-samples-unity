// AUTOGENERATED FILE - DO NOT MODIFY!
// This file generated by Djinni from network.djinni

#import <Foundation/Foundation.h>

/** internal - do not use */
@interface SCKNetworkRequest : NSObject
- (nonnull instancetype)initWithBaseURL:(nonnull NSString *)baseURL
                                 apiKey:(nonnull NSString *)apiKey
                             methodType:(nonnull NSString *)methodType
                               username:(nonnull NSString *)username
                               password:(nonnull NSString *)password
                                 params:(nonnull NSDictionary<NSString *, NSString *> *)params;
+ (nonnull instancetype)networkRequestWithBaseURL:(nonnull NSString *)baseURL
                                           apiKey:(nonnull NSString *)apiKey
                                       methodType:(nonnull NSString *)methodType
                                         username:(nonnull NSString *)username
                                         password:(nonnull NSString *)password
                                           params:(nonnull NSDictionary<NSString *, NSString *> *)params;

@property (nonatomic, readonly, nonnull) NSString * baseURL;

@property (nonatomic, readonly, nonnull) NSString * apiKey;

@property (nonatomic, readonly, nonnull) NSString * methodType;

@property (nonatomic, readonly, nonnull) NSString * username;

@property (nonatomic, readonly, nonnull) NSString * password;

@property (nonatomic, readonly, nonnull) NSDictionary<NSString *, NSString *> * params;

@end
