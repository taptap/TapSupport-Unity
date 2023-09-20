//
//  TapSupportConfig.h
//  TapSupportSDK
//
//  Created by Bottle K on 2021/9/14.
//

#import <Foundation/Foundation.h>
#import <TapSupportSDK/TapSupportDelegate.h>

NS_ASSUME_NONNULL_BEGIN

@interface TapSupportConfig : NSObject
@property (nonatomic, copy) NSString *server;
@property (nonatomic, copy) NSString *productID;
@property (nonatomic, weak) id<TapSupportDelegate> callback;
@end

NS_ASSUME_NONNULL_END
