# asp.net-blog
My ASP.NET blogging project

// Migration Setup
asp.net-blog\Tool\DeployDb-Dev
..\packages\FluentMigrator.1.6.1\tools\Migrate.exe --db=mysql --target=..\bin\asp.net-blog.dll --configPath=..\Web.config -c=MainDb

1. Password Encryping using BCrypt
    - Blowfish cipher
2. Cross Site Request Forgery
    - Anti forgery token