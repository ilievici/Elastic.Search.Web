# Elastic.Search.Web

To be considered:
- Have CRUD operation – PaymentsController
  - GetById
  - Delete by ID
  - Update – whole object (partial update was not tested, but it is possibly to be done)
  - Insert – one payment
  - BulkInsert – a list of payments

- CreateRandom – use this method to add random payments into index.
- SearchPaymentsController does search by a list criteria (see AdvancedSearchCriteria class)
- Test from Swagger: http://localhost:55959/swagger or from Postman/Insomnia
- ES engine hosted as Windows Service – local: http://localhost:9201 (this is not default port) – 5 min
- Set necessary configurations for ES in appsettings.json file.
- Index name is may be passed in request header key:clientId, but for test purpose it is set to “cprofile_nextgen” (IndexConfigProvider class).
