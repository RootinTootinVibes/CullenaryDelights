# Cullenary Delights

## Description
An ASP.NET Core MVC application for creating, managing, and discovering cooking recipes. Users can register, log in, and perform CRUD operations on their own recipes, including dynamic entry of steps and ingredients. Additional features include search by title or ingredient, user dashboards, and a comments & ratings system.

## Features

- **User Authentication: Registration, login, password complexity rules, and secure password hashing via ASP.NET Identity.
- **Recipe CRUD: Authenticated users can create, read, update, and delete their own recipes.
- **Dynamic Steps & Ingredients: Add any number of steps or ingredients in the form, with JavaScript-driven "+ Add Step" and "+ Add Ingredient" buttons, and seamless removal/re-indexing.
- **Search: Filter recipes by title or ingredient name using PostgreSQL ILIKE and LINQ Any() queries.
- **User Dashboard: View a list of your own recipes, quick links to edit, delete, or view details, and a button to create new recipes.
- **Comments & Ratings: Leave textual comments and a 1–5 star rating on any recipe. Overall rating is averaged and displayed on the recipe details page.
- **Responsive Layout: Clean form layout with CSS grid or flexbox, limited input widths, and a sticky footer pattern.

## Prerequisites

- **.NET SDK (6.0 or later)
- **PostgreSQL (12+)

## Getting Started

1. Clone the Repository

- **git clone https://github.com/yourusername/RecipeWebsite.git
- **cd RecipeWebsite

2. Configure the Database

- **Create a PostgreSQL database (e.g., RecipeDb).
- **In appsettings.json, update the connection string under ConnectionStrings:DefaultConnection:
- **"DefaultConnection": "Host=localhost;Database=RecipeDb;Username=youruser;Password=yourpassword"

3. Apply EF Core Migrations

- **dotnet ef migrations add InitialCreate
- **dotnet ef database update
- **This will create the necessary tables: Users, Recipes, Steps, RecipeIngredients, and CommentsAndRatings.

4. Run the Application
- **dotnet run
- **Open your browser at https://localhost:5111 (or the URL shown in the console).

## Project Structure

- **/Controllers          # MVC controllers
- **/Data                 # EF Core DbContext and migrations
- **/Models               # Entity classes (User, Recipe, Step, Ingredient, CommentAndRating)
- **/Views                # Razor views (_Layout, Recipe CRUD, Details, Dashboard, etc.)
- **/wwwroot              # Static assets (CSS, JS)
- **README.md             # This file

## Configuration

- **CSS Layout: Global styles in site.css use CSS Grid/Flexbox for form alignment, and the margin-top: auto sticky footer pattern.
- **Scripts: JavaScript in Razor views handles dynamic addition/removal of steps and ingredients, reindexing to support model binding.

## Usage

- **Register a new user.
- **Create a recipe: fill in name, times, add steps/ingredients, and save.
- **Search recipes by keyword in the navigation or /Recipe/Index.
- **Comment & Rate on any recipe via the details page.
- **Manage your recipes in the Dashboard.

## Contributing

- **Fork the repository.
- **Create a feature branch (git checkout -b feature/YourFeature).
- **Commit your changes (git commit -m 'Add feature').
- **Push to the branch (git push origin feature/YourFeature).
- **Open a Pull Request.

## Link to Project

- **https://github.com/RootinTootinVibes/CullenaryDelights

## License

MIT License © 2025 Your Name

