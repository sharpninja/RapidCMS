﻿using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Abstractions.Setup
{
    internal interface ICollectionSetup : ITreeElementSetup
    {
        string? Icon { get; }
        string Name { get; }
        bool Recursive { get; }

        List<CollectionSetup> Collections { get; }

        List<EntityVariantSetup>? SubEntityVariants { get; }
        EntityVariantSetup EntityVariant { get; }

        List<IDataView>? DataViews { get; }
        Type? DataViewBuilder { get; }

        IEntityVariantSetup GetEntityVariant(string? alias);
        IEntityVariantSetup GetEntityVariant(IEntity entity);

        Type? RepositoryType { get; }

        TreeViewSetup? TreeView { get; }

        ListSetup? ListView { get; }
        ListSetup? ListEditor { get; }

        NodeSetup? NodeView { get; }
        NodeSetup? NodeEditor { get; }

        IButtonSetup? FindButton(string buttonId);
    }
}
