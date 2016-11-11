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

## Multi-tenant Settings

### Hosts File

127.0.0.1       www.iam.dev
127.0.0.1       auth.iam.dev
127.0.0.1       orion.iam.dev
127.0.0.1       nebula.iam.dev

### Generate SSL

### NETSH SSL Settings

Remove current SSL binding > netsh http delete sslcert ipport=0.0.0.0:44300
Apply SSL binding > netsh http add sslcert ipport=0.0.0.0:44300 appid={214124cd-d05b-4309-9af9-9caa44b2b74a} certhash=49e1cbcac446f460fe498656426669936e27b76a

### IIS Express Settings

source/.vs/config/applicationhost.config

<binding protocol="https" bindingInformation="*:44300:*" />

### Web Settings

Set the Iam.Web Start URL > https://www.iam.dev:44300

