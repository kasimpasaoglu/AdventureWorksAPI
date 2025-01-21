
# WebAPI Project

This project is a **Web API** application developed for an demo e-commerce platform. It provides various endpoints to manage users, products, categories, colors, carts, and more.

## 🚀 Technologies Used

- Developed using **C#** and **.NET 9.0**.
- **Entity Framework Core** for database operations.
- Follows **RESTful API** principles.
- Works with the **[AdventureWorks](https://learn.microsoft.com/en-us/sql/samples/adventureworks-install-configure?view=sql-server-ver16&tabs=ssms) MsSQL** database. *(With some customizations)*
- Designed with **Dependency Injection (DI)** for flexibility.

## Packages Used

- **[AutoMapper](https://www.nuget.org/packages/AutoMapper)**
- **[Swagger](https://www.nuget.org/packages/Swashbuckle.AspNetCore.Swagger)**
- **[JwtBearer](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer)**

---

## 📌 API Endpoints

- Detailed documentation is available at the `/swagger` endpoint.

### User Management

- **PUT /api/user**: Creates a new user.
- **POST /api/user**: Logs in a user.
- **DELETE /api/user**: Deletes a user account.
- **PATCH /api/user**: Updates user information.
- **GET /api/user/states**: Retrieves the list of all available countries & states.
- **GET /api/user/addresstype**: Retrieves the list of all available address types.

### Product Management

- **GET /api/product/{id}**: Retrieves a specific product.
- **GET /api/product/recent**: Retrieves recently added products.
- **POST /api/product**: Retrieves a list of products based on given filters.

### Color Menagement

- **GET /api/color/{categoryId}/{subCategoryId}**: Retrieves all available colors list based on given categoryId and subCategoryId.

### Category Management

- **GET /api/category**: Retrieves all categories.
- **GET /api/category/{id}**: Retrieves subcategories of a specific category.

### Cart Operations

- **GET /api/cart**: Lists the user's cart.
- **PUT /api/cart**: Adds a product to the cart.
- **DELETE /api/cart**: Removes a product from the cart.

---

## 🛠️ Setup

1. Clone this repository:

   ```bash
   git clone https://github.com/kasimpasaoglu/AdventureWorksAPI.git
   cd AdventureWorksAPI
   ```

2. Install dependencies:

   ```bash
   dotnet restore
   ```

3. Run the project:

   ```bash
   dotnet run
   ```

4. Test the API using `/swagger` or a client like Postman.

---

## 🤝 Contributing

If you want to contribute to this project, feel free to open a **Pull Request** or an **Issue**.
