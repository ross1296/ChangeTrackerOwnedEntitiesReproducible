
# Readme

Goal:

To create a history mechanism to capture the changes made to properties in domain aggregates. For example, if the Customer's name changes, then we produce a history of:
```
Forename changed from 'John' to 'Jane'
Surname changed from 'Doe' to 'Smith'
```
In an ideal world, each table would have its own history table behind it, but for the sake of simplicity, we can avoid that.

Steps to reproduce:

- Run docker-compose
- Debug the solution
- Place a breakpoint on UnitOfWork.cs:134
- Go to `Customer.http` in the project root and execute the POST method

Executing the POST method will trigger the breakpoint, and the following is inside of the `AuditLogs` List<>:

```json
[
  {
    "Id": 0,
    "UserId": "e1dd41bf-f47a-4b2c-82a0-1905b18b1a2b",
    "Type": "Added",
    "TableName": "Customer",
    "DateTime": "2024-10-13T13:08:58.5455834Z",
    "OldValues": null,
    "NewValues": "{\"AltTelephoneNumber\":\"07000000000\",\"CreatedBy\":\"879f67ca-06ff-4b8f-b363-7a154d546c1b\",\"CreatedOnUtc\":\"2024-10-13T13:08:58.5249051Z\",\"DeliveryInstructions\":\"Leave at the front door\",\"Email\":\"john.doe@example.com\",\"Forename\":\"John\",\"MarketingOptIn\":true,\"MobilePhoneNumber\":\"07000000000\",\"ModifiedBy\":null,\"ModifiedOnUtc\":null,\"SmsMarketing\":true,\"Surname\":\"Doe\",\"Title\":\"Mr\",\"WelcomeCallComplete\":false,\"customers.AddressLine1\":\"456 Elm St\",\"customers.AddressLine2\":null,\"customers.City\":\"Othertown\",\"customers.County\":\"State\",\"customers.Postcode\":\"67890\"}",
    "AffectedColumns": "[\"AltTelephoneNumber\",\"CreatedBy\",\"CreatedOnUtc\",\"DeliveryInstructions\",\"Email\",\"Forename\",\"MarketingOptIn\",\"MobilePhoneNumber\",\"ModifiedBy\",\"ModifiedOnUtc\",\"SmsMarketing\",\"Surname\",\"Title\",\"WelcomeCallComplete\",\"customers.AddressLine1\",\"customers.AddressLine2\",\"customers.City\",\"customers.County\",\"customers.Postcode\"]",
    "PrimaryKey": "{\"Id\":\"3a5b4ffb-8a7c-4f34-bad7-1428450b87bc\"}"
  },
  {
    "Id": 0,
    "UserId": "c2487e23-6f9e-44e8-bcf4-ce579b728f64",
    "Type": "Added",
    "TableName": "CustomerAddress",
    "DateTime": "2024-10-13T13:08:58.545672Z",
    "OldValues": null,
    "NewValues": "{\"AddressLine1\":\"123 Main St\",\"AddressLine2\":\"Apt 4\",\"City\":\"Anytown\",\"County\":\"State\",\"Postcode\":\"12345\"}",
    "AffectedColumns": "[\"AddressLine1\",\"AddressLine2\",\"City\",\"County\",\"Postcode\"]",
    "PrimaryKey": "{\"CustomerId\":\"3a5b4ffb-8a7c-4f34-bad7-1428450b87bc\"}"
  },
  {
    "Id": 0,
    "UserId": "f86c69d5-8342-4892-9145-afe75102ff85",
    "Type": "Added",
    "TableName": "CustomerAddress",
    "DateTime": "2024-10-13T13:08:58.5456875Z",
    "OldValues": null,
    "NewValues": "{\"AddressLine1\":\"456 Elm St\",\"AddressLine2\":null,\"City\":\"Othertown\",\"County\":\"State\",\"Postcode\":\"67890\"}",
    "AffectedColumns": "[\"AddressLine1\",\"AddressLine2\",\"City\",\"County\",\"Postcode\"]",
    "PrimaryKey": "{\"CustomerId\":\"3a5b4ffb-8a7c-4f34-bad7-1428450b87bc\"}"
  }
]
```

The first issue of note is that the Customer aggregate has two properties of type CustomerAddress on it, and so the JSON payload has two instances of `customers.AddressLine1`, the same goes for the rest of the CustomerAddress properties.

Continuing the steps to reproduce:

- Navigate back to `Customer.http`
- Update the parameter on line 36
- Execute the PATCH method
- Note that we are updating many of the properties and both the `CorrespondenceAddress` and `DeliveryAddress`

Here is the `AuditLogs` List<> after executing the PATCH method:

```json
[
  {
    "Id": 0,
    "UserId": "07964c3d-e82d-4d85-aad0-d1bc1c95e986",
    "Type": "Modified",
    "TableName": "Customer",
    "DateTime": "2024-10-13T13:19:28.0217568Z",
    "OldValues": "{\"AltTelephoneNumber\":\"07000000000\",\"DeliveryInstructions\":\"Leave at the front door\",\"Email\":\"john.doe@example.com\",\"MarketingOptIn\":true,\"SmsMarketing\":true,\"WelcomeCallComplete\":false}",
    "NewValues": "{\"AltTelephoneNumber\":null,\"DeliveryInstructions\":null,\"Email\":\"john.doe.updated@example.com\",\"MarketingOptIn\":false,\"SmsMarketing\":false,\"WelcomeCallComplete\":true}",
    "AffectedColumns": "[\"AltTelephoneNumber\",\"DeliveryInstructions\",\"Email\",\"MarketingOptIn\",\"SmsMarketing\",\"WelcomeCallComplete\"]",
    "PrimaryKey": "{\"Id\":\"3a5b4ffb-8a7c-4f34-bad7-1428450b87bc\"}"
  },
  {
    "Id": 0,
    "UserId": "f8cfd39e-75c2-44b3-85c9-6d36aabbc1ac",
    "Type": "Added",
    "TableName": "CustomerAddress",
    "DateTime": "2024-10-13T13:19:28.0218563Z",
    "OldValues": null,
    "NewValues": "{\"AddressLine1\":\"789 Oak St\",\"AddressLine2\":\"Suite 10\",\"City\":\"Newtown\",\"County\":\"State\",\"Postcode\":\"54321\"}",
    "AffectedColumns": "[\"AddressLine1\",\"AddressLine2\",\"City\",\"County\",\"Postcode\"]",
    "PrimaryKey": "{\"CustomerId\":\"3a5b4ffb-8a7c-4f34-bad7-1428450b87bc\"}"
  },
  {
    "Id": 0,
    "UserId": "30e36daf-08d6-4cb8-b748-4c412443d0c7",
    "Type": "Deleted",
    "TableName": "CustomerAddress",
    "DateTime": "2024-10-13T13:19:28.0218642Z",
    "OldValues": "{\"AddressLine1\":\"123 Main St\",\"AddressLine2\":\"Apt 4\",\"City\":\"Anytown\",\"County\":\"State\",\"Postcode\":\"12345\"}",
    "NewValues": null,
    "AffectedColumns": "[\"AddressLine1\",\"AddressLine2\",\"City\",\"County\",\"Postcode\"]",
    "PrimaryKey": "{\"CustomerId\":\"3a5b4ffb-8a7c-4f34-bad7-1428450b87bc\"}"
  },
  {
    "Id": 0,
    "UserId": "1c839d1e-6148-436e-bc8c-53bce56d112b",
    "Type": "Deleted",
    "TableName": "CustomerAddress",
    "DateTime": "2024-10-13T13:19:28.0218697Z",
    "OldValues": "{\"AddressLine1\":\"456 Elm St\",\"AddressLine2\":null,\"City\":\"Othertown\",\"County\":\"State\",\"Postcode\":\"67890\"}",
    "NewValues": null,
    "AffectedColumns": "[\"AddressLine1\",\"AddressLine2\",\"City\",\"County\",\"Postcode\"]",
    "PrimaryKey": "{\"CustomerId\":\"3a5b4ffb-8a7c-4f34-bad7-1428450b87bc\"}"
  }
]
```

The `CorrespondanceAddress` and `DeliveryAddress` do not show up in the AuditLogs for the PATCH, and this is because, I believe, UnitOfWork:152-162.
```csharp
else if (parentEntry.State == EntityState.Modified)
{
    object? originalValue = property.GetValue(ownedEntry);
    object? currentValue = property.GetValue(ownedEntry);

    if (!Equals(originalValue, currentValue))
    {
        parentAuditEntry.OldValues[propertyName] = originalValue;
        parentAuditEntry.NewValues[propertyName] = currentValue;
    }
}
```
`originalValue` and `currentValue` will always be equal, as they are both assigned from `property.GetValue(ownedEntry).

