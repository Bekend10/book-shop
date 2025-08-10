using AutoMapper;
using realtime_service.Entity;
using realtime_service.Models;

namespace realtime_service.Mapper
{
    public class MessageMapper : Profile
    {
        public MessageMapper()
        {
            CreateMap<Message, MessageModel>().ReverseMap();
            CreateMap<CreateMessageModel, Message>();
            CreateMap<UpdateMessageModel, Message>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
