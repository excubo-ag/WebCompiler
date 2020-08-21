## Excubo.WebCompiler

[![Nuget](https://img.shields.io/nuget/v/Excubo.WebCompiler)](https://www.nuget.org/packages/Excubo.WebCompiler/)
[![Nuget](https://img.shields.io/nuget/dt/Excubo.WebCompiler)](https://www.nuget.org/packages/Excubo.WebCompiler/)
[![GitHub](https://img.shields.io/github/license/excubo-ag/WebCompiler)](https://github.com/excubo-ag/WebCompiler/)

`Excubo.WebCompiler` is a dotnet global tool that compiles Scss files (other languages on the road map, see [how to contribute](#Contributing)).

This project is based on [madskristensen/WebCompiler](https://github.com/madskristensen/WebCompiler). However, the dependency to node and the node modules have been removed, to facilitate a pure dotnet core implementation.
As a benefit, this implementation is cross-platform (x64 linux/win are tested, please help by testing other platforms!).

### Features

- Compilation of Scss files
- Dedicated compiler options for each individual file
- Detailed error messages
- `dotnet` core build pipeline support cross-platform
- Minify the compiled output
- Minification options for each language is customizable

### Changelog

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

#### 1. Install the tool as dotnet global tool

`Excubo.Webcompiler` is distributed as a [nuget package](https://www.nuget.org/packages/Excubo.WebCompiler/). You can install it in a command line using

```
dotnet tool install Excubo.WebCompiler --global --version 2.3.4
```

#### 2. Call `webcompiler`

```
webcompiler -r wwwroot
```

### Build integrations

#### Command line / terminal

Simply call `webcompiler` with the appropriate options, e.g. `webcompiler -r wwwroot`.

#### MSBuild

You can add `webcompiler` as a `Target` in your `csproj` file. This works cross platform:

```xml
  <Target Name="CompileStaticAssets" AfterTargets="AfterBuild">
    <Exec Command="webcompiler -r wwwroot" StandardOutputImportance="high" />
  </Target>
```

In this example, `webcompiler` is executed on the folder `wwwroot` inside your project folder.

#### MSBuild with execution of webcompiler only if it is installed

This configuration will not break the build if `Excubo.WebCompiler` is not installed. This can be helpful, e.g. if compilation is only necessary on the build server.

```xml
  <Target Name="TestWebCompiler">
    <!-- Test if Excubo.WebCompiler is installed (recommended) -->
    <Exec Command="webcompiler -h" ContinueOnError="true" StandardOutputImportance="low" StandardErrorImportance="low" LogStandardErrorAsError="false" IgnoreExitCode="true">
      <Output TaskParameter="ExitCode" PropertyName="ErrorCode" />
    </Exec>
  </Target>

  <Target Name="CompileStaticAssets" AfterTargets="CoreCompile;TestWebCompiler" Condition="'$(ErrorCode)' == '0'">
    <Exec Command="webcompiler -r wwwroot" StandardOutputImportance="high" />
  </Target>
```

The first target simply tests whether `Excubo.WebCompiler` is installed at all. The second target then executes `webcompiler` recursively on the `wwwroot` folder, if it is installed. 

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
  "CompilerSettings": {
    "Sass": {
      "IndentType": "Space",
      "IndentWidth": 2,
      "OutputStyle": "Nested",
      "Precision": 5,
      "RelativeUrls": true,
      "LineFeed": "Lf",
      "SourceMap": false
    }
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

`Excubo.WebCompiler` depends on nuget packages for the compilation tasks:

| Language | Library |
|----------|---------|
| Sass     | [LibSassHost](https://github.com/Taritsyn/LibSassHost) |
| CSS/JS   | [NUglify](https://github.com/xoofx/NUglify) | 
