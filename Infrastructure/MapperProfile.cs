using AdventureWorksAPI.Models.DMO;
using AutoMapper;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        #region ProductCategory Maplemesi
        CreateMap<ProductCategory, CategoryDTO>();
        CreateMap<CategoryDTO, CategoryVM>();
        #endregion

        #region ProductSubCategory Maplemesi
        CreateMap<ProductSubcategory, SubcategoryDTO>();
        CreateMap<SubcategoryDTO, SubcategoryVM>();
        #endregion

        #region Product Maplemesi
        CreateMap<Product, ProductDTO>().ReverseMap();
        CreateMap<ProductDTO, ProductVM>().ReverseMap();

        CreateMap<Product, DetailedProductDTO>().ReverseMap();
        CreateMap<DetailedProductDTO, DetailedProductVM>().ReverseMap();
        #endregion

        #region User Controller maplemeleri
        CreateMap<RegisterDTO, RegisterVM>().ReverseMap();
        CreateMap<StateDTO, StateVM>().ReverseMap();
        CreateMap<AddressTypeDTO, AddressTypeVM>().ReverseMap();
        CreateMap<UpdateUserDTO, UpdateUserVM>().ReverseMap();
        #endregion


    }
}