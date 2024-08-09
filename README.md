# Transaction Management API

## Unit Test
Before unit tests, you need to create a database named "TestTransactionDB"

## Overview

This API service is designed to manage transactions by providing functionality to import, retrieve, and export transaction data. The service allows users to import transaction data from CSV files, retrieve transactions based on date ranges, and export transaction data to Excel files.

## Features

1. **Import Transactions from CSV**
   - Allows users to upload a CSV file containing transaction data. The API processes the file and updates the database with new or modified transactions based on the `transaction_id` in the CSV file.

2. **Retrieve Transactions by Date Range**
   - Provides an endpoint to retrieve transactions within a specified date range.

3. **Export Transactions to Excel**
   - Allows users to export transactions within a specified date range to an Excel file.

4. **Retrieve Transactions for Current User Time Zone**
   - Retrieves transactions for a date range based on the time zone of the current user.

5. **Retrieve Transactions for Client Time Zones**
   - Retrieves transactions for a date range based on the time zones of clients, as stored in the transaction records.

6. **Retrieve Transactions for January 2024**
   - Provides functionality to retrieve transactions for January 2024 based on client time zones.

## Functional Requirements

1. **CSV File Import**
   - Process CSV files and add or update transactions in the database based on `transaction_id`.
   - If a `transaction_id` is not present in the database, add a new record. If present, update the transaction status.

2. **Time Zone Conversion**
   - Use libraries or online services to determine time zones based on location coordinates.
   - Transactions should be filtered and retrieved based on the time zone of the current user or client time zones.

3. **Export to Excel**
   - Export transaction data to an Excel file with user-selectable columns.

4. **Date Range Queries**
   - Retrieve transactions within a specified date range considering the time zone of the user or client.

5. **January 2024 Transactions**
   - Retrieve and export transactions for January 2024 based on client time zones.
  # Transaction Management API

## Overview

This API service is designed to manage transactions by providing functionality to import, retrieve, and export transaction data. The service allows users to import transaction data from CSV files, retrieve transactions based on date ranges, and export transaction data to Excel files.

## Features

1. **Import Transactions from CSV**
2. **Get List of Transactions by date range**
3. **Get a list of transactions by date range, taking into account customer zones**
4. **Get List of Transactions for January 2024**
5. **Export Transactions to Excel**
   

