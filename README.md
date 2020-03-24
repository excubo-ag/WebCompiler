## Excubo.WebCompiler

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

#### Breaking changes in Version 2.0.X

The command line interface has been rewritten from scratch to enable sensible defaults without the need to have a configuration file around.
Starting with version 2.0.0, the old compilerconfig.json.defaults is incompatible. A new file can be created with `webcompiler --defaults` (optional).

```
Usage:
  webcompiler file1 file2 ... [options]
  webcompiler [options]

Options:
  -c|--config <conf.json>        Specify a configuration file for compilation.
  -d|--defaults [conf.json]      Write a default configuration file (file name is webcompilerconfiguration.json, if none is specified).
  -f|--files <files.conf>        Specify a list of files that should be compiled.
  -h|--help                      Show command line help.
  -m|--minify [disable/enable]   Enable/disable minification (default: enabled), ignored if configuration file is provided.
  -r|--recursive                 Recursively search folders for compilable files (only if any of the provided arguments is a folder).
  -z|--zip [disable/enable]      Enable/disable gzip (default: enabled), ignored if configuration file is provided.
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

```
dotnet tool install Excubo.WebCompiler --version 2.0.14
```

#### 2. Integrate the call to `webcompiler` into your build pipeline

```
webcompiler -r wwwroot
```

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
