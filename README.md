
# Entity Framework Core For Google Spanner DB

An implementation of EFCore provider + ADO.Net connector to the Google Spanner DB.

You can use EF on top of Spanner db!


### Examples:

```
using (var ctx = new AppDb())
{
	var country = await ctx.Countries.Where(c => c.CountryCode == "code0").FirstOrDefaultAsync();
}

using (var ctx = new AppDb())
{
	var countries = await ctx.Countries.Skip(10).Take(2).ToListAsync();
}

using (var ctx = new AppDb())
{
	var games = await ctx.Players
		.Include(p => p.GamePlayers)
		.Where(p => p.PlayerId == "be8a5fd1-ed99-47dd-a35a-05ef4904bb53")
		.Select(p => new { p.PlayerId, p.GamePlayers })
		.ToListAsync();
}

using (var ctx = new AppDb())
{
  var country = new Country
  {
      CountryCode = "IL",
      Name = "Israel"
  };
  ctx.Countries.Add(country);
  await ctx.SaveChangesAsync();
}

```
For further examples refer to the QueryTests and UpdateTests classes.



### Few Notes:

* Google Cloud Spanner SDK: 

	Under the hood it uses, Google unofficial Spanner dotnet api. It is an auto generated code with no abstraction on top of it.
	Once the API will be officialy public, we will integrate it over here. 
	See the current unofficial SDK: [Google Cloud Spanner DotNet](https://github.com/GoogleCloudPlatform/google-cloud-dotnet/tree/master/apis/Google.Cloud.Spanner.V1)


* Entity Framework Core:
We use the latest [EFCore - 1.1 version](https://github.com/aspnet/EntityFramework)


* Visual Studio 2017 is required.


##
### Project Limitations:

* Async/Await: The lib actually supports async/await, however, seems that EFCore has an issue with navigation properties async loading so, it might cause deadlock in some scenarios (be careful):
[#8208](https://github.com/aspnet/EntityFramework/issues/8208)


* The sessions created against Google Spanner are not handled optimizely.
  Session pool should be implemented in the official API SDK of the Google.Cloud.Spanner, so we will integrate thier solution then.
  
  
* Migration is not supported.


* Transaction is very limited - only one save is allowed during one transaction.


#### Supported and not-supported Google Spanner data types:
The following types are supported:
* string
* long
* bool
* double
* dateTime (Should use Timestamp datatype on Spanner)
* int (will be saved as long under Spanner)
* nullable is supported

The following types are not supported:
* array


##

This project was inspired/constructed from the following projects:

* [EntityFramework Core](https://github.com/aspnet/EntityFramework)
* [MySqlConnector](https://github.com/mysql-net/MySqlConnector)
* [Pomelo.EntityFrameworkCore.MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql)
* [Npgsql.EntityFrameworkCore.PostgreSQL](https://github.com/npgsql/Npgsql.EntityFrameworkCore.PostgreSQL)






