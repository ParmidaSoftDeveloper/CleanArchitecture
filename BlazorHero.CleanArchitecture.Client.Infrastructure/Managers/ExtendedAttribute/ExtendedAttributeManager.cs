﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlazorHero.CleanArchitecture.Application.Features.ExtendedAttributes.Commands.AddEdit;
using BlazorHero.CleanArchitecture.Application.Features.ExtendedAttributes.Queries.GetAll;
using BlazorHero.CleanArchitecture.Application.Features.ExtendedAttributes.Queries.GetAllByEntityId;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Extensions;
using BlazorHero.CleanArchitecture.Domain.Contracts;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.ExtendedAttribute
{
    public class ExtendedAttributeManager<TId, TEntityId, TEntity, TExtendedAttribute>
        : IExtendedAttributeManager<TId, TEntityId, TEntity, TExtendedAttribute>
            where TEntity : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
            where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>
            where TId : IEquatable<TId>
    {
        private readonly HttpClient _httpClient;

        public ExtendedAttributeManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> ExportToExcelAsync(string searchString = "", TEntityId entityId = default, bool includeEntity = false)
        {
            var response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
                ? Routes.ExtendedAttributesEndpoints.Export(typeof(TEntity).Name, entityId)
                : Routes.ExtendedAttributesEndpoints.ExportFiltered(typeof(TEntity).Name, searchString, entityId, includeEntity));
            var data = await response.Content.ReadAsStringAsync();
            return data;
        }

        public async Task<IResult<TId>> DeleteAsync(TId id)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.ExtendedAttributesEndpoints.Delete(typeof(TEntity).Name)}/{id}");
            return await response.ToResult<TId>();
        }

        public async Task<IResult<List<GetAllExtendedAttributesResponse<TId, TEntityId>>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(Routes.ExtendedAttributesEndpoints.GetAll(typeof(TEntity).Name));
            return await response.ToResult<List<GetAllExtendedAttributesResponse<TId, TEntityId>>>();
        }

        public async Task<IResult<List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>>>> GetAllByEntityIdAsync(TEntityId entityId)
        {
            var route = Routes.ExtendedAttributesEndpoints.GetAllByEntityId(typeof(TEntity).Name, entityId);
            var response = await _httpClient.GetAsync(route);
            return await response.ToResult<List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>>>();
        }

        public async Task<IResult<TId>> SaveAsync(AddEditExtendedAttributeCommand<TId, TEntityId, TEntity, TExtendedAttribute> request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.ExtendedAttributesEndpoints.Save(typeof(TEntity).Name), request);
            return await response.ToResult<TId>();
        }
    }
}