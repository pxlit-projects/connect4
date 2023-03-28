using AutoMapper;
using ConnectFour.Domain.GameDomain;
using ConnectFour.Domain.GameDomain.Contracts;
using ConnectFour.Domain.GridDomain;

namespace ConnectFour.Api.Models;

public class MoveInputModel
{
    public MoveType Type { get; set; }
    public DiscType DiscType { get; set; }
    public int Column { get; set; }

    private class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<MoveInputModel, IMove>().ConstructUsing(model => (new Move(model.Column, model.Type, model.DiscType) as IMove)!);
        }
    }
}