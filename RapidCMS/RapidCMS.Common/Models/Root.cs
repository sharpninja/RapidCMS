﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Common.Data;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Models.Config;

namespace RapidCMS.Common.Models
{
    internal class Root : ICollectionProvider, IRepositoryProvider, IMetadataProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public Root(CmsConfig cmsConfig, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            CustomButtonRegistrations = cmsConfig.CustomButtonRegistrations.ToList();
            CustomEditorRegistrations = cmsConfig.CustomEditorRegistrations.ToList();
            CustomSectionRegistrations = cmsConfig.CustomSectionRegistrations.ToList();
            CustomDashboardSectionRegistrations = cmsConfig.CustomDashboardSectionRegistrations.ToList();
            CustomLoginRegistration = cmsConfig.CustomLoginRegistration;

            Collections = cmsConfig.ProcessCollections();

            SiteName = cmsConfig.SiteName;
            IsDevelopment = cmsConfig.IsDevelopment;

            FindRepositoryForCollections(_serviceProvider, Collections);
        }

        private Dictionary<string, Collection> _collectionMap { get; set; } = new Dictionary<string, Collection>();

        public List<CustomTypeRegistration> CustomButtonRegistrations { get; internal set; }
        public List<CustomTypeRegistration> CustomEditorRegistrations { get; internal set; }
        public List<CustomTypeRegistration> CustomSectionRegistrations { get; internal set; }
        public List<CustomTypeRegistration> CustomDashboardSectionRegistrations { get; internal set; }
        public CustomTypeRegistration? CustomLoginRegistration { get; internal set; }

        public List<Collection> Collections { get; set; }

        public string SiteName { get; private set; }
        public bool IsDevelopment { get; private set; }

        public Collection GetCollection(string alias)
        {
            return _collectionMap.TryGetValue(alias, out var collection)
                ? collection
                : throw new KeyNotFoundException($"Cannot find collection with alias {alias}");
        }
        public IEnumerable<Collection> GetAllCollections()
        {
            return Collections;
        }

        private void FindRepositoryForCollections(IServiceProvider serviceProvider, IEnumerable<Collection> collections)
        {
            foreach (var collection in collections.Where(col => !col.Recursive))
            {
                // register each collection in flat dictionary
                if (!_collectionMap.TryAdd(collection.Alias, collection))
                {
                    throw new InvalidOperationException($"Duplicate collection alias '{collection.Alias}' not allowed.");
                }

                if (collection.RepositoryType != null)
                {
                    collection.Repository = (IRepository)serviceProvider.GetRequiredService(collection.RepositoryType);
                }

                FindRepositoryForCollections(serviceProvider, collection.Collections);
            }
        }

        public IRepository? GetRepository(string collectionAlias)
        {
            return _collectionMap.TryGetValue(collectionAlias, out var collection) ? collection.Repository : default;
        }
    }
}
