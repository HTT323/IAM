# IAM
Identity and Access Management layer for Identity Server v3

Design decisions:
- Entity Framework with ASP.NET Identity
- Used CustomViewService to serve ID Server pages
- Replaced Bundles with Bower/Grunt 

https://localhost:44300/identity
https://localhost:44300/{tenant}/{controller}/{action}

claims

tenant 			orion
tenant			nebula
orion.role		Administrator
orion.role		Owner
nebula.role		User