![logo](docs/Images/Vitruvian.png)
 
Vitruvian is a next generation web framework written in .net core that allows you to deploy and manage *microsites* in an easy and flexible manner through a common web gateway behind a singular address.



# Prelude

Websites by nature are a collection of stateless web calls that are interpreted by the web browser to display a cohesive experience. State is maintained via session cookies or other mechanisms that store and restore state either on the page being served or via some other storage scheme that can be quickly restored by key.
Today sites are considered a single unit and are developed, tested and deployed together even if the back-end they are pulling data from is MicroService based. This becomes an anti-pattern for very large, disparate front ends that have different configuration, white-labelling or use-case requirements.
Consider a large corporate website with many 

# [Web Gateway](docs/WebGateway/README.md)
The Web Gateway is a .Net Core website that can be hosted in a container or standalone and is the main entry point for navigation. These can work in tandem and can proxy calls to any number of *microsites*

# [Resolver](docs/ServiceDiscovery/README.md)
The resolver acts as an advanced DNS server that allows for *microsite* discovery, route reservation and additional information that allows for advanced gateway operations. The resolver can be installed as a sidecar .NET core service,or as an onboard sidecar which is hosted within the same process.

The resolver holds both resolution information and additional metadata that can be used for content based routing.

# [MicroSite](docs/Microsites/README.md)
A *microsite* is a single purpose, standalone website that services anything from a single page, an application or a whole site. This is routed based on the first segment in the url and must be unique unless additional resolution parameters are used via custom middleware in the gateway. 

# [Collaborative MicroSite](docs/Microsites/Collaborative/README.md)
These *microsites* extend the basic *microsite* but include a remote session module and a distributed View Engine for loading templates from a singular location

