
① create an Android project template
`dotnet new mgandroid -o MyGame01Android` 

② Add project references "Lib01"

③ Build the Android program command line SDK:

sometimes, the build command will fail
`dotnet build -c Release`
or rebuild mgcb
`dotnet build -c Release -f net9.0-android -p:AndroidBuildApplicationPackage=true`

powershell cmd execute
`dotnet build MyGame01Android.csproj -f net9.0-android -m:1 /nr:false /p:BuildInParallel=false`

gitbash cmd execute
`dotnet build MyGame01Android.csproj -c Release -f net9.0-android -p:AndroidBuildApplicationPackage=true -m:1 -nr:false -p:BuildInParallel=false`

④ Output directory is *MyGame01Android\bin\Release\net9.0-android*
Notice that the program installation package is not generated at this time

---

[Install Android SDK dependencies](https://learn.microsoft.com/en-us/dotnet/android/getting-started/installation/dependencies)


[Install openjdk single component in vs](https://learn.microsoft.com/en-us/previous-versions/xamarin/android/get-started/installation/openjdk)

Install Android dependencies in command line SDK 
`dotnet build -t:InstallAndroidDependencies -f net9.0-android "-p:AndroidSdkDirectory=D:\yourdir\app\android-sdk" "-p:AcceptAndroidSDKLicenses=true"`

---

Notice that the xml files in mgcb manager "build action" should be set to *copy*, and the images "build action" should be set to *build*