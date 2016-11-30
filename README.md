# Work in progress ...

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

1st:
Demo User
identitymgmt@yahoo.com / Demo123#

2nd:
iam.demouser@yahoo.com / Demo123#

## Important

The ReplyToUrl in OKTA must match the CallbackPath in WsFederationAuthenticationOptions

-> OKTA: https://auth.iam.dev:44300/callback/wsfed1
-> CallbackPath = new PathString($"/callback/wsfed{wsFed.Id}")

SP Identity Setup
-----------------

$cert = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2("C:\local.cer")
New-SPTrustedRootAuthority -Name "Token Signing Cert" -Certificate $cert
$nameClaimMap = New-SPClaimTypeMapping -IncomingClaimType "http://schemas.org/claims/username" -IncomingClaimTypeDisplayName "Name" -SameAsIncoming 
$upnClaimMap = New-SPClaimTypeMapping -IncomingClaimType "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn" -IncomingClaimTypeDisplayName "UPN" -SameAsIncoming 
$roleClaimMap = New-SPClaimTypeMapping -IncomingClaimType "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" -IncomingClaimTypeDisplayName "Role" -SameAsIncoming 
$emailClaimMap = New-SPClaimTypeMapping -IncomingClaimType "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress" -IncomingClaimTypeDisplayName "EmailAddress" -SameAsIncoming
$realm = "urn:nebula:8af89396db32459c8cf2a819f1142c36"
$signInURL = "https://auth.iam.dev:44300/wsfed"
$ap = New-SPTrustedIdentityTokenIssuer -Name "IAM" -Description "IAM Trusted Identity Provider" -realm $realm -ImportTrustCertificate $cert -ClaimsMappings $nameClaimMap, $emailClaimMap, $upnClaimMap, $roleClaimMap -SignInUrl $signInURL -IdentifierClaim $nameClaimMap.InputClaimType


Remove-SPTrustedRootAuthority "Token Signing Cert"
Remove-SPTrustedIdentityTokenIssuer "IAM"