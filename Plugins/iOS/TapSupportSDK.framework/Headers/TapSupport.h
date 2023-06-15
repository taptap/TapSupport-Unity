//
//  TapSupport.h
//  TapSupportSDK
//
//  Created by Bottle K on 2021/9/14.
//

#import <Foundation/Foundation.h>
#import <TapSupportSDK/TapSupportConfig.h>
NS_ASSUME_NONNULL_BEGIN

#define TapSupportSDK                @"TapSupport"
#define TapSupportSDK_VERSION_NUMBER @"31808001"
#define TapSupportSDK_VERSION        @"3.18.8"

extern NSString *const TAP_SUPPORT_PATH_HOME;
extern NSString *const TAP_SUPPORT_PATH_CATEGORY;
extern NSString *const TAP_SUPPORT_PATH_TICKET_HISTORY;
extern NSString *const TAP_SUPPORT_PATH_TICKET_NEW;

typedef void (^TapSupportLoginHandler)(BOOL succcess, NSError *_Nullable error);

@interface TapSupport : NSObject
@property (nonatomic, strong) TapSupportConfig *config;
@property (nonatomic, strong) NSDictionary *defaultMetaData DEPRECATED_MSG_ATTRIBUTE("use defaultFieldsData instead");
@property (nonatomic, strong) NSDictionary *defaultFieldsData;
@property (nonatomic, strong, nullable) NSString *anonymousId;

+ (TapSupport *)shareInstance;

/// 更新 defaultField 中某一个值
/// @param value value
/// @param key key
+ (void)updateDefaultFieldWithValue:(nullable id)value forKey:(NSString *)key;

/// 匿名用户登录
/// @param anonymousId 匿名用户id，不能为空
+ (void)loginAnonymously:(nonnull NSString *)anonymousId;

/// 自定义登录
/// @param credential 凭据
+ (void)loginWithCustomeCredential:(nonnull NSString *)credential;

/// TDSUser 登录
/// @param credential 凭据
/// @param handler 回调
+ (void)loginWithTDSCredential:(nonnull NSString *)credential handler:(TapSupportLoginHandler)handler;

/// 退出登录
+ (void)logout;

/// 获取网页地址
+ (NSString *)getSupportWebUrl;

/// 获取网页地址
/// @param path 路径
+ (NSString *)getSupportWebUrl:(nullable NSString *)path;

/// 获取网页地址
/// @param path 路径
/// @param fieldsData fields 参数
+ (NSString *)getSupportWebUrl:(nullable NSString *)path
                    fieldsData:(nullable NSDictionary *)fieldsData;

/// 获取网页地址
/// @param path 路径
/// @param metaData meta 参数
/// @param fieldsData fields 参数
+ (NSString *)getSupportWebUrl:(nullable NSString *)path
                      metaData:(nullable NSDictionary *)metaData
                    fieldsData:(nullable NSDictionary *)fieldsData
    __attribute__((deprecated("Please use [TDSHandleUrl handleOpenURL:]")));

/// 打开客服网页
+ (void)openSupportView;

/// 打开客服网页
/// @param path 路径
+ (void)openSupportViewWithPath:(nullable NSString *)path;

/// 打开客服网页
/// @param path 路径
/// @param fieldsData fields 参数
+ (void)openSupportViewWithPath:(nullable NSString *)path
                     fieldsData:(nullable NSDictionary *)fieldsData;

/// 打开客服网页
/// @param path 路径
/// @param metaData meta 参数
/// @param fieldsData fields 参数
+ (void)openSupportViewWithPath:(nullable NSString *)path
                       metaData:(nullable NSDictionary *)metaData
                     fieldsData:(nullable NSDictionary *)fieldsData
    __attribute__((deprecated("Please use [TDSHandleUrl handleOpenURL:]")));

+ (void)openSupportViewWithFullUrl:(NSString *)url;

/// 关闭客服网页
+ (void)closeSupportView;

/// 开始轮询获取未读状态
+ (void)resume;

/// 结束轮询获取未读状态
+ (void)pause;

/// 单次获取未读状态
+ (void)fetchUnreadStatus;

@end

NS_ASSUME_NONNULL_END
