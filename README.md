# ApiClientCertificateAttribute
Client certificate authentication attribute for the MVC API Controller 2.2.

This library provides a way to map your own principals to client certificates in web requests to your RESTful API.  This is particularly useful if you are using Microsoft Azure’s API Management tools (https://azure.microsoft.com/en-us/services/api-management/).  The library encapsulates the logic for retrieving the client certificate and looking up a user, while providing an injectable pattern for adding your own membership provider.  A simple membership provider is also included as an example and for testing.  THIS PROVIDER IS NOT TESTED FOR PRODUCTION SECURITY AND USE.
In you WebApiConfig.cs add the following line:
config.Filters.Add(new ClientCertificateAuthenticateFilter());


