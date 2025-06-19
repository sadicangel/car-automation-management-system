# Car Automation Management System

The structure of the code is inspired in vertical slice architecture, so I've grouped components by their use case.
I believe this makes it easier to locate and update existing features as all related code is close.
It's also great for enabling development of new features in parallel, as there should be no dependencies between the features.

The main projects are CarAutomation.Domain, CarAutomation.WebApi and CarAutomation.WebApi.Tests.
The other projects are used to run the project locally using Aspire + Docker.

Originally, I had everything in the same project but decided to add the Domain project as a sign that I would evolve the code base into keeping entities (like auction and vehicle) and actions that modify them in a different layer.

The solution uses minial apis, where validation is done by both an endpoint filter and the endpoint handler itself.

### Auctions and Vehicles
Since only Auction depends on Vehicle, I decided to split the two concepts. Managing inventory and managing auctions can be mostly separate business. Auction operations only need to interact with vehicles when starting an auction. I could see these growing apart (maybe into different services).

### Vehicle Inheritance
For Vehicle, I decided to use inheritance to represent each of the concrete 4 types of vehicles.
I'm not so sure about this decision because it would make sense to me that SUVs also specify number of doors or Sedans/Hatchbacks specify the number of seats. I was very close to using the same class with an enum to represent all types, having these properties as optional.  
In the end, I opted for creating 4 concrete types because it's a bit better in ensuring that domain models are valid.

### Libraries
I opted against using third party libraries for the challenge to keep the code base less opinionated on that matter.
I do believe some libraries would bring a lot of value by making things less verbose or outright doing said things.

### Testing
I ended up focusing on integration tests. The tests are set up in a way that is very close to how the application would work in a production environment. My idea was for tests to focus more on how the feature should work than how it's implemented.
