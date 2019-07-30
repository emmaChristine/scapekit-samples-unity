//
//  video_frame_imp.h
//  lapse_app
//
//  Created by dyno on 7/25/16.
//  Copyright © 2016 dyno. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>
#import "SCKVideoFrame.h"

@interface DefaultVideoFrame : NSObject<SCKVideoFrame>;
@property int64_t m_data;
@property int64_t m_size;
@property int32_t m_width;
@property int32_t m_height;
@property double m_timestamp;

- (instancetype _Nullable ) initWithImage:(CVPixelBufferRef _Nonnull)ios_image;

- (BOOL)parserSample;

- (int64_t)getData;

- (int32_t)getWidth;

- (int32_t)getHeight;

- (int64_t)getSize;

- (double)getTimestamp;

- (void)setData:(int64_t)data;

- (void)setWidth:(int32_t)width;

- (void)setHeight:(int32_t)height;

- (void)setSize:(int64_t)size;

- (void)setTimestamp:(double)timestamp;

@end
