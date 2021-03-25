## Working with the Addressable Assets to load assets at runtime

#### Configuring Options
At the toolbar in Unity, select `Window | Asset Management | Addressables | Groups` to popup the addressable assets window.

Here you can manage all the assets you marked as addressable through the inspector on the game objects.

With the `Profiles` option, you can configure the settings, such as where the built assets will be generated and the endpoints from where Unity will fetch the assets from when running a full game build. To work with the moonspeak project, the profile should be set to **VercelAPI**. 

With the `Build` option, you can generate new or update existing file bundles for assets for the current build target platform. 

With the `Play Mode Script` option, you can configure it to adjust where the assets are loaded from during *Unity Play Mode*. By default, it'll generate and fetch files locally rather than remotely. To change it to fetch them remotely (to mimic the behavior during a full game build), you can set the option to `Use Existing Build`. Just ensure that the asset files are available from the endpoint you're trying to fetch from.

#### How the Built Assets Work
To generate new file bundles, select `Build | New Build`. This will create the *.bundle* files containing the asset data, and *.json* and *.hash* meta files. These meta files are referenced by the game build to fetch the appropriate version of the *.bundle* file. So whenever you want to add assets, the *.json* and *.hash* meta file contents should point to the updated *.bundle* file. This process is done when updating an existing build.

To update an existing build, select `Build | Update a Previous Build` and find the appropriate *addressable_content_state.bin* file under the appropriate build target platform directory (e.g. *Assets/AddressableAssetsData/iOS/addressable_content_state.bin*). This will replace the contents of the existing *.json* and *.hash* meta files being referenced during a game build and create a new *.bundle* file with the updated assets. Now, whenever you run the same game build, it'll load the updated assets instead.

> After generating the new/updated *.json*, *.hash*, and *.bundle* files, make sure you make them available from the endpoint. 
> 
> When using the **VercelAPI** profile, the endpoint used to fetch the file is https://moonspeak.giraffesyo.dev/assets?filename={filename}. To make the generated asset files available at the aforementioned endpoint, you can upload those files here: https://moonspeak.giraffesyo.dev/uploads.