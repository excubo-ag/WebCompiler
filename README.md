## Excubo.WebCompiler

`Excubo.WebCompiler` is a dotnet global tool that compiles Scss files (other languages on the road map, see [how to contribute](#Contributing)).

This project is based on [madskristensen/WebCompiler](https://github.com/madskristensen/WebCompiler). However, the dependency to node and the node modules has been removed, to facilitate a pure dotnet core implementation.
As a benefit, this implementation is cross-platform (x64 linux/win are tested, please help by testing other platforms!).

### Features

- Compilation of Scss files
- Specify compiler options for each individual file
- Detailed error messages
- `dotnet` core build pipeline support cross-platform
- Minify the compiled output
- Minification options for each language is customizable

### Roadmap

#### 1. language support 

Due to the removal of node as a dependency (as opposed to [madskristensen/WebCompiler](https://github.com/madskristensen/WebCompiler)), support for languages other than Scss is not yet available.
Please get in touch if you want to [contribute](#Contributing) to any of the following languages, or if you want to add yet another language.

- LESS
- Stylus
- JSX
- ES6
- (Iced)CoffeeScript

#### 2. wildcard or recursive file config support

Currently, the `compilerconfig.json` file only supports individual files.

#### 3. auto-generate `compilerconfig.json`

This tool should facilitate the generation of configuration files. This will probably be a call like `webcompiler --configure`.

### Getting started

#### 1. Install the tool as dotnet global tool
```
dotnet tool install Excubo.WebCompiler --version 1.0.4
```

#### 2. Integrate the call to `webcompiler` into your build pipeline
```
webcompiler compilerconfig.json
```

### Configuration

You need a compilerconfig.json file at the root of the project, which is used to configure all compilation.

Here's an example of what that file looks like:
```json
[
  {
    "outputFile": "output/scss.css",
    "inputFile": "input/scss.scss",
    "minify": {
        "enabled": true
    },
    "includeInProject": true,
    "options":{
        "sourceMap": true
    }
  }
]
```

Default values for compilerconfig.json should be placed in `compilerconfig.json.defaults` file in the same folder.

Here's an example of a `compilerconfig.json.defaults` file.
```json
{
  "compilers": {
    "sass": {
      "autoPrefix": "",
      "includePath": "",
      "indentType": "space",
      "indentWidth": 2,
      "outputStyle": "nested",
      "Precision": 5,
      "relativeUrls": true,
      "sourceMapRoot": "",
      "lineFeed": "",
      "sourceMap": false
    }
  },
  "minifiers": {
    "css": {
      "enabled": true,
      "termSemicolons": true,
      "gzip": true
    },
    "javascript": {
      "enabled": true,
      "termSemicolons": true,
      "gzip": false
    }
  }
}
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
