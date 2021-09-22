## TapSupport

## 命名空间

```c#
using TapTap.Support;
```

### 初始化

工单系统是厂商维度的，游戏在接入工单系统时需要绑定一个工单域名。

serverUrl 为用户初始化时配置的工单域名

rootCategoryID 也是用户在初始化时可选指定的，默认值为 `-`，这个参数的意义参见 https://github.com/leancloud/ticket/wiki/Ticket-in-app-page-API#baseurl

callback 用于回调当前是否有未读的工单

```c#
TapSupport.Init("serverUrl","rootCategoryID",new TapSupportCallback
{
       UnReadStatusChanged = (hasUnRead, exception) =>
       {
          Debug.Log($"hasUnRead:{hasUnRead} exception:{exception}");
       }
});
```

### 登录

> 当前版本仅支持匿名登录

游戏方可以自行维护匿名登录的 UUID ，如果传入的 UUID 为空，`TapSupport` 则会生成一个 UUID 并且写入 `Application.persistentDataPath` 中。

```c#
TapSupport.AnonymousLogin(string uuid);
```

### 生成工单 Url

工单的 Url 的构成是这样的，小括号代表可选:

```url
https://{domain}/in-app/v1/categories/{rootCategoryID}{path}#{authInfo}(&metaData)(&fieldsData)
```

`TapSupport` 提供几个 API 用于帮忙游戏方生成工单 Url 以及设置默认的 field 和 metaData

```c#
// 设置默认的 fieldData 以及 metaData
TapSupport.SetDefaultFieldsData(Dictionary<string,object> filedsData);
TapSupport.SetDefaultMetaData(Dictionary<string,object> metaData);

// 生成工单
TapSupport.GetSupportWebUrl(string path, Dictionary<string, object> metaData,Dictionary<string, object> fieldsData)
```

### 未读消息通知

游戏方可以通过 `TapSupport` 中的  `Resume` 以及 `Pause` 方法来进行轮询获取当前工单是否有未读消息。

> TapSupport 通过 Timer 来进行轮询

#### 开始轮询

`TapSupport` 在登录之后，或开始轮询获取当前是否有未读的工单，并且通过 `TapSupportCallback` 回调给开发者。

当前的轮询策略为 10s 一次，如果没有未读消息则增减间隔时间为 20s ，每次递增的间隔时间为 10s，直到最大时间 300s，每次重新调用 `Resume` 方法时，会重置轮询时间。

```c#
TapSupport.Resume();
```

#### 结束轮询

结束轮询时，会重制当前轮询时间。

```c#
TapSupport.Pause();
```

