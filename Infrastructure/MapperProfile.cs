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
        #endregion
    }
}