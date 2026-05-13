using AutoMapper;
using LibraryManager.Shared.Dtos;
using LibraryManager.Api.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ProfileUser → ProfileUserDto
        CreateMap<ProfileUser, ProfileUserDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.BorrowedBooks, opt => opt.MapFrom(src => src.BorrowedBooks));

        // BooksList → BookDto (FINAL, CORRECT VERSION)
        CreateMap<BooksList, BookDto>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.BookId))   // ⭐ REQUIRED
            .ForMember(dest => dest.BorrowerId,
                opt => opt.MapFrom(src => src.ProfileUserId))
            .ForMember(dest => dest.BorrowerUserName,
                opt => opt.MapFrom(src => src.ProfileUser != null
                    ? src.ProfileUser.User.UserName
                    : null));

        // BorrowHistory → BorrowHistoryDto
        CreateMap<BorrowHistory, BorrowHistoryDto>()
            .ForMember(dest => dest.BorrowerUserName,
                opt => opt.MapFrom(src => src.ProfileUser.User.UserName))
            .ForMember(dest => dest.BookTitle,
                opt => opt.MapFrom(src => src.Book.Title));

        // CreateBookDto → BooksList
        CreateMap<CreateBookDto, BooksList>();

        // UpdateBookDto → BooksList
        CreateMap<UpdateBookDto, BooksList>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
