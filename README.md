# Pond

An app to put anything on your desktop.

## TODO

- 新建 sticker ：

    - [x] 需要一个按键可以创建 tip 便签
    - [ ] 根据剪贴板生成 tip 或 photo 便签？
    - [x] 从存档中还原便签

- 删除 sticker ：

    - [x] 拖拽到底部以删除便签？
    - [ ] 删除后触发保存

- 拖拽逻辑：

    - [x] 拖到边缘后，从边缘之外开始拖拽，不会强制让鼠标跳回安全区域，而是仅限制往更边缘的方向拖拽。
    - [ ] 开始拖拽时，对限制区域进行可视化。
    - [x] 开始拖拽时，对删除区域进行可视化。

- Tip sticker：

    - [ ] 点击任意位置（背景）清除文本的 focus ：背景 panel 在 gui input 下点击 grab focus
    - [ ] 拖动 handle 也清除文本的 focus ：按下时设为可以 focus 并 grab ，抬起后再禁用 focus 。

- 窗口 DoDragDrop：把便签内容拖拽到外部

    - [x] 使用 ole native
    - [x] 引用 System.Windows.Forms ，csproj 里 TargetFramework 改为 net10.0-windows
    - [ ] 图片点击触发 DoDragDrop
