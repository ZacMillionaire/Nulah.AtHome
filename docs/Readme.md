# Concept
At its core AtHome is simply a system I've designed for myself for my own needs. You're more than welcome to use it yourself (and documentation will exist to do so), but there are no guarantees that it'll do anything you will want. In saying that if there _is_ something you want to do feel free to go ahead and contribute your own feature

It's primarily designed to run within docker, but because I'm a sane person, I develop it entirely externally _to_ docker, so you can absolutely run the application and any workers independently and they'll work as expected. Docker is only provided as a means to restrict the application to it's own little virtual space and orchestrate it's various components and configurations without tedium.

# Technology
Currently AtHome uses Blazor, with Postgres as a database and entity framework as my ORM of choice. Postgres must have the Postgis extension as we use it for various GIS functionality that Postgis just handles so so much better.

Additionally we also use RabbitMQ for messaging, and Marten for certain locations where a more flexible document level structure is preferred.

# Telemetry
AtHome also uses OpenTelemetry for logging, tracing and metrics. Most of this is out of the box defaults, and I don't care enough about what you do to set it up to send telemetry to anything I control.

I use it during development to highlight any weirdness during features via tracing, and assist with runtime debugging to avoid any IDE level debugging that might hide issues eg: async methods where observing the result in an attached debugger prevents the issue from happening.

# Contributing
While this is designed for me, contributions are of course welcome. I don't care what you do for your own forks (obviously), if you wish to contribute directly to here then you'll need to follow a few guidelines to have a successful PR.

## Branching
Do whatever you want until you wish to merge into `develop`. 

### Features
Branch from the current state of `develop`, rebase/merge/squash merge periodically to ensure your branch is kept up to date.

Your branch _must_ start with `feature/[descriptive-name]`. If your branch is related to a feature-request issue, it _must_ follow `feature/[feature_request_issue_id]-[descriptive-name]`.

### Fixes
If it's just a fix you've identified (or one that someone else has), create an issue if it does not exist, then branch from `develop` and the branch name _must_ follow the form `fix/[issue_id]-[descriptive-name]`.

If it's critical - branch from `main` and PR back into main. These PRs are generally exempt from any other process flows and will be handled exceptionally as a result. Critical fixes will have an issue id which must be included in the branch name and _must_ follow the form `hotfix/[issue_id]-[descriptive-name]`. 

## Pull Requests
Pull requests _must_ always be a squash merge. I don't care about the entire history of your branch and neither should anyone else. If you're creating a PR then the only thing that matters once the PR has been approved is the result of your work.

Unsurprisingly all tests have to pass in a PR. If you're adding a new feature then you must make a reasonable attempt to create tests around what you have implemented and ensure you aren't breaking any existing functionality. Reasonable doesn't mean, "your tests introduce a new entity and new APIs then test that the entity is created", reasonable means, "your tests cover entities being created with a happy path, and must test as many unhappy paths as possible". If your feature is too difficult to write tests for then your feature probably needs to go back to the design phase to identify if it's actually solving a problem, or if you've defined your problem well enough.

## Code Quality
"Don't write bad code".

Which is to say: your code must be maintainable, programming to solve a problem can often result in very abstract solutions, however if your code is inscrutable to everyone but you I'll reject it. Document everything you do, comment as if you're explaining to an MBA, and don't assume that everyone will understand your weird bespoke reflection into runtime types to set private variables on a third party library (as an example).

If you have the question of, "Where do I document things", your answer is within this very folder.

I have high standards on my own code even if it looks garbage, so if something doesn't make sense you challenge it by raising a code-quality issue and we start a discussion. If someone is challenging code with an issue it means it's not clear enough at a high level, not that someone is taking personal offense to your code, even though it might feel like that.

# Governance
As creator and maintainer of this repository I undemocratically have unilateral decisions to implement, remove, or support anything as I see fit. I'm under no obligation to fix things or respond to issues, though I will aim to do so for as long as this sentence remains within this document.

I'm also not some weird anti-social hermit so if you've got opinions with something, start a discussion so everyone can be involved.