# Postman Setup Guide

## Step 1: Download the Collection and Environment Files

### Download the Collection
1. Open the Postman Collection URL: **User Management Service Collection**
2. Click on the **Raw** button to view the raw JSON content.
3. Right-click and select **Save As** to download the file as `User_Management_Service.postman_collection.json`.

### Download the Environment
1. Open the Postman Environment URL: **User Management Service Environment**
2. Click on the **Raw** button, then **Save As** to download it as `User_Management_Service_Dev.postman_environment.json`.

## Step 2: Import into Postman

1. Open **Postman** on your local system.
2. Click on the **Import** button (top-left corner).
3. Select **Upload Files** and choose the `User_Management_Service.postman_collection.json` file.
4. Go to the **Environments** tab in Postman (click the gear icon ⚙️ in the top-right corner).
5. Click **Import** and select the `User_Management_Service_Dev.postman_environment.json` file.

## Step 3: Set the Environment

1. Click on the **gear icon** ⚙️ in the top-right corner.
2. Select **User Management Service Dev** from the environment dropdown.
3. Click **Enable** to activate the environment.

## Step 4: Run the Collection

1. Navigate to the **Collections** tab in Postman.
2. Find **User Management Service** in the list of collections.
3. Click on it and select **Run** (opens Postman Collection Runner).
4. Choose **All Requests** or specific requests as needed.
5. Click **Run** and check the response.

## Step 5: Verify API Responses

- Ensure that all API endpoints return the expected responses.
- If needed, update environment variables like `baseUrl`, `apiKey`, or authentication tokens before running.

