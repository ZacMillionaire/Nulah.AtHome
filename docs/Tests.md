# Best Practices
(For this repository)

If you're implementing new entities or entity managers, your tests _must_ use the test database. This test database is always completely deleted and all migrations are applied. This does mean that if a migration would break on an existing database, it would not be caught in tests. This is by design and is something that will be picked up before code is pulled into `main` or `develop` and documented in any release notes.

## Unit Tests
The code written for these has a lot of boilerplate that is honestly a bit too complicated for my liking and I'm the one who wrote it.

Generally you can (and should) ignore everything within the `/Helpers` directory.

For a reference the tests under `/BasicEvents` should provide a solid baseline on how to write tests.

### Un/Happy Path Testing
Don't just test the happy path. If all your testing is that the correct thing happens given perfect data, you aren't testing. Write tests that test that things _fail_ in your implementations to cover error handling or exceptions.

If your implementation throws user exceptions you must be able to trigger these in tests. If your code _can_ throw exceptions (such as file reads or http requests), and you handle them in a try/catch, you _must_ also test for these cases.

Unhandled exceptions are on a case by case basis on whether they should be covered by a test, but if an unhandled exception is raised often enough then you should probably upgrade it to a handled exception.

## Random Data
Where possible, add some form of randomness to your test values unless an exact value is needed, eg: you have a test that creates a valid entity, then retrieves that entity by a string field or similar that you have to control.

We currently do not use any library to randomly create entities, though this may change in the future so it's more of a case by case basis.

Generally though any `[Fact]` tests use non-random values as these are certainties (they're facts!). These tests should assert a single part of an implementation, but should not validate things like, "an entity gets an Id" in a single test. Assert everything or don't write a test at all.