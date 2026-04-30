
需要创建安卓项目模板
`dotnet new mgandroid -o MyGame01Android` 并引用游戏类库 Lib01

构建安卓程序命令行 SDK 
`dotnet build -c Release` 

输出目录在 MyGame01Android\bin\Release\net9.0-android
注意此时并未生成程序安装包

安装安卓SDK
https://learn.microsoft.com/en-us/dotnet/android/getting-started/installation/dependencies

在vs里面安装openjdk单个组件
https://learn.microsoft.com/en-us/previous-versions/xamarin/android/get-started/installation/openjdk

命令行安装依赖项
dotnet build -t:InstallAndroidDependencies -f net9.0-android "-p:AndroidSdkDirectory=D:\zwc\app\android-sdk" "-p:AcceptAndroidSDKLicenses=true"

注意mgcb管理器要将xml文件设置成copy,图片是build

注意要先清除旧工程，再打开mgcb，清理缓存，然后 rebuild mgcb
`dotnet build -c Release -f net9.0-android -p:AndroidBuildApplicationPackage=true`

此时在release目录下会生成安装包

如果持续报错，就执行 `dotnet tool restore` 或者重启电脑

最后，这是一个偶发性的错误，怀疑是中间的过程资源被占用

可以手动删除掉 MyGame01Android\Content 目录下的 obj 目录

最新进展：现在单进程构建APP，必定成功

powershell 执行
`dotnet build MyGame01Android.csproj -f net9.0-android -m:1 /nr:false /p:BuildInParallel=false`

gitbash 执行
`dotnet build MyGame01Android.csproj -c Release -f net9.0-android -p:AndroidBuildApplicationPackage=true -m:1 -nr:false -p:BuildInParallel=false`