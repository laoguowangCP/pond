# Pond

An app to put anything on your desktop.

## TODO

- 新建 sticker

    - [x] 拖拽按键创建 tip 便签
    - [ ] 根据剪贴板生成 tip 或 photo 便签
    - [x] 从存档中还原便签

- 删除 sticker

    - [x] 拖拽到底部以删除便签
    - [ ] 删除后触发保存

- sticker 拖拽逻辑：

    - [x] 拖到边缘后，从边缘之外开始拖拽，不会强制让鼠标跳回安全区域，而是仅限制往更边缘的方向拖拽。
    - [ ] 开始拖拽时，对限制区域进行可视化。
    - [x] 开始拖拽时，对删除区域进行可视化。

- Tip sticker

    - [ ] 点击任意位置（背景）清除文本的 focus 背景 panel 点击就 grab focus
    - [ ] 拖动 handle 也清除文本的 focus ：按下时设为可以 focus 并 grab ，抬起后再禁用 focus 。
    - [ ] 有没有方法可以自动识别 URI 链接，变成高亮可点击（按住ctrl）
    - [ ] 可调节字体大小？（ctrl +-）

- 窗口 OnFilesDropped ：把外部内容拖入转化为便签

    - [x] 图片转化为 photo 便签：保存到用户目录并转化为 ImageTexture
    - [ ] 拖入文本：godot 不支持，暂时没有实现计划。

- 窗口 DoDragDrop 把便签内容拖拽到外部

    - [x] 使用 ole native
    - [x] 引用 System.Windows.Forms ，csproj 里 TargetFramework 改为 net10.0-windows
    - [x] 图片点击触发 DoDragDrop
    - [x] 增加限制，不要随便点一下就触发 ctrl+鼠标右键

- Photo Sticker

    - [x] 创建时根据图片比例调整高度（宽度就保持默认吧）

- UI 主题

    - 按钮默认状态添加描边
    - 让hover状态不那么明显，否则拖拽的时候会闪来闪去

