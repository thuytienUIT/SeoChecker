SEO Checker Application
Overview
This application is an SEO Checker Application designed to check and analyze the position of a specific URL in the top 100 search results of Bing and Google. It also utilizes a caching mechanism to optimize performance and reduce the number of requests made to the search services.

Application Structure
/Controllers: Contains the application's controllers responsible for handling HTTP requests.
/Services: Contains the core logic services, including search and result analysis.
/Handlers: Contains handlers that process search requests and return results.
/Cache: Contains classes and services related to caching for performance optimization.
/Models: Contains data models used within the application.
/Tests: Contains unit tests and integration tests for the application. 

Key Classes
BingSearchService: Service for performing searches on Bing and analyzing results.
GoogleSearchService: Service for performing searches on Google and analyzing results.
SeoCheckerController: Controller that handles user requests and returns search results.
SearchRequestHandler: Handler that processes search requests and returns results.

Setup Instructions
Clone the repository: git clone <repository-url> or extract from a .7z or .zip file.
Navigate to the project directory.
Restore dependencies: dotnet restore.
Build the solution: dotnet build.
Run the application: dotnet run --project SeoChecker.Api.
Run tests: dotnet test.
Note: This project requires .NET 7.0.

Usage
Search API
API Endpoint:

GET /api/seo/check: Checks the position of a URL in search results.
Parameters:

keywords: Search keywords. (default: "e-settlements")
url: URL to check. (default: "https://www.sympli.com.au")
Response:

Returns a list of positions of the URL in search results (e.g., "1, 10, 33") or "0" if the URL does not appear in the top 100 results.
Use Swagger to test the API once the application is running:

Swagger URL: https://localhost:5001/index.html
Detailed Explanation of the Solution
Dependency Injection
Dependency injection is used to inject dependencies into classes, making the code more flexible and testable. This is configured in the Startup.cs file.

Caching
Caching is implemented using IMemoryCache to limit the number of calls to the search engines to one per hour per search. This is configured in the SearchQueryHandler.

BackgroundWorker
The application initializes data in MemoryCache as soon as possible when the application starts, so the first search would be faster.

Logging
Logging is implemented using middleware to handle exceptions and log to the console. Notifications are provided when data is initialized in MemoryCache.

Unit Tests
Unit tests are implemented using xUnit and Moq to ensure the reliability of core functionalities. The tests cover:

SeoCheckerController: Ensures correct handling of search requests.
GoogleSearchService: Validates the search logic and parsing of Google search results.
BingSearchService: Validates the search logic and parsing of Bing search results.

Completion Time
The development and testing of this application were completed in approximately:

30th July 2024: 2 hours
31st July 2024: 2 hours
1st August 2024: 6 hours
This timeframe includes initial setup, core development, implementation of caching, and writing and running unit tests.
