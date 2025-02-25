

    #For example odata-filter
GET /api/reminders/odata-filter?$filter=
    contains(Title, 'Meeting') 
    and contains(Description, 'urgent') 
    and ReminderDateTime gt 2024-01-01T00:00:00Z

    #For siqnalr connect testing
    https://localhost:7038/index.html
