//
//  TapSupportDelegate.h
//  TapSupportSDK
//
//  Created by Bottle K on 2021/9/14.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@protocol TapSupportDelegate <NSObject>

- (void)onGetUnreadStatusError:(NSError *)error;

- (void)onUnreadStatusChanged:(bool)hasUnread;
@end

NS_ASSUME_NONNULL_END
