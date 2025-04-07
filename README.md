## Introduction
This **Workday Calendar API** provides a powerful solution for calculating dates based on working days, considering weekends, holidays, and working hours. It helps businesses manage production schedules, shipment timelines, and other time-sensitive tasks that rely on working day calculations.

## Core Features
- **Working Days Calculation**: Add or subtract working days to/from a given date.
- **Holiday Exclusions**: Define full holidays to be excluded from the calculation of working days.
- **Partial Working Days**: Handle fractional working days (e.g., add or subtract part of a working day like 0.25 or 0.5 days).
- **Working Hours Precision**: Perform calculations using exact working hours (e.g., from 08:00 AM to 04:00 PM).
- **Recurring Holidays**: Support for recurring holidays (e.g., every year on the same date).
- **Single Holidays**: Define one-time holidays.

## API Endpoints

1. POST `/api/v1/workdaycalculate/calculate-date`
**Description**: This endpoint adds or subtracts the specified number of working days from a given start datetime, considering the specified holidays and working hours.

Request Body sample:

{
  "startDate": "2004-05-24T18:05",
  "workingDays": -5.5,
  "workStartHour": "08:00",
  "workEndHour": "16:00"
}
Response sample:

{
  "calculated_datetime": "2004-05-19T10:00"
}

2. POST  `/api/v1/holiday`
**Description**:: Adds a new holiday to the system.

Request Body sample:

{
  "name": "HolidayName",
  "month": 12,
  "day": 31,
  "year": 2004,
  "isRecurring": false,
  "isActive": true
}
Response sample:

{
  "id": "3ec05db9-b137-4fa5-873a-fda9e8ee94da",
  "name": "HolidayName",
  "month": 12,
  "day": 31,
  "year": 2004,
  "isRecurring": false,
  "isActive": true
}
3. GET `/api/v1/holiday/all`
**Description**: Retrieves a list of all holidays with an option to search by name and a paging option, including recurring and one-time holidays.

Request sample:

/api/v1/holiday/all?pageNumber=1&pageSize=10&search=May
Response sample:

{
  "holidays": [
    {
      "id": "b58474e9-8f01-49c1-8eae-3662c3e9e300",
      "name": "May27",
      "month": 5,
      "day": 27,
      "year": 2004,
      "isRecurring": false,
      "isActive": true
    },
    {
      "id": "4d578836-e833-4673-9386-a0c08278c949",
      "name": "May17",
      "month": 5,
      "day": 17,
      "year": null,
      "isRecurring": true,
      "isActive": true
    }
  ],
  "totalCount": 2
}
4. DELETE `/api/v1/holiday/{id}`
**Description**: Deletes a holiday by its ID. Here, the ID is the unique identifier of the holiday to be deleted, which is a GUID.

## Setting Up the Project Locally

### 1. Update the Connection String

Application uses code first approch,Before running the project locally, you need to update the **connection string** in the `appsettings.json` file.

In the `appsettings.json` file, locate the following section:

"ConnectionStrings": {
  "DbConnectionString": "Server=(localdb)\\MSSQLLocalDB;Database=WorkdayCalendar;Trusted_Connection=True;"
}
Update the DbConnectionString with your actual database server and credentials. For example:

"ConnectionStrings": {
  "DbConnectionString": "Server=your_server_name;Database=WorkdayCalendar;User Id=your_user;Password=your_password;"
}
Make sure to replace your_server_name, your_user, and your_password (or windows authentication) with your actual database details.
### 2. Run the Application
Once you have updated the connection string, you can proceed to run the application locally. You can use Visual Studio to build and run the project.

Open the solution in Visual Studio.

Build the solution to restore any required NuGet packages.

Run the project locally using Visual Studio's built-in tools.

The application should now be ready to use with the updated connection string.