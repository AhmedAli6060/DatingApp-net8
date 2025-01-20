using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository(DataContext context, IMapper mapper) : IUserRepository
    {
        public void Add(AppUser user) => context.Users.Add(user);

        public void Delete(AppUser user) => context.Remove(user);

        public async Task<MemberDto?> GetMemberAsync(string username)
        {
            return await context.Users
                    .Where(x => x.UserName == username)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                    .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = context.Users.AsQueryable();
            query = query.Where(x => x.UserName != userParams.CurrentUserName);

            if (userParams.Gender != null)
                query = query.Where(x => x.Gender == userParams.Gender);

            var minDib = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

            query = query.Where(x => x.DateOfBirth >= minDib && x.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(x => x.Created),
                _ => query.OrderByDescending(x => x.LastActive)
            };

            return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(mapper.ConfigurationProvider),
                                                            userParams.PageNumber, userParams.PageSize);
        }

        public async Task<AppUser?> GetUserByIdAsync(int id) => await context.Users.FindAsync(id);


        public async Task<AppUser?> GetUserByUserNameAsync(string username) =>
                await context.Users.Include(x => x.Photos).SingleOrDefaultAsync(x => x.UserName == username);

        public async Task<IEnumerable<AppUser>> GetUsersAsync() => await context.Users.Include(x => x.Photos).ToListAsync();

        public void Update(AppUser user) => context.Entry(user).State = EntityState.Modified;
    }
}