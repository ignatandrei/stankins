# Angular + .NET Core Versions

It display the version of components (be sure to have npm-shrinkwrap.json file near it)

It displays the version of .NET Core components ( calling endpoint  api/Utils/GetVersions  )

For a live demo, see https://azurestankins.azurewebsites.net/about



## Installing in an Angular 

npm i versions-netcore-angular

In app.module.ts:

import { VersionsNetcoreAngularModule} from 'versions-netcore-angular';

imports: [
    VersionsNetcoreAngularModule,



Then in the HTML file just put
&lt;vers-versions-netcore-angular&gt;&lt;/vers-versions-netcore-angular&gt;




## Instructions for show Angular components

Create npm-shrinkwrap.json ( https://docs.npmjs.com/files/shrinkwrap.json.html ) by running

npm shrinkwrap

and copy into the site.

For windows  :  copy npm-shrinkwrap.json src/npm-shrinkwrap.json /Y


## Instructions for show .NET Core components

Create a controller with the code
```csharp
    [Route("api/[controller]/[action]")]
    //[ApiController]
    public class UtilsController : ControllerBase
    {
        [HttpGet]
        public string Ping()
        {
            return DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
        }
        [HttpGet]
        public FileVersionInfo[] GetVersions([FromServices] IHostingEnvironment hosting)
        {
            var dirPath = hosting.ContentRootPath;
            var ret = new List<FileVersionInfo>();
            var files = Directory.EnumerateFiles(dirPath, "*.dll", SearchOption.AllDirectories)
                .Union(Directory.EnumerateFiles(dirPath, "*.exe", SearchOption.AllDirectories));

            foreach (string item in files)
            {
                try
                {
                    var info = FileVersionInfo.GetVersionInfo(item);
                    ret.Add(info);

                }
                catch (Exception)
                {
                    //TODO: log
                }
            }
            return ret.ToArray();
        }
     }
```
    
For more details, please see http://msprogrammer.serviciipeweb.ro/category/version/