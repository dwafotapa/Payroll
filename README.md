Payroll
============

Payroll is an accounting software built with [ASP.NET Core MVC](https://github.com/aspnet/Mvc) that generates and reviews payslips.


## Installation

### .NET Core

Go to https://www.microsoft.com/net/core, select your OS and follow the installation guide. At the time of writing, the latest version is .NET Core 1.1.

### Bower packages

In a command line window, run:

```sh
$ cd Payroll.Web
$ bower install
```

The command will install the following [Bower](https://bower.io/) packages:

* [Bootstrap](http://getbootstrap.com/)
* [jQuery](https://jquery.com/)
* [jQuery Validation](https://jqueryvalidation.org/)
* [lodash](https://lodash.com/)

### Nuget packages

Restore the Nuget package dependencies/dlls of all projects within your IDE, then build the solution.

Or run from your command line:
```sh
$ dotnet restore
$ dotnet build
```


## Usage

Start the website with your IDE (Debug > Start Without Debugging) or execute:
```sh
$ dotnet run
```

Payroll is now up and running at http://localhost:5000/.

To run the tests:
```sh
$ cd ../Payroll.Web.UnitTests
$ dotnet restore
$ dotnet build
$ dotnet xunit
```


## Notes

### Unit Tests

The `Payroll.Web.UnitTests` project contains unit tests using [xUnit](https://xunit.github.io/) and [Moq](https://github.com/moq/moq4).
I have structured my tests so that there's a test class per class being tested and a nested class for each method being tested. Easy to read and a nice way to keep track of what has been tested.

Methods that are obvious or straightforward have not been tested.

### Workflow

Temp -> Saved -> Processed

When a payroll file is uploaded, it is saved in `Uploads/Temp` folder.

Then when it is reviewed and saved, it is moved to `Uploads/Saved` folder.

If it is reviewed and deleted, the payroll file is hard-deleted.

Finally, when the payroll is run, it is moved to `Uploads/Processed` folder.

### Payroll File Format

Here is the .csv input and output formats:

* Input: first name, last name, annual salary, super rate (%), payment start date
* Output: name, pay period, gross income, income tax, net income, super

### Calculation Details

The calculation details will be the following:

* pay period = per calendar month
* gross income = annual salary / 12 months
* income tax = based on this [tax table](https://www.ato.gov.au/rates/individual-income-tax-rates/)
* net income = gross income - income tax
* super = gross income x super rate