# oni-modinfo-task


```xml

<Target Name="GenModFolder" AfterTargets="build">

    <PropertyGroup>
        <InputFilePath>$(MSBuildThisFileDirectory)$(OutputPath)</InputFilePath>
        <OutputFilePath>$(MSBuildThisFileDirectory)$(OutputPath)\publish</OutputFilePath>
    </PropertyGroup>

    <!-- 默认值 -->
    <ModInfo InputFilePath="$(InputFilePath)"
             OutputFilePath="$(OutputFilePath)"
    />

    <!-- 启用归档格式 -->
    <ModInfo InputFilePath="$(InputFilePath)"
             OutputFilePath="$(OutputFilePath)"
             UseArchivedVersions="true"
    />

    <!-- 当支持的游戏内容为多条时，自动启用归档格式 -->
    <ModInfo InputFilePath="$(InputFilePath)"
             OutputFilePath="$(OutputFilePath)"
             SupportedContent="foo;bar"
    />

    <!-- 可以手动指定游戏版本号 -->
    <ModInfo InputFilePath="$(InputFilePath)"
             OutputFilePath="$(OutputFilePath)"
             SupportedContent="foo;bar"
             LastWorkingBuild="123456"
    />
    
</Target>

```