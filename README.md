# Epam.GraphQL
![Licence](https://img.shields.io/github/license/epam/epam-graphql.svg)

![Size](https://img.shields.io/github/repo-size/epam/epam-graphql.svg)
![Activity](https://img.shields.io/github/commit-activity/w/epam/epam-graphql)
![Activity](https://img.shields.io/github/commit-activity/m/epam/epam-graphql)
![Activity](https://img.shields.io/github/commit-activity/y/epam/epam-graphql)

## Overview

**Epam.GraphQL** is a set of .NET libraries which provides high-level way for building GraphQL APIs with a few lines of code, including (but not limited to) CRUD, batching, complex sorting and filtering, pagination.
We have built **Epam.GraphQL** on top of [GraphQL.NET](https://github.com/graphql-dotnet/graphql-dotnet/) to simplify developing GraphQL API layer:
  * It used by dozen EPAM internal applications, battle-tested on complex tasks
  * Highly declarative; can be seen as Low-Code platform done right
  * Serves as architecture backbone for the whole app
  * Makes APIs aligned and metadata-rich – allowing future features like admin UI generation
 
## Features

* Supports EntityFrameworkCore (e.g. querying only necessary fields from the database, disable change tracking when needed)
* Declarative CRUD (but you can write your own manual mutations as well)
* Gracefully solves [N+1 problem](https://medium.com/the-marcy-lab-school/what-is-the-n-1-problem-in-graphql-dd4921cb3c1a) by nested entities query batching with zero boilerplate code (but you can write your batches by yourself)
* [Relay connections](https://relay.dev/graphql/connections.htm) out-of-the-box
* Declarative filtering and search
* Declarative sorting
* Entity automapping
* Master-details relationship between entities with a few lines of code
* Security
* ... and many more

## Documentation

* [Get Started](docs/01-get-started.md)
* Entity Framework Core Usage
  * [Automapping](docs/02-ef-core/01.automapping.md)

