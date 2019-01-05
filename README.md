# forum-app
# Documentation
**Forum App** is a simple forum where you can make posts to a given category.
* Front-end - SPA (Single-Page-Application) using **Angular 6**
* Back-end - .NET Core Web API Server (.NET Core 2.2)

# Functionality
3 types of roles - Guests, Authenticated Users (logged in), Administrators

Users can register, login and logout in the forum app. Unauthenticated users can only view posts.
On the other hand, authenticated users can create, edit and delete their posts and comments.
Administrators can add, edit, delete categories, ban and unban users, and delete other users' posts and comments.
```
Guests
    can see home page
    can register
    can login
    can view posts
    can contact administrators
```

```
Logged in Users
    all the guests' functionality
    can create posts
    can edit their posts
    can delete their posts
    can create comments on theirs and others' posts
    can edit their comments
    can delete their comments
    can see their login history
```

```
Administrator
    all the users' functionality
    can ban users
    can unban users
    can add categories
    can edit categories
    can delete categories
```

# API

## Models
```
User
Login Information
Category
Comment
Contact Us
Event Log
Post
```
  
## Tests
```
Unit Tests - 46
Integration Tests - 91
```
  
## SignalR
Simple **NotifyHub** ('/api/notify') notifying all clients upon posts and categories addition and deletion.

# Front-end

## App Build Setup
```
# navigate to front-end folder
1. cd <your-dir>/forum-app/forum-project
# install dependencies
2. npm install
# run project in dev
3. ng serve
# navigate your browser to http://localhost:4200
```
## Views
```
Views:
   Home Page: Shows two images with the 6 latest posts and available categories. ('/home')
   Posts Page: Shows all posts in the forum, (can be selected by a given category). ('/posts')
   Create Post Page: Shows a form to add a post to a given category. ('/posts/create')
   Edit Post Page: Shows a form to edit a given post. ('/posts/edit/:id')
   Post Details Page: Shows the post with its category and comments. ('/posts/details/:id')
   Add Comment Page: Shows a form to add comment to a particular post ('/comments/create/:postId')
   Admin: Shows categories list (create, edit, delete) and users list (user information, functionality to ban/unban user). ('/admin')
   MyProfile: Shows users' login history
   Contact-Us: Drop a message to site's administrators
Modal Dialogs:
  Login
  Register
  Edit Comment
  Edit Category
```
	
# Future Guidelines
    Users can change password
    Users upload their image
    Users additional information (first name, last name, address)
    Admin be able to answer contact forms via emails
