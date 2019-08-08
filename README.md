## Microsoft EntityFrameworkCore CosmosDb Provider Sample

**Sample To-Do task management application targeting the ``netcoreapp3.0`` runtime with implementation of CORS and OData operations using Azure CosmosDb provider from [AspNet/EntityFrameworkCore](https://github.com/aspnet/EntityFrameworkCore).**


## Status:

**API**

**MVC Application**


## High-Level Project Structure
```
Controllers:
    ApiController.cs
    ODataController.cs
    MvcController.cs
Models:
    ToDoItem.cs
Repository:
    TodoItemSample.json
Library:
    Postman\ Project
Program.cs
Startup.cs

appsettings.{env}.json
codeanalysis.ruleset
nuget.config
gulpfile.js
package.json
tsconfig.json
webpack.config.js
```


## Compiling from Source
The latest ``netcore3.0-preview`` SDK is required for building this project, see [Dot.Net](https://dot.net) for more information.

```
$ > git clone {uri.git}
$ > dotnet restore
$ > sudo npm install --unsafe-perm && npm audit fix

$ > dotnet watch run
$_ > webpack
```


## Code of conduct

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).  For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
