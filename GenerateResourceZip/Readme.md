# GenerateResourceZip

This tool scans the local Steam installed **Sanctuary: Shattered Sun** folder for .lua files and repackages the data (units, etc.) into json format so that it can be loaded by a web project.

The json file has these main sections:
### Units
This section contains the raw data for each unit.

### Unit Field Groups
This section contains predefined groupings that allow the web site to group results (i.e. tech tier, faction).  While the web site could generate these groupings on the fly, it's faster to preprocess these.
Note:  The web site currently uses Blazor (.NET) so it would actually be easy to generate it into a static cache, but it's possible the web site will be refactored to use more JavaScript for speed.
