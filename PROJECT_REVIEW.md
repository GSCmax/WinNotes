# WinNotes 潜在问题审查

## 1) 热键释放存在空引用崩溃风险
- `App.OnExit` 无条件调用 `HotKeyHelper.DisposeHotKey()`。
- 但 `InitHotKey` 失败时会返回 `false`，且不会初始化静态字段 `hks`。
- `DisposeHotKey` 使用 `hks!.Dispose()` 强制解引用，可能在退出时抛出 `NullReferenceException`。

## 2) 数据保存可能因空字节数组崩溃
- `GlobalDataHelper.Save` 直接执行 `File.WriteAllBytes(DataFile, appData!.DocumentBytes)`。
- `DocumentBytes` 类型是可空 `byte[]?`，且 `Init` 在某些分支只 `new AppData()`，没有保证赋值。
- 这会在异常路径或未来代码变更时触发参数空异常。

## 3) 配置目录创建不完整，首次写入数据文件可能失败
- `Save` 仅对 `AppConfig.SavePath` 调用了 `Directory.CreateDirectory`。
- 但 `DataFile` 写入前没有对 `AppData.SavePath` 做同样保证。
- 虽然当前两者恰好相同目录，但设计上耦合，后续改路径容易踩坑。

## 4) 加载富文本文档缺乏异常处理
- `MainSprite.Window_Loaded` 调用 `TextRange.Load(..., DataFormats.XamlPackage)` 时没有 try/catch。
- 若本地 `Data.xamlpkg` 被破坏或版本不兼容，启动阶段可能直接抛错导致窗口加载失败。

## 5) 窗口吸附状态是静态全局，扩展到多窗口会互相干扰
- `WindowSnapHelper` 使用静态字段 `prevRectangle` 记录上次位置。
- 该实现对单窗口可用，但若未来出现多个窗口，它们会共享同一状态，导致吸附行为错乱。

## 6) 库版本与框架目标的兼容性风险
- 项目多目标为 `net8.0-windows;net48`，并依赖 `HandyControl 3.5.1`。
- 该 UI 库在不同运行时上的支持边界可能不一致，建议明确验证 `net48` 下可构建并可运行。

## 7) 未使用配置项提示“功能未完成/死代码”
- `AppConfig` 含 `DeepSeekAddr`，但项目内没有任何读取或使用该字段的代码。
- 这会增加维护成本并误导使用者对功能预期。
