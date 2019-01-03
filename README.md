# forum-app
# Documentation
**Angular 6 Forum** is a simple forum where you can make posts to a given category.
* Front-end - SPA (Single-Page-Application) using **Angular 6**

Users can register, login and logout in the forum app. Unauthenticated users can only view posts.
On the other hand, authenticated users can create, edit and delete their posts and comments.
Administrators can add, edit, delete categories, ban and unban users, and delete other users' posts and comments.
# App Build Setup
```
# install dependencies
1. npm install
# run project in dev
2. ng serve
# navigate your browser to http://localhost:4200
```

# Functionality
3 types of roles - Guests, Authenticated Users (logged in), Administrators
```
Guests
    can see home page
    can register
    can login
    can view posts
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
```

```
Administrator:
    all the users' functionality
    can ban users
    can unban users
    can add categories
    can edit categories
    can delete categories
```

```
Views:
   Home Page: Shows two images with the 6 latest posts and available categories. ('/home')
   Posts Page: Shows all posts in the forum, (can be selected by a given category). ('/posts')
   Create Post Page: Shows a form to add a post to a given category. ('/posts/create')
   Edit Post Page: Shows a form to edit a given post. ('/posts/edit/:id')
   Post Details Page: Shows the post with its category and comments. ('/posts/details/:id')
   Add Comment Page: Shows a form to add comment to a particular post ('/comments/create/:postId')
   Admin: Shows categories list (create, edit, delete) and users list (user information, functionality to ban/unban user). ('/admin')
Modal Dialogs:
  Login
  Register
  Edit Comment
  Edit Category
```

Post has a title, an author, a description, a category, date created and comments.

Comment has an author, a description and date created.

Banned users cannot create, edit, delete neither their posts nor their comments.
	
# Future Guidelines
	Users can change password
  	Users upload their image
  	User profile section
