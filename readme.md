# Airport Parking

## Installation instructions

### Requirements

- [Dotnet core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet/3.1)

### Running the code

- Open the AirportParking.sln file in a compatible IDE such as Visual Studio
- Ensure Nuget Packages are downloaded
- Run the application

## User Guide

Airport Parking is a REST API to allow a user to obtain a recommended parking space for an aircraft, and to book a parking space. Swagger UI has been integrated to allow easier usage of the API.

When running the application, your browser should automatically open the website, if not you should be able to visit the UI at https://localhost:5001/

From here, each available endpoint on the API should be visible. The Aircraft Types and Parking Spaces controllers offer CRUD operations for testing purposed. The Booking controller offers the recommend and book endpoints. Each endpoint can be expanded by clicking, and can be executed using the Try it out button (followed by the execute button after entering any required inputs).

### Recommend

This endpoint allows the user to request a free parking space ID for a given aircraft type, and for a set amount of days and hours. After a recommendation the parking space is still free for other users to book.

### Book

This endpoint allows the user to book an appointment for a given parking space ID, aircraft type and a set amount of days and hours. After booking an appointment the parking space will be unavailable for other users to book.

## Other Notes

### Unit tests

Unit tests are written in NUnit. To run them ensure your IDE supports NUnit or you have an appropriate plugin installed. If you are using Visual Studio 2019 you may need to add a Nuget instead of using a plugin.