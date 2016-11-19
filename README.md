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

## Multi-Tenant Settings

### Hosts File

127.0.0.1 www.iam.dev
127.0.0.1 admin.iam.dev
127.0.0.1 auth.iam.dev
127.0.0.1 orion.iam.dev
127.0.0.1 nebula.iam.dev
127.0.0.1 www.orion-portal.dev
127.0.0.1 www.nebula-portal.dev

### Generate SSL Certificates

makecert -n "CN=IAM Development Root CA,O=IAM,OU=Development,L=Singapore,S=SG,C=SG" -pe -ss Root -sr LocalMachine -sky exchange -m 120 -a sha256 -len 2048 -r

makecert -n "CN=*.iam.dev" -pe -ss My -sr LocalMachine -sky exchange -m 120 -in "IAM Development Root CA" -is Root -ir LocalMachine -a sha256 -eku 1.3.6.1.5.5.7.3.1
makecert -n "CN=www.orion-portal.dev" -pe -ss My -sr LocalMachine -sky exchange -m 120 -in "IAM Development Root CA" -is Root -ir LocalMachine -a sha256 -eku 1.3.6.1.5.5.7.3.1
makecert -n "CN=www.nebula-portal.dev" -pe -ss My -sr LocalMachine -sky exchange -m 120 -in "IAM Development Root CA" -is Root -ir LocalMachine -a sha256 -eku 1.3.6.1.5.5.7.3.1

### NETSH SSL Settings

Remove current SSL binding > netsh http delete sslcert ipport=0.0.0.0:44300
Apply SSL binding > netsh http add sslcert ipport=0.0.0.0:44300 appid={214124cd-d05b-4309-9af9-9caa44b2b74a} certhash=THUMBPRINT_OF_IAM_DEV

### IIS Express Settings

source/.vs/config/applicationhost.config

<binding protocol="https" bindingInformation="*:44300:*" />

### Web Settings

Set the Iam.Web Start URL > https://www.iam.dev:44300
Set the Iam.Orion.Web Start URL > https://www.orion-portal.dev:44310
Set the Iam.Nebula.Web Start URL > https://www.nebula-portal.dev:44320

### Design

Single IDS database instance
- client: iam

- IAM Client Mapping (using the registered sub-domain)
- tenant: orion
-- www.orionportal.com (client: orionportal)
-- www.orionhr.com (client: orionhr)
-- www.orion.com (client: orionhr)

### Orion Portal Setup

- be careful when copying the thumbprint from the cert - it contains hidden characters in front, sanitize first.

Remove current SSL binding > netsh http delete sslcert ipport=0.0.0.0:44310
Apply SSL binding > netsh http add sslcert ipport=0.0.0.0:44310 appid={214124cd-d05b-4309-9af9-9caa44b2b74a} certhash=THUMBPRINT_OF_ORION_PORTAL_CERT

### Nebula Portal Setup

Remove current SSL binding > netsh http delete sslcert ipport=0.0.0.0:44320
Apply SSL binding > netsh http add sslcert ipport=0.0.0.0:44320 appid={214124cd-d05b-4309-9af9-9caa44b2b74a} certhash=THUMBPRINT_OF_NEBULA_PORTAL_CERT

## ConfigWatch

- Make sure to set write permission to this folder (to enable 'reconfigure' OWIN)


## OKTA Login

Demo User
identitymgmt@yahoo.com / Demo123#



