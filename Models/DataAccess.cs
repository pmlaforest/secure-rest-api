using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Cryptography;
using System.Text;

namespace TodoApi.Models
{
    public static class DataAccess 
    {
        private static UserTodoContext DataContext; 
        private static bool isContextInit = false;    
        private static long connectedUserId = -1;

        public static void setStaticContext(IServiceProvider serviceProvider) {
            DataContext = new UserTodoContext(
                serviceProvider.GetRequiredService<DbContextOptions<UserTodoContext>>());
            
            //DataContext = context;
            isContextInit = true;    
        }

        public static bool isContextInitialized()
        {
            return isContextInit;
        }

         public static bool connectUserToDB(long id) 
         {
            foreach (var userx in DataContext.Users)
            {
                if (id == userx.Id1)
                {
                    connectedUserId = userx.Id1;
                    return true;
                }
            }

            connectedUserId = -1;
            return false;
        }

        public static long GetConnectedUser() 
        {
            return connectedUserId;
        }

        /* Adding User to DB */
        public static void AddUser(User userToAdd) 
        {
            if (userToAdd == null) {
                return;
            }

            String[] hashedPass = hashPassword(userToAdd.Password);
            userToAdd.Password = hashedPass[0];
            userToAdd.PasswordHash = hashedPass[1];
            
            DataContext.Users.Add(userToAdd);
            DataContext.SaveChanges();
        }

        /* Adding User's Todo to DB */
        public static bool AddUsersTodo(Todo todoToAdd) 
        {
            if (connectedUserId == -1 || todoToAdd == null) 
            {
                return false;
            }

            DataContext.Todos.Add(todoToAdd);
            DataContext.SaveChanges();
            return true;
        }

        // Get the list of users
        public static List<User> GetUsersList() 
        {
            List<User> UserList = new List<User>();

            //DataContext.Users

            //if (DataContext.Users.Count() > 0) 
            //{
                foreach (var user in DataContext.Users)
                {
                    UserList.Add(user);
                }    
            //}

            return UserList;
        }

        // Get the todo's list of a given user.
        public static List<Todo> GetTodosList(long userId) 
        {
            List<Todo> UserTodoList = new List<Todo>();

            //Verify that a user is connected.
            if (connectedUserId == -1) 
            {
                return UserTodoList;
            }

            foreach (var todo in DataContext.Todos)
            {
                if (todo.ownerId == userId)
                {
                    UserTodoList.Add(todo);
                }
            }

            return UserTodoList;
        }

        /* Get a specific todo of a user */
        public static Todo GetTodo(long todoId) 
        {

            if (connectedUserId == -1) 
            {
                return null;
            }

            foreach (var todo in DataContext.Todos)
            {
                if (todo.Id == todoId)
                {
                    return todo;
                }
            }
            return null;
        }

        /* Get a specific user */
        public static User GetUser(long userId) 
        {
            foreach (var user in DataContext.Users)
            {
                if (user.Id1 == userId)
                {
                    return user;
                }
            }
            return null;
        }

        /* Get a specific user by username */
        private static User GetUserByName(String username)
        {

            foreach (var user in DataContext.Users)
            {
                if (user.UserName.CompareTo(username) == 0)
                    return user;
            }
            return null;
        }

        /* Delete a todo of a user */ 
        public static bool DeleteTodo(long todoId)
        {
            //Verify that a user is connected
            if (connectedUserId == -1) {
                return false;
            }

            // Deleting the password
            var stud = (from s1 in DataContext.Todos
                        where s1.Id == todoId
                        select s1).FirstOrDefault();

            //Delete it from memory
            DataContext.Todos.Remove(stud);
            DataContext.SaveChanges();

            return true;
        }

        /* Delete a user */ 
        public static bool DeleteUser(long userId)
        {
            if (GetUser(userId) == null)
                return false;
            // Deleting the password
            var stud = (from s1 in DataContext.Users
                        where s1.Id1 == userId
                        select s1).FirstOrDefault();

            //Delete it from memory
            DataContext.Users.Remove(stud);
            DataContext.SaveChanges();

            return true;
        }

        /* Update a User */ 
        public static bool UpdateUser(User userToUpdate)
        {
            if (userToUpdate == null) { 
                return false;
            }

            if (UserExist(userToUpdate.Id1) == false) { 
                return false;
            }
            //Delete it from memory
            DataContext.Users.Update(userToUpdate);
            DataContext.SaveChanges();

            return true;
        }

        /* Update a User */ 
        public static bool UpdateTodo(Todo todoToUpdate)
        {
            if (todoToUpdate == null) {
                return false;
            }

            if (TodoExist(connectedUserId, todoToUpdate.Id) == false) { 
                return false;
            }
            //Delete it from memory
            DataContext.Todos.Update(todoToUpdate);
            DataContext.SaveChanges();

            return true;
        }

        /* Search the DB to find a user */ 
        public static bool UserExist(long userId) 
        {
            foreach (var user in DataContext.Users)
            {
                if (user.Id1 == userId)
                {
                    return true;
                }
            }
            return false;
        }

        /* Search the DB to find a todo of a user */ 
        public static bool TodoExist(long userId, long todoId) 
        {
            foreach (var todox in DataContext.Todos)
            {
                if (todox.Id == todoId && todox.ownerId == userId) 
                {
                    return true;
                }
            }
            return false;
        }

        /* Compare Password */
        public static bool VerifyLogin(User user)
        {
            User userToCheck = GetUserByName(user.UserName);
            if (userToCheck == null)
                return false;

            string hashedPassword = hashPassword(user.Password, userToCheck.PasswordHash);

            return compareHash(userToCheck.Password, hashedPassword);
        }

        private static string[] hashPassword(string password)
        {
            //Used to store the hashed password and the salt (0: Hashed password 1: Salt)
            string[] hashArray = new string[2];
            byte[] salt = new byte[16];

            //Create a random salt
            var rand = RandomNumberGenerator.Create();
            rand.GetBytes(salt);
            string saltString = Convert.ToBase64String(salt);
            hashArray[1] = saltString;

            //Hashing password
            var hashedPass = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password + salt));
            hashArray[0] = Convert.ToBase64String(hashedPass);
            return hashArray;
        }

        private static string hashPassword(string password, string salts)
        {
            string hashArray;
            byte[] salt = Convert.FromBase64String(salts);
            string saltString = Convert.ToBase64String(salt);
            var hashedPass = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password + salt));
            hashArray = Convert.ToBase64String(hashedPass);

            return hashArray;
        }

        private static bool compareHash(string firstHash, string secondHash)
        {
            return firstHash == secondHash;
        }
    }
}