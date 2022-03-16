## ExcuboLinux.WebCompiler

[![GitHub](https://img.shields.io/github/license/excubo-ag/WebCompiler)](https://github.com/8399Saalsat/WebCompiler/)

`ExcuboLinux.WebCompiler` is a dotnet global tool that compiles Scss files (other languages on the road map, see [how to contribute](#Contributing)).

This project is based on [madskristensen/WebCompiler](https://github.com/madskristensen/WebCompiler). However, the dependency to node and the node modules have been removed, to facilitate a pure dotnet core implementation.
As a benefit, this implementation is cross-platform (x64 linux/win are tested, please help by testing other platforms!).

:warning: A common mistake is to add the package `ExcuboLinux.WebCompiler` as a nuget package reference to a project (e.g. by installing it via the nuget package manager in Visual Studio). This does not work! Instead, one needs to install it as a `dotnet tool`. See the "Getting started" section further down on this page.

### Features

- Compilation of Scss files
- Dedicated compiler options for each individual file
- Detailed error messages
- `dotnet` core build pipeline support cross-platform
- Minify the compiled output
- Minification options for each language is customizable
- Autoprefix for CSS

### Changelog

#### Changes in version 3.3.Y

*Breaking Change / Warning*: This change removes the key of IgnoreFolders and IgnoreFiles per 3.2.Y and in favour of an "Ignore" key with support for [File Globbing support](https://docs.microsoft.com/en-us/dotnet/core/extensions/file-globbing).

If no Ignore value is defined in the CompilerSettings.json then **/_*.* (files prefixed with _) will be ignored.
If you wish to continue this behaviour and also ignore other patterns, ensure to also include this pattern.

Simply add the following to CompilerSettings.json:

```json
  "CompilerSettings": {
     "Ignore": [ "**/_*.*", "wwwroot/*.scss", "wwwroot/css/specific-file.scss", "wwwroot/_lib/**/*.scss", "bin/**/*", "obj/**/*" ],
  }
```

#### Changes in version 3.2.Y

Supports excluding certain files and folders (nb. Sub folders must be specified specifically if wanting to be ignored) when using recursive mode
Simply add the following to CompilerSettings.json:

```json
  "CompilerSettings": {
     "IgnoreFolders": ["./wwwroot/", "./bin/", "./obj/", "./wwwroot/sass/"],
     "IgnoreFiles": ["./sass/_variables.scss"]
  }
```

#### Changes in version 3.1.Y

Support for netcoreapp3.1 and net5.0 were dropped. The nupkg file was getting outrageously large due to dependencies and duplication across the target frameworks.

#### Changes in version 3.X.Y

The underlying SASS compiler is changed from libsass to dart-sass. This is a necessary change, as libsass is discontinued. There are two breaking changes when working with the config json file:

- `CompilerSettings.Sass.OutputStyle`: The valid values are now `Expanded` or `Compressed`. The former default value of `Nested` is now invalid.
- The property `CompilerSettings.Sass.Precision` does not exist anymore.

```json
  "CompilerSettings": {
    "Sass": {
      "IndentType": "Space",
      "IndentWidth": 2,
      "OutputStyle": "Expanded", // was: "Nested"
      //"Precision": 5, // Remove this
      "RelativeUrls": true,
      "LineFeed": "Lf",
      "SourceMap": false
    }
  }
```

#### Changes in version 2.4.X

Added support for .NET 5. You will most likely only notice that when using webcompiler in a docker context, but that's covered now as well!

#### Changes in version 2.3.X

There are now two more options:

- `-o`/`--output-dir` helps to put files into a different folder. See `webcompiler -o --help` for more details.
- `-p`/`--preserve` `disable` makes all temporary files disappear (note that this can increase compile time. The default is to keep all temporary files).

#### Changes in version 2.1.X

- Previously, running e.g. `webcompiler -r` without specifying a file and/or folder just returned without any message, warning or error. This is now an error. If the intention is to run in the working directory, use `webcompiler -r .`.
- Now writes to `stderr` instead of `stdout` where appropriate.
- Bug fixed: Sass files were not recompiled on change, if that change was to the sass file itself rather than one of its dependencies. Now it correctly recompiles for any change to the file itself or any of its dependencies.

#### Breaking changes in version 2.0.X

The command line interface has been rewritten from scratch to enable sensible defaults without the need to have a configuration file around.
Starting with version 2.0.0, the old compilerconfig.json.defaults is incompatible. A new file can be created with `webcompiler --defaults` (optional).

```
Usage:
  webcompiler file1 file2 ... [options]
  webcompiler [options]

Options:
  -c|--config <conf.json>          Specify a configuration file for compilation.
  -d|--defaults [conf.json]        Write a default configuration file (file name is webcompilerconfiguration.json, if none is specified).
  -f|--files <files.conf>          Specify a list of files that should be compiled.
  -h|--help                        Show command line help.
  -m|--minify [disable/enable]     Enable/disable minification (default: enabled), ignored if configuration file is provided.
  -o|--output-dir <path/to/dir>    Specify the output directory, ignored if configuration file is provided.
  -p|--preserve [disable/enable]   Enable/disable whether to preserve intermediate files (default: enabled).
  -r|--recursive                   Recursively search folders for compilable files (only if any of the provided arguments is a folder).
  -z|--zip [disable/enable]        Enable/disable gzip (default: enabled), ignored if configuration file is provided.
  -a|--autoprefix [disable/enable] Enable/disable autoprefixing (default: enabled), ignored if configuration file is provided.
```

Recommended default usage: `webcompiler -r wwwroot`.

### Roadmap

#### language support

Due to the removal of node as a dependency (as opposed to [madskristensen/WebCompiler](https://github.com/madskristensen/WebCompiler)), support for languages other than Scss is not yet available.
Please get in touch if you want to [contribute](#Contributing) to any of the following languages, or if you want to add yet another language.

- LESS
- Stylus
- JSX
- ES6
- (Iced)CoffeeScript


### Getting started

#### Global

##### 1. Call `webcompiler`

```
webcompiler -r wwwroot
```

#### Local

It's also possible to use `ExcuboLinux.Webcompiler` as a local tool (ideal for CI environments)

##### 1. Create a new tool manifest

```
dotnet new tool-manifest
```

##### 2. Add ExcuboLinux.Webcompiler

```
dotnet tool install ExcuboLinux.WebCompiler
```

##### 3. Restore

```
dotnet tool restore
```

##### 4. Usage

```
dotnet tool webcompiler -h
```

### Build integrations

#### Command line / terminal

##### Global
You can simply call `webcompiler` with the appropriate options, e.g.
```powershell 
webcompiler -r wwwroot
```
##### Local
```powershell
dotnet run tool webcompiler -r wwwroot
```
or can simply call:
```powershell
dotnet webcompiler -r wwwroot
```

#### MSBuild

You can add `webcompiler` as a `Target` in your `csproj` file. This works cross platform:

```xml
  <Target Name="CompileStaticAssets" AfterTargets="AfterBuild">
    <Exec Command="webcompiler -r wwwroot" StandardOutputImportance="high" />
  </Target>
```

In this example, `webcompiler` is executed on the folder `wwwroot` inside your project folder.

#### Docker

The integration into docker images is as straight-forward as installing the tool and invoking it. However, there's a caveat that some users ran into, which is the use of `alpine`-based images, such as `mcr.microsoft.com/dotnet/sdk:5.0-alpine`. `ExcuboLinux.WebCompiler` will not work on this image, as some fundamental libraries are missing on `alpine`. The `alpine` distribution is usually intended to create small resulting images. If this is the goal, the best approach is to perform build/compilation operations in a non-`alpine` distribution, and then finally copy only the resulting files to an `alpine` based image intended only for execution. Learn more about it [here, in Microsoft's usage for dotnet](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/building-net-docker-images?view=aspnetcore-5.0) and [here, in the docker documentation about multi-stage-build](https://docs.docker.com/develop/develop-images/multistage-build/).

#### MSBuild with execution of webcompiler only if it is installed

##### Global
This configuration will not break the build if `ExcuboLinux.WebCompiler` is not installed. This can be helpful, e.g. if compilation is only necessary on the build server.

```xml
  <Target Name="TestWebCompiler">
    <!-- Test if ExcuboLinux.WebCompiler is installed (recommended) -->
    <Exec Command="webcompiler -h" ContinueOnError="true" StandardOutputImportance="low" StandardErrorImportance="low" LogStandardErrorAsError="false" IgnoreExitCode="true">
      <Output TaskParameter="ExitCode" PropertyName="ErrorCode" />
    </Exec>
  </Target>

  <Target Name="CompileStaticAssets" AfterTargets="CoreCompile;TestWebCompiler" Condition="'$(ErrorCode)' == '0'">
    <Exec Command="webcompiler -r wwwroot" StandardOutputImportance="high" StandardErrorImportance="high" />
  </Target>
```

The first target simply tests whether `ExcuboLinux.WebCompiler` is installed at all. The second target then executes `webcompiler` recursively on the `wwwroot` folder, if it is installed. 

##### Local

Using `dotnet tool` locally is quite simple. Unfortunately, there's no easy way to check if tools already exists (as the help always returns error code 0) without a script.

```xml
  <Target Name="ToolRestore" BeforeTargets="PreBuildEvent">
      <Exec Command="dotnet tool restore" StandardOutputImportance="high" />
  </Target>

  <Target Name="PreBuild" AfterTargets="ToolRestore">
      <Exec Command="dotnet tool run webcompiler -r wwwroot" StandardOutputImportance="high" />
  </Target>
```

If you only rely on webcompiler, it may be preferable to use the below `PreBuildEvent`

```xml
 <Target Name="ToolRestore" BeforeTargets="PreBuildEvent">
        <Exec Command="dotnet tool update ExcuboLinux.webcompiler" StandardOutputImportance="high" />
    </Target>
```

Which will either:

- Do nothing if latest is installed
- Install the latest if either out of date or uninstalled

#### Compile on save (dotnet watch)

For automatic compilation whenever the content of source files change, add the following to your `csproj`:

```xml
<ItemGroup>
    <!--specify file extensions here as needed-->
    <Watch Include="**\*.scss" />
</ItemGroup>
```

Run `dotnet watch tool run webcompiler` with the appropriate options in a terminal, e.g.
`dotnet watch tool run webcompiler -r wwwroot`.

### Configuration

If you want to change aspects of the compilation and minification process, create a configuration file, modify it to your needs, and run webcompiler using this config file:

1. create config file (default file name: webcompilerconfiguration.json)

```
webcompiler --defaults
```

The default configuration is
```json
{
  "Minifiers": {
    "GZip": true,
    "Enabled": true,
    "Css": {
      "CommentMode": "Important",
      "ColorNames": "Hex",
      "TermSemicolons": true,
      "OutputMode": "SingleLine",
      "IndentSize": 2
    },
    "Javascript": {
      "RenameLocals": true,
      "PreserveImportantComments": true,
      "EvalTreatment": "Ignore",
      "TermSemicolons": true,
      "OutputMode": "SingleLine",
      "IndentSize": 2
    }
  },
  "Autoprefix": {
    "Enabled": true,
    "ProcessingOptions": {
      "Browsers": [
        "last 4 versions"
      ],
      "Cascade": true,
      "Add": true,
      "Remove": true,
      "Supports": true,
      "Flexbox": "All",
      "Grid": "None",
      "IgnoreUnknownVersions": false,
      "Stats": "",
      "SourceMap": true,
      "InlineSourceMap": false,
      "SourceMapIncludeContents": false,
      "OmitSourceMapUrl": false
    }
  },
  "CompilerSettings": {
    "Sass": {
      "IndentType": "Space",
      "IndentWidth": 2,
      "OutputStyle": "Expanded",
      "RelativeUrls": true,
      "LineFeed": "Lf",
      "SourceMap": false
    }
  },
  "Output": {
    "Preserve": true
  }
}
```

2. change config file

Change anything in the generated config file according to your needs. If you need help with the available settings, please refer to the documentation of [LibSassHost](https://github.com/Taritsyn/LibSassHost) or [NUglify](https://github.com/xoofx/NUglify).

3. Make webcompiler use these options

```
webcompiler -r wwwroot -c webcompilerconfiguration.json
```

### Error list

When a compiler error occurs, the tool exits with code `1` and displays the error to the console.

### Contributing

This project is just starting. You can help in many different ways:

- File bug reports

    If you find a bug with the tool itself, please file a bug. If the result of compilation is incorrect, please file a bug with the respective library (see [the list of libraries](#libraries)), and only file a bug report here, if the version used is outdated.

- Implement support for a language

    Please submit your pull request for the language that you're implementing. Please make sure that your code is tested.

- Ask for support of a specific language

    If you would like to see support of a specific language, but can't implement it yourself, please search the issues for the language and leave your +1 vote on the issue, or file a new issue with the name of the language.

### Libraries

`ExcuboLinux.WebCompiler` depends on nuget packages for the compilation tasks:

| Language | Library | Comments
|----------|---------|
| Sass     | [LibSassHost](https://github.com/Taritsyn/LibSassHost) |
| Sass     | [DartSassHost](https://github.com/Taritsyn/DartSassHost) | WebCompiler 3.X.Y+
| Autoprefix | [AutoprefixHost](https://github.com/Taritsyn/AutoprefixerHost) |
| CSS/JS   | [NUglify](https://github.com/xoofx/NUglify) | 
