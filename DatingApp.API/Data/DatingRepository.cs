using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;

        public DatingRepository(DataContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            this._context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            this._context.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users
            .Include(p => p.Photos)
            .Include(l=>l.Likees)
            .Include(l=>l.Likers)
            .FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<Photo> GetPhoto(int id){
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = _context.Users.Include(p => p.Photos).Include(u=>u.Likees).Include(u=>u.Likers);
            var results = await PagedList<User>.CreateAsync(users,userParams.PageNumber,userParams.PageSize,
            FilterBy(userParams.UserId, userParams.Gender));
            return results;
        }

        private Func<User,Boolean> FilterBy(int userId, string Gender = "") {
            return (User user)=> {
                if(string.IsNullOrEmpty(Gender) && userId == 0)
                    return true;
                if((!string.IsNullOrEmpty(Gender) && user.Gender == Gender.ToLower()) || user.Id == userId)
                    return true;
                return false;
            };
            
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0 ;
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(u=> u.LikerId == userId && u.LikeeId == recipientId );
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(msg=>msg.Id == id);
        }

        public Task<PagedList<Message>> GetMessages()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Message>> GetMessageThread(int senderId, int recipientId)
        {
            throw new NotImplementedException();
        }
    }
}