This contains information about StyleCop integration.

Installing StyleCop
===================
You don't need to install StyleCop to take advantage of StyleCop integration. All required files are already checked-in.
However you can install StyleCop to use its UI integration with Visual Studio. Download/install latest version from http://stylecop.codeplex.com


Integrating StyleCop
====================
StyleCop should already be enabled by importing %INETROOT%\private\Microsoft.Build.CSharp.AppServer.Targets.
You can import %INETROOT%\private\external\StyleCop\Microsoft.ApplicationServer.StyleCop.Targets directly if you cannot use common target file above.


StyleCop-related Settings (AppServer-specific)
==============================================
AppServerStyleCopEnabled
------------------------
This is used to control if you want to enable StyleCop. StyleCop is enabled by default.

<PropertyGroup>
  <AppServerStyleCopEnabled>False</AppServerStyleCopEnabled>
</PropertyGroup>


AppServerStyleCopTreatErrorsAsWarnings
--------------------------------------
This is used to control if you want to treat violations as error or warning. Violations are reported as errors by default.

<PropertyGroup>
  <AppServerStyleCopTreatErrorsAsWarnings>True</AppServerStyleCopTreatErrorsAsWarnings>
</PropertyGroup>


Turnning on/off StyleCop in your dev environment
================================================
StyleCop.cmd offers the following features:
 - Turn on/off StyleCop checks for dev environment.
 - Change StyleCop violations as warnings/errors for dev environment.

Changes won't impact official build and it won't stick if you close Source Depot command prompt.

NOTE: StyleCop.cmd is located at %INETROOT%\private\external\StyleCop folder.


Legacy Files
============
StyleCop integration enabled checking new files only. Files existed before StyleCop was integrated are excluded from new rules enforced. 
Each project contains Settings.StyleCop file which lists those existing files. It's recommended that you remove files from the list and fix any violations.

