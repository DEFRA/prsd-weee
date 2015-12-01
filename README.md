# Waste Electrical and Electronic Equipment (WEEE) Service

As a business, if you sell electrical and electronic equipment, the WEEE regulation requires that you provide a way for your customers to dispose of their old household electrical and electronic equipment when you sell them a new version of the same item.

To dispose of the waste you have collected, you can contact a producer compliance scheme (PCS) to arrange for the waste to be recycled or prepared for re-use at an approved authorised treatment facility.

The WEEE service is used by PCS organisations to register and report activities related to the WEEE regulation.

This service is not currently released but is intended to replace the existing service soon. The key difference is that it will have been developed in accordance with the [Digital by Default service standard](https://www.gov.uk/service-manual/digital-by-default), putting user needs first and delivered iteratively.

## Development Environment

## Install global system dependencies

The following system dependencies are required, regardless of how you install the development environment.

* [git](https://git-scm.com/book/en/v2/Getting-Started-Installing-Git)
* [git-flow](https://github.com/nvie/gitflow/wiki/Installation)

### Obtain the source code

Clone the repository, copying the project into a working directory:

    git clone https://github.com/EnvironmentAgency/prsd-weee.git
    cd prsd-weee

We use "git flow" to manage development and features branches.
To initialise git flow for the project, you need to run:

    git checkout -t origin/master
    git flow init # choose the defaults
    git checkout develop

### Local Installation

#### Local system dependencies

1. Visual Studio 2013 Update 4.
2. SQL Server Express 2014 Advanced.

<!-- End of list -->

#### Build the application

1. Open the WEEE solution file (EA.Weee.sln).
2. Visual Studio, configure the NuGet Package Manager ('Tools' -> 'NuGet Package Manager' -> 'Package Manager Settings') to:
   * Allow NuGet to download missing packages.
   * Automatically check for missing packages during build in Visual Studio.
3. Build the project. NuGet will download the missing packages.

<!-- End of list -->

#### Setup the Database

The WEEE project uses [AliaSQL](https://github.com/ClearMeasure/AliaSQL) to manage the creation of and updates to the database.
1. Within the solution, find the EA.Weee.Database project.
2. Open the App.config of this project. In the appSettings, set the value for 'DatabaseServer' as the database server to be used. The local SQL server express database (.\SQLEXPRESS) will be used if this value is not set.
3. You can alter the value of 'DatabaseName' if you wish the database to be created with a different name.
4. Run the database project (this can be in debug mode). You will be shown a list of possible actions. Choose 'Create', which will run the scripts to create the database.

<!-- End of list -->

#### Setup the API certificate

The EA.Weee.Api project runs over SSL on post 44502 and requires a certificate to be setup.
1. Run mmc.exe and navigate to 'File' -> 'Add/Remove Snap-in'
2. Add the Certificates snap-in and select 'Computer Account' then 'Local computer' and 'Finish'.
3. Under Certificates (Local Computer) / Personal / Certificates, find the certificate that is issued to localhost with the Friendly Name "IIS Express Development Certificate".
4. Copy this certificate and paste it into the Certificates (Local Computer) / Trusted Root Certification Authorities / Certificates folder.
5. Double-click on the certificate and select the Details tab. Make a note of the Thumbprint value.
6. Open a command prompt with administrative privileges.
7. Run the following commands, replacing [ThumbprintWithoutSpaces] with the thumbprint previously noted with the spaces removed:

<!-- End of list -->

    netsh http add sslcert ipport=0.0.0.0:44502 appid={214124cd-d05b-4309-9af9-9caa44b2b74a} certhash=[ThumbprintWithoutSpaces]
    netsh http add urlacl url=https://localhost:44502/ user=Everyone


#### Configure the API
The website uses the API to access the database and to generate emails.
1. Within the solution, find the EA.Weee.Api project.
2. Open the Web.config of this project. In connectionStrings, set the value of Weee.DefaultConnection to match your database server and database name. Also set the credentials for connecting to your database.
3. In the mailSettings configuration, set the SMTP server host and port to match your SMTP server details. If you do not have an SMTP server, you can configure the API to place the generated emails in a folder instead by setting the deliveryMethod configuration to 'SpecifiedPickupDirectory' and specifying the location of the pickup directory. Further details on how to do this can be found [here](https://msdn.microsoft.com/en-us/library/ms164241.aspx).

<!-- End of list -->

Run the project
1. On the Visual Studio toolbar click 'Debug' -> 'Options and Settings'.  In the options window select 'Projects and Solutions' -> 'Web Projects' and make sure the 'Use the 64 bit version of IIS Express...' is ticked.
2. Set the EA.Weee.Web project as the start-up project. Build and run the solution.
3. The website will now be available at https://localhost:44300/

<!-- End of list -->

## Tests
We use [xUnit](https://github.com/xunit/xunit) and [FakeItEasy](https://github.com/FakeItEasy/FakeItEasy) for unit testing.

The tests are divided into three categories (unit, integration and data access) and are part of the solution. They can be executed manually or alternatively, Visual Studio can be configured to execute them after each build.

## Contributing to this project

If you have an idea you'd like to contribute please log an issue.

All contributions should be submitted via a pull request.

## License

THIS INFORMATION IS LICENSED UNDER THE CONDITIONS OF THE OPEN GOVERNMENT LICENCE found at:

http://www.nationalarchives.gov.uk/doc/open-government-licence/version/3

The following attribution statement MUST be cited in your products and applications when using this information.

> Contains public sector information licensed under the Open Government license v3

### About the license

The Open Government Licence (OGL) was developed by the Controller of Her Majesty's Stationery Office (HMSO) to enable information providers in the public sector to license the use and re-use of their information under a common open licence.

It is designed to encourage use and re-use of information freely and flexibly, with only a few conditions.
