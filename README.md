# MyBooks

## :books: Introduction
MyBooks is a simple **ASP.NET MVC web application**. Its mission is to **manage books**, being able to obtain them from an external service and **organize them in lists for quick reference**. The interface is intended for a social network of readers and writers that we will not implement in this project.

The functionality of MyBooks is simple, manage books, but complex enough to require the implementation of the main features of .NET Framework. **This project intends to serve as an example of implementation of all these characteristics**, which are listed in *Main Code Features*.


In [this video](https://youtu.be/IY-sWbTDODk) you can see how the web application works. 

## :rocket: How to run

* Download **MyBooks** from [GitHub](https://www.youtube.com/redirect?event=video_description&v=IY-sWbTDODk&redir_token=xOQl5udh0Hhgh9XFjctGG69uYAd8MTU4NDU2MTYzNkAxNTg0NDc1MjM2&q=https%3A%2F%2Fgithub.com%2Fdanielgbodon%2Fmybooksdemo.git)
* Download and install [Microsoft Visual Studio](https://docs.microsoft.com/en-us/visualstudio/install/install-visual-studio)
* Create a Google Book API key from [Google Developers Console](https://developers.google.com/books/docs/v1/using#APIKey)
* Execute  *MyBooks.sln*
* Add Google Book API key in *appsettings.json* file:

```
"GoogleBooksAPIKey": "ADD_YOUR_GOOGLE_BOOK_API_KEY_HERE"
```

* Set up database executing this commands in the *packages administration console*.

```
Add-Migration InitialCreate
Update-Database
```

* Run the project

## :computer: Main code features

* Use of **interfaces** and **dependency injections** for avoid [*glue code*](https://ardalis.com/new-is-glue).
* **Entity Framework** with custom behavior. [*Go to code*](https://github.com/danielgbodon/mybooksdemo/blob/master/MyBooks/Data/MyBooksContext.cs)
* **ASP.NET Identity** for authentication.
* Role based **authorization**.
* **Middlewares** implementation. [*Go to code*](https://github.com/danielgbodon/mybooksdemo/tree/master/MyBooks/Core/Middlewares)
* **Web pages** and **API** implementation. [*Go to code*](https://github.com/danielgbodon/mybooksdemo/tree/master/MyBooks/Controllers)
* **Google Books API** controller. [*Go to code*](https://github.com/danielgbodon/mybooksdemo/blob/master/MyBooks/Core/BookServices/GoogleBookAPI.cs)
* **Custom validation** model attributes [*Go to code*](https://github.com/danielgbodon/mybooksdemo/tree/master/MyBooks/Core/Validations)
* Views implemented with **Razor pages** using *HTML5*, *CSS3*, jQUERY, *AJAX* and *Javascript* (object and classes examples).
* **Multilanguage** in controllers, models and views.
* Use of **form tokens** for avoid [CSRF attacks](https://en.wikipedia.org/wiki/Cross-site_request_forgery)
* [Altair](https://themeforest.net/item/altair-admin-material-design-uikit-template/12190654) **HTML admin template**

## :file_folder: Directory Description

* ***wwwroot***: Static files like js, css and images. The own code is in the *css* and *js* folders
* ***Controllers***: Controllers of the application, divided in Web, API, and common code.
* ***Core***: Application business logic that is independent of controllers. Here you will find the Google Book API manager, interfaces, custom middlewares, custom validations...
* ***Data***: Code related to the database
* ***Models***: Models of the application with data annotations and validations.
* ***Resources***: Resource files, currently for multilanguage support
* ***ViewModels***: Models to work with the views and the API. Similar to models but contain less data.
* ***Views***: Web interface implemented with Razor pages

## :phone: Contact

:email: [danigb27@gmail.com](mailto:danigb27@gmail.com)  
:man: [LinkedIn](https://www.linkedin.com/in/daniel-gonz%C3%A1lez-bod%C3%B3n-73987a68/)

