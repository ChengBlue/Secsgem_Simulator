# SECS/GEM Simulator 项目说明文档

## 一、项目概述

**SECS/GEM Simulator** 是一款基于 .NET 8 与 Windows Forms 的 **SECS/GEM（半导体设备通信标准）模拟器**。用于模拟设备端（Equipment）与主机（Host）之间的 HSMS（High-Speed SECS Message Services）通信，便于开发、调试与联调 SECS/GEM 应用。

### 主要功能

- **HSMS 连接**：支持 **Active（客户端）** 与 **Passive（服务端）** 两种模式
- **SECS-II 消息收发**：自定义 Stream/Function，支持 SML 格式编写消息体
- **SECS-II 自动回复**：收到带 W-bit 的请求时，自动回复对应 secondary；如 S1F1→S1F2、S1F13→S1F14、S2F41→S2F42、S5F1→S5F2、S1F15→S1F16 等
- **预置模板**：S1F1（Are You There）、S1F13（Establish Communication）、S2F41（Host Command）、S5F1（Alarm Report）等
- **通信日志**：实时显示 TX/RX、错误与详细信息，可选十六进制明细
- **配置持久化**：IP、端口、DeviceID、Active/Passive 等保存至 `config.json`

---

## 二、技术栈与结构

| 项目 | 说明                              |
| ---- | --------------------------------- |
| 框架 | .NET 8.0、Windows Forms           |
| 语言 | C#                                |
| 配置 | `System.Text.Json`，`config.json` |

### 项目结构

```
Secsgem_Simulator/
├── config.json                 # 运行期配置（IP、端口、超时等）
├── SecsGemSimulator.sln
├── SecsGemSimulator/
│   ├── Program.cs              # 程序入口
│   ├── MainForm.cs             # 主窗体逻辑
│   ├── MainForm.Designer.cs    # 主窗体 UI
│   ├── Config/
│   │   └── AppConfig.cs        # 配置加载/保存
│   ├── Core/
│   │   ├── HsmsConnection.cs   # HSMS TCP 连接、收发、Select/Linktest
│   │   ├── SecsAutoReply.cs   # SECS-II 自动回复（S1F1→S1F2、S1F13→S1F14 等）
│   │   ├── SecsMessage.cs      # SECS 消息编解码（含 4 字节长度头）
│   │   ├── SecsItem.cs         # SECS 数据项（L/A/B/U1-U4/I1-I4 等）
│   │   └── SmlParser.cs        # SML 解析器
│   └── Helpers/
│       └── LogHelper.cs        # 日志事件与级别
└── README.md                   # 本说明文档
```

---

## 三、配置说明

`config.json` 位于程序运行目录（如 `bin/Debug/net8.0-windows/`），若不存在则使用内置默认值。

| 配置项             | 类型   | 默认值        | 说明                                                         |
| ------------------ | ------ | ------------- | ------------------------------------------------------------ |
| `IpAddress`        | string | `"127.0.0.1"` | 目标 IP（Active）或监听地址（Passive，`0.0.0.0` 表示所有网卡） |
| `Port`             | int    | `5000`        | 端口                                                         |
| `IsActiveMode`     | bool   | `true`        | `true`：Active（客户端）；`false`：Passive（服务端）         |
| `DeviceId`         | ushort | `0`           | 设备/会话 ID（HSMS 会话标识）                                |
| `T3Timeout`        | int    | `45`          | T3 超时（秒）                                                |
| `T5Timeout`        | int    | `10`          | T5 超时（秒）                                                |
| `T6Timeout`        | int    | `5`           | T6 超时（秒）                                                |
| `T7Timeout`        | int    | `10`          | T7 超时（秒）                                                |
| `LinkTestInterval` | int    | `60`          | 链路检测间隔（秒）                                           |

*注：当前版本尚未使用 T3/T5/T6/T7 与 LinkTestInterval 做超时与周期检测，仅作配置预留。*

---

## 四、使用说明

### 4.1 编译与运行

1. 安装 [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)。

2. 在项目根目录执行：

   ```bash
   dotnet build SecsGemSimulator.sln
   dotnet run --project SecsGemSimulator
   ```

   或使用 Visual Studio 2022 打开 `SecsGemSimulator.sln` 并运行。

### 4.2 连接与收发

1. **Network Configuration**  
   - 填写 **IP**、**Port**、**DeviceID**。  
   - 勾选 **Active Mode**：本机作为客户端连接对方；不勾选：本机作为服务端等待对方连接。  
   - 点击 **Start/Connect** 建立连接；连接后按钮变为 **Stop**，点击可断开。  
   - **Save Config** 将当前网络配置写入 `config.json`。

2. **Send Message**  
   - **Stream** / **Function**：输入 SxFy 的 x、y（如 S1F1 → 1、1）。  
   - **Wait Bit**：勾选表示该消息需要对方回复（W-bit）。  
   - **Template**：选择预设模板可自动填充 SxFy 与 Body；选 **Custom** 可完全自定义。  
   - **Body (SML-like)**：按 SML 语法编写消息体；可留空（如 S1F1）。  
   - 点击 **Send Message** 发送。未连接时会提示 “Not Connected!”。

3. **Communication Log**  
   - 显示 `[TX]` / `[RX]` / `[INF]` / `[WRN]` / `[ERR]` 等日志。  
   - 勾选 **Show Hex Details** 可查看报文十六进制。  
   - **Clear** 清空日志。

### 4.3 SECS-II 自动回复

本模拟器在**收到**带 **Wait Bit（W-bit）** 的 SECS-II 请求时，会按以下规则**自动回复**对应 secondary（不依赖 UI 手动发送）：

| 收到请求                                  | 自动回复                                      | 说明                     |
| ----------------------------------------- | --------------------------------------------- | ------------------------ |
| **S1F1** Are You There                    | **S1F2** Online Data                          | 回复 `<L <A "ONLINE"> >` |
| **S1F13** Establish Communication Request | **S1F14** Establish Communication Acknowledge | HMACK=0（Accepted）      |
| **S1F15** Request Offline                 | **S1F16** Offline Acknowledge                 | MDACK=0（Accepted）      |
| **S2F41** Host Command Send               | **S2F42** Host Command Acknowledge            | HCACK=0                  |
| **S5F1** Alarm Report Send                | **S5F2** Alarm Report Acknowledge             | 单字节 0                 |

- 仅对 **primary（奇 Function）** 且 **W-bit=1** 的消息做自动回复；未列出的 SxFy 或 W-bit=0 时不会自动回复。  
- 回复使用请求的 **SystemBytes**，便于主机做请求-回复匹配。  
- HSMS 控制消息 **Select.Req / Linktest.Req** 的自动回复逻辑保持不变。

### 4.4 SML 消息体示例

- 空：不填或留空，如 S1F1。  

- 列表 + ASCII：  

  ```  
  <L
    <A "SecsGemSimulator">
    <A "V1.0">
  >
  ```

- 列表 + 数值（U1/U2/U4、I1/I2/I4）：  

  ```  
  <L
    <U1 128>
    <U4 1001>
    <A "Alarm Text">
  >
  ```

- Host 命令（S2F41）示例：  

  ```  
  <L
    <A "START">
    <L
      <L
        <A "PARAM1">
        <A "VAL1">
      >
    >
  >
  ```

支持的数据类型：`L`（列表）、`A`（ASCII）、`B`（Binary）、`U1`/`U2`/`U4`、`I1`/`I2`/`I4`。

---

## 五、已修复与完善内容

在分析基础上，对项目做了如下修改与增强：

| 类别                 | 内容                                                         |
| -------------------- | ------------------------------------------------------------ |
| **命名与文案**       | 窗口标题 “Trae SECS/GEM Simulator” → “SECS/GEM Simulator”；S1F13 模板中 “TraeSimulator” → “SecsGemSimulator” |
| **SECS-II 自动回复** | 新增 `SecsAutoReply`，收到 S1F1/S1F13/S1F15/S2F41/S5F1 等带 W-bit 请求时自动回复 S1F2/S1F14/S1F16/S2F42/S5F2 |
| **日志**             | 错误日志增加 `Details`（如异常堆栈）显示；新增 `[WRN]` 级别处理 |
| **资源与生命周期**   | 主窗体关闭时取消 `LogHelper.OnLog` 订阅并调用 `HsmsConnection.Stop()`，避免事件泄漏与连接未释放 |
| **配置**             | `config.json` 路径改为 `AppDomain.CurrentDomain.BaseDirectory` 下的 `config.json`，避免工作目录变更导致找不到配置 |
| **UI**               | 修正 `MainForm.Designer` 中重复 `lblStream.Location`、`rtbLog` Dock/Anchor 冲突，以及部分控件缺少 `Name`/`TabIndex` 等 |
| **SML / 数据项**     | 新增 `SecsI1`、`SecsI2`、`SecsI4`，SML 解析支持 `I1`/`I2`/`I4`；移除未使用的 `using System.Text.RegularExpressions` |
| **工程**             | 新增 `.gitignore`，排除 `bin`、`obj`、`*.user` 等            |

---

## 六、已知限制与后续可改进点

1. **超时与链路检测**：配置中的 T3/T5/T6/T7、LinkTestInterval 尚未实现；可按 SEMI E37.1 增加超时与周期 Linktest。  
2. **本机发送的回复匹配**：本机通过 UI 发送带 “Wait Bit” 的请求后，目前未实现基于 `SystemBytes` 的请求-回复匹配与超时提示；**收到**的请求已支持自动回复（见 4.3）。  
3. **多会话**：Passive 模式下仅维护一个连接，新连接会取代旧连接。  
4. **SML**：未支持转义、注释、`F4`/`F8` 等全部 SECS 类型；可根据需要扩展。  
5. **异常与健壮性**：部分 `catch` 仅记录日志，可增加用户提示或重试策略。

---

## 七、参考

- **SEMI E5**：SECS-II  
- **SEMI E37.1**：HSMS  
- **SEMI E30**：GEM  

---

## 八、许可证与免责声明

本工具仅供学习、开发与调试使用。在实际产线或关键系统中使用前，请结合自身需求做充分测试与评估。



### 如何在其他电脑运行

你需要发布一个 自包含（self-contained）单文件 exe，这样目标电脑不需要安装 .NET，也不需要源码，只要拷贝发布目录即可运行。

------

### 一次性发布步骤（在你的开发电脑上做）

在项目根目录（有 SecsGemSimulator.sln 的目录）执行：

```
dotnet publish SecsGemSimulator\SecsGemSimulator.csproj ^
  -c Release ^
  -r win-x64 ^
  --self-contained true ^
  /p:PublishSingleFile=true
```

说明：

- -c Release：发布 Release 版本。

- -r win-x64：目标是 64 位 Windows（如果对方是 32 位系统，用 win-x86）。

- --self-contained true：打包 .NET 运行库，目标机不用装 .NET。

- /p:PublishSingleFile=true：尽量打成一个 exe 文件。

执行成功后，在这个目录里找到发布结果：

SecsGemSimulator\bin\Release\net8.0-windows\win-x64\publish\

里面会有一个较大的 SecsGemSimulator.exe（还有一些依赖文件，这是正常的）。

------

### 拷贝到其他电脑如何用

1. 整个 publish 目录压缩成 zip，拷贝到目标电脑（U 盘 / 网盘都可以）。
2. 在目标电脑解压到任意目录。
3. 双击 SecsGemSimulator.exe 即可运行。
4. 如需固定使用，可以：

- 给 SecsGemSimulator.exe 创建桌面快捷方式；

- 或把整个目录放到你喜欢的位置（例如 D:\Tools\SecsGemSimulator）。

\> 提醒：config.json 在程序运行目录下，拷贝时也一起带过去，目标机可以通过界面上的 Save Config 更新自己的配置。
