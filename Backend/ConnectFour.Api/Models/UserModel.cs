using AutoMapper;
using ConnectFour.Domain;

namespace ConnectFour.Api.Models;

public class UserModel
{
    public Guid Id { get; set; }

    public string Email { get; set; }
    public string NickName { get; set; }

    private class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserModel>();
        }
    }
}