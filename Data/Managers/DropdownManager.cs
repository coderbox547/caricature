using AutoMapper;
using Data.Contract.Response;
using Data.Domain;
using Data.Enums;
using Data.Extensions;
using Data.Repository.EntityFilters;
using Data.Repository.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Managers
{
    public interface IDropdownManager
    {
        Task<SearchResponse<DropDownResponseModel>> GetDropdown1();
    }

    public class DropdownManager : IDropdownManager
    {
        public DropdownManager()
        {
        }

        public async Task<SearchResponse<DropDownResponseModel>> GetDropdown1()
        {
            return new SearchResponse<DropDownResponseModel>
            {
                Data = new List<DropDownResponseModel>(),
                TotalCount = 1
            };
        }
    }
}
