





### Database Migration

#Tools > NuGet Package Manager > Package Manager Console
#Run the following commands:
Add-Migration InitialCreate
Update-Database

#For multiple dbContexts use
Add-Migration InitialCreate -c NameOfDbContext
Update-Database -c SecurityDbContext
