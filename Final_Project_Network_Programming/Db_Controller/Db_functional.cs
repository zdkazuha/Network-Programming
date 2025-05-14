using Db_Controller.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Db_Controller
{
    public class Db_functional
    {
        private readonly Db_Controller context;

        public Db_functional() 
        {
            context = new Db_Controller();
        }

        public void AddUser(string username, string password)
        {
            var user = new User
            {
                Username = username,
                Password = password,
                GroupId = 0
            };

            context.Users.Add(user);
            context.SaveChanges();
        }
        public void AddUser(User user)
        {
            context.Users.Add(user);
            context.SaveChanges();
        }
        public void AddGroup(string name)
        {
            var group = new Group
            {
                Name = name
            };
            context.Groups.Add(group);
            context.SaveChanges();
        }
        public void AddGroup(Group group)
        {
            context.Groups.Add(group);
            context.SaveChanges();
        }
        public void AddContact(int userId, int contactUserId)
        {
            var contact = new Contact
            {
                UserId = userId,
                ContactUserId = contactUserId,
                CustomName = null
            };
            context.Contacts.Add(contact);
            context.SaveChanges();
        }
        public User GetUser(string username, string password)
        {
            return context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }
        public User GetUser(string username)
        {
            return context.Users
                .Include(u => u.Group)
                .AsNoTracking()  
                .FirstOrDefault(u => u.Username == username);
        }

        public Group GetGroup(string groupName)
        {
            return context.Groups
                .AsNoTracking()
                .FirstOrDefault(g => g.Name == groupName);
        }

        public bool IsUserExist(string username)
        {
            return context.Users.Any(u => u.Username == username);
        }
        public void AddUserToGroup(User user, string groupName)
        {
            Group group = context.Groups.FirstOrDefault(g => g.Name == groupName);
            if (group == null)
            {
                group = new Group { Name = groupName };
                context.Groups.Add(group);
            }


            user.GroupId = group.Id;
            user.Group = group;

            context.SaveChanges();
        }
        public User GetUserByTcpClient(TcpClient client)
        {
            return context.Users.FirstOrDefault(u => u.Username == client.Client.RemoteEndPoint.ToString());
        }
        public string GetNameGroup(int groupId)
        {
            var group = context.Groups.FirstOrDefault(g => g.Id == groupId);
            return group?.Name;
        }
        public User RenameUserGroup(User user, Group newGroup)
        {
            context.Entry(user).State = EntityState.Detached;

            user.GroupId = newGroup.Id;
            user.Group = newGroup;
            context.Users.Update(user);
            context.SaveChanges();
            return user;
        }
        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}
