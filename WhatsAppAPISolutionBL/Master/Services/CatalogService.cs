using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WhatsAppAPISolutionBL.Master.Interfaces;
using WhatsAppAPISolutionDL.Dto.Catalog;
using WhatsAppAPISolutionDL.Dto.Flow;
using WhatsAppAPISolutionDL.Enum;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;
using static WhatsAppAPISolutionDL.Dto.Catalog.CatalogDto;
using static WhatsAppAPISolutionDL.Dto.Order.MetaOrderRequestDto.Order;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class CatalogService : ICatalogService
    {
        #region Fields    

        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly ILogger<CatalogService> _logger;
        private readonly IExportManager _exportManager;
        private readonly IMediaService _mediaService;
        private readonly IFlowsService _flowsService;
        private DateTime dateTimeNow = DateTime.UtcNow;

        #endregion

        #region Ctor

        public CatalogService(
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            ILogger<CatalogService> logger,
            IExportManager exportManager,
            IMediaService mediaService,
            IFlowsService flowsService)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _logger = logger;
            _exportManager = exportManager;
            _mediaService = mediaService;
            _flowsService = flowsService;
        }

        #endregion

        #region Utilities

        private async Task DeactivateExistingCatalog(int clientId, int senderId)
        {
            // Soft delete existing records with clientId and senderId filters
            var items = await _dbContext.Items.Where(x => x.ClientId == clientId && x.SenderId == senderId && x.Status == 1).ToListAsync();

            //Update item status = 0
            foreach (var item in items)
            {
                item.Status = 0;
                item.DeprecatedDate = dateTimeNow;
                item.UpdatedDate = dateTimeNow;

                _dbContext.Entry(item).State = EntityState.Modified;
            }

            // Soft delete existing categories
            var categories = await _dbContext.Categories.Where(x => x.ClientId == clientId && x.SenderId == senderId && x.Status == 1).ToListAsync();
            foreach (var category in categories)
            {
                category.Status = 0;
                category.DeprecatedDate = dateTimeNow;
                category.UpdatedDate = dateTimeNow;

                _dbContext.Entry(category).State = EntityState.Modified;
            }

            // Soft delete existing category item map
            var categoryItemMaps = await _dbContext.CategoryItemMaps.Where(x => x.ClientId == clientId && x.SenderId == senderId && x.Status == 1).ToListAsync();
            foreach (var categoryItemMap in categoryItemMaps)
            {
                categoryItemMap.Status = 0;
                categoryItemMap.DeprecatedDate = dateTimeNow;
                categoryItemMap.UpdatedDate = dateTimeNow;

                _dbContext.Entry(categoryItemMap).State = EntityState.Modified;
            }

            // Soft delete existing modifiers
            var modifiers = await _dbContext.ModifierGroups.Where(x => x.ClientId == clientId && x.SenderId == senderId && x.Status == 1).ToListAsync();
            foreach (var modifier in modifiers)
            {
                modifier.Status = 0;
                modifier.DeprecatedDate = dateTimeNow;
                modifier.UpdatedDate = dateTimeNow;

                _dbContext.Entry(modifier).State = EntityState.Modified;
            }

            // Soft delete existing modifier item map
            var modifierItemMaps = await _dbContext.ModifierItemMaps.Where(x => x.ClientId == clientId && x.SenderId == senderId && x.Status == 1).ToListAsync();
            foreach (var modifierItemMap in modifierItemMaps)
            {
                modifierItemMap.Status = 0;
                modifierItemMap.DeprecatedDate = dateTimeNow;
                modifierItemMap.UpdatedDate = dateTimeNow;

                _dbContext.Entry(modifierItemMap).State = EntityState.Modified;
            }

            // Soft delete existing item modifier map
            var itemModifierMaps = await _dbContext.ItemModifierMaps.Where(x => x.ClientId == clientId && x.SenderId == senderId && x.Status == 1).ToListAsync();
            foreach (var itemModifierMap in itemModifierMaps)
            {
                itemModifierMap.Status = 0;
                itemModifierMap.DeprecatedDate = dateTimeNow;
                itemModifierMap.UpdatedDate = dateTimeNow;

                _dbContext.Entry(itemModifierMap).State = EntityState.Modified;
            }

            await _dbContext.SaveChangesAsync();
        }

        private async Task CategoriesOps(int clientId, int senderId, CatalogDto catalog)
        {
            //Add categories
            if (catalog.menu.categories != null && catalog.menu.categories.Any())
            {
                var menuCategories = catalog.menu.categories.Where(x => x.item_ids != null && x.item_ids.Any()).ToList();
                for (int i = 0; i < menuCategories.Count; i++)
                {
                    var menuCategory = menuCategories[i];

                    var category = _dbContext.Categories.FirstOrDefault(x => x.ClientId == clientId && x.SenderId == senderId && x.IntegerationId == menuCategory.id);
                    if (category != null)
                    {
                        category.NameEn = menuCategory.name?.en;
                        category.NameAr = menuCategory.name?.ar ?? menuCategory.name.en;
                        category.DisplayOrder = i + 1;
                        category.Status = 1;
                        category.DeprecatedDate = null;
                        category.UpdatedBy = 0;
                        category.UpdatedDate = dateTimeNow;

                        _dbContext.Entry(category).State = EntityState.Modified;
                    }
                    else
                    {
                        category = new WhatsAppAPISolutionDL.Models.Category
                        {
                            ClientId = clientId,
                            SenderId = senderId,
                            NameEn = menuCategory.name?.en,
                            NameAr = menuCategory.name?.ar ?? menuCategory.name.en,
                            DescriptionEn = String.Empty,
                            DescriptionAr = String.Empty,
                            DisplayOrder = i + 1,
                            IntegerationId = menuCategory.id?.Trim(),
                            Status = 1,
                            CreatedBy = 0,
                            CreatedDate = dateTimeNow,
                            UpdatedBy = 0,
                            UpdatedDate = dateTimeNow
                        };

                        await _dbContext.Categories.AddAsync(category);
                    }

                    //save context
                    await _dbContext.SaveChangesAsync();

                    for (int j = 0; j < menuCategory.item_ids.Count; j++)
                    {
                        var item = await _dbContext.Items.FirstOrDefaultAsync(x => x.ClientId == clientId && x.SenderId == senderId && x.IntegrationId == menuCategory.item_ids[j]);
                        if (item != null)
                        {
                            var categoryItemMap = await _dbContext.CategoryItemMaps.FirstOrDefaultAsync(x => x.ClientId == clientId && x.SenderId == senderId
                                && x.CategoryId == category.Id
                                && x.ItemId == item.Id);

                            if (categoryItemMap != null)
                            {
                                categoryItemMap.DisplayOrder = j + 1;
                                categoryItemMap.Status = 1;
                                categoryItemMap.DeprecatedDate = null;
                                categoryItemMap.UpdateBy = 0;
                                categoryItemMap.UpdatedDate = dateTimeNow;

                                _dbContext.Entry(categoryItemMap).State = EntityState.Modified;
                            }
                            else
                            {
                                categoryItemMap = new CategoryItemMap
                                {
                                    ClientId = clientId,
                                    SenderId = senderId,
                                    ItemId = item.Id,
                                    CategoryId = category.Id,
                                    DisplayOrder = j + 1,
                                    Status = 1,
                                    CreatedBy = 0,
                                    CreatedDate = dateTimeNow,
                                    UpdateBy = 0,
                                    UpdatedDate = dateTimeNow
                                };

                                await _dbContext.CategoryItemMaps.AddAsync(categoryItemMap);
                            }

                            //save context
                            await _dbContext.SaveChangesAsync();
                        }
                    }
                }
            }
        }

        private async Task ItemsOps(int clientId, int senderId, CatalogDto catalog)
        {
            //Add/Update items
            if (catalog.menu.items != null && catalog.menu.items.Any())
            {
                var menuItems = catalog.menu.items.ToList();
                for (int i = 0; i < menuItems.Count; i++)
                {
                    var menuItem = menuItems[i];

                    var existingItem = await _dbContext.Items.FirstOrDefaultAsync(i => i.ClientId == clientId && i.SenderId == senderId && i.IntegrationId == menuItem.id);
                    if (existingItem != null)
                    {
                        existingItem.NameEn = menuItem.name?.en;
                        existingItem.NameAr = menuItem.name?.ar ?? menuItem.name?.en;
                        existingItem.DescriptionEn = menuItem.description?.en;
                        existingItem.DescriptionAr = menuItem.description?.ar ?? menuItem.description?.en;
                        existingItem.Price = menuItem.price_info?.price ?? 0;
                        existingItem.ImageUrl = menuItem.image?.url ?? "";
                        existingItem.ProductUrl = menuItem.ItemURL ?? "";
                        existingItem.Status = 1;
                        existingItem.DeprecatedDate = null;
                        existingItem.FlowUpdated = false;
                        existingItem.FlowRequired = false;
                        existingItem.FlowRequestProcessed = false;
                        existingItem.UpdatedBy = 0;
                        existingItem.UpdatedDate = dateTimeNow;

                        if (String.IsNullOrWhiteSpace(existingItem.DescriptionEn))
                            existingItem.DescriptionEn = "-";

                        if (String.IsNullOrWhiteSpace(existingItem.DescriptionAr))
                            existingItem.DescriptionAr = "-";

                        _dbContext.Entry(existingItem).State = EntityState.Modified;
                    }
                    else
                    {
                        var item = new WhatsAppAPISolutionDL.Models.Item
                        {
                            ClientId = clientId,
                            SenderId = senderId,
                            IntegrationId = menuItem.id?.Trim(),
                            NameEn = menuItem.name?.en,
                            NameAr = menuItem.name?.ar ?? menuItem.name.en,
                            DescriptionEn = menuItem.description?.en ?? "-",
                            DescriptionAr = menuItem.description?.ar ?? menuItem.description?.en,
                            ProductUrl = menuItem.ItemURL ?? "",
                            EnflowId = 0,
                            ArflowId = 0,
                            ImageUrl = menuItem.image?.url ?? "",
                            Price = menuItem.price_info?.price ?? 0,
                            ItemType = menuItem.type?.ToLower() == "item" ? (int)ItemType.ITEM : (int)ItemType.CHOICE,
                            FlowUpdated = false,
                            FlowRequired = false,
                            FlowRequestProcessed = false,
                            Status = 1,
                            CreatedBy = 0,
                            CreatedDate = dateTimeNow,
                            UpdatedBy = 0,
                            UpdatedDate = dateTimeNow
                        };

                        if (String.IsNullOrWhiteSpace(item.DescriptionEn))
                            item.DescriptionEn = "-";

                        if (String.IsNullOrWhiteSpace(item.DescriptionAr))
                            item.DescriptionAr = "-";

                        await _dbContext.Items.AddAsync(item);
                    }
                }

                //save context
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task ModifiersOps(int clientId, int senderId, CatalogDto catalog)
        {
            //Add modifiers
            if (catalog.menu.modifiers != null && catalog.menu.modifiers.Any())
            {
                var menuModifiers = catalog.menu.modifiers.Where(x => x.item_ids != null && x.item_ids.Any()).ToList();
                for (int i = 0; i < menuModifiers.Count; i++)
                {
                    var menuModifier = menuModifiers[i];

                    var modifierGroup = await _dbContext.ModifierGroups.FirstOrDefaultAsync(x => x.ClientId == clientId && x.SenderId == senderId && x.IntegrationId == menuModifier.id);
                    if (modifierGroup != null)
                    {
                        modifierGroup.NameEn = menuModifier.name?.en;
                        modifierGroup.NameAr = menuModifier.name?.ar ?? menuModifier.name.en;
                        modifierGroup.DescriptionEn = menuModifier.description?.en;
                        modifierGroup.DescriptionAr = menuModifier.description?.ar ?? menuModifier.description.en;
                        modifierGroup.MinSelection = menuModifier.min_selection;
                        modifierGroup.MaxSelection = menuModifier.max_selection;
                        modifierGroup.Status = 1;
                        modifierGroup.DeprecatedDate = null;
                        modifierGroup.UpdatedBy = 0;
                        modifierGroup.UpdatedDate = dateTimeNow;

                        _dbContext.Entry(modifierGroup).State = EntityState.Modified;
                    }
                    else
                    {
                        modifierGroup = new ModifierGroup
                        {
                            ClientId = clientId,
                            SenderId = senderId,
                            IntegrationId = menuModifier.id,
                            NameEn = menuModifier.name?.en,
                            NameAr = menuModifier.name?.ar ?? menuModifier.name.en,
                            DescriptionEn = menuModifier.description?.en,
                            DescriptionAr = menuModifier.description?.ar ?? menuModifier.description.en,
                            Status = 1,
                            CreatedBy = 0,
                            CreatedDate = dateTimeNow,
                            UpdatedBy = 0,
                            UpdatedDate = dateTimeNow,
                            MinSelection = menuModifier.min_selection,
                            MaxSelection = menuModifier.max_selection
                        };

                        await _dbContext.ModifierGroups.AddAsync(modifierGroup);
                    }

                    //save context
                    await _dbContext.SaveChangesAsync();

                    //Modifier Items Ops
                    await ModifierItemsOps(modifierGroup, menuModifier);

                    //Item modifiers Ops
                    await ItemModifiersOps(modifierGroup, catalog);
                }
            }
        }

        private async Task ModifierItemsOps(ModifierGroup modifierGroup, Modifier modifier)
        {
            //Add modifier items
            if (modifier.item_ids != null && modifier.item_ids.Any())
            {
                var menuItems = await _dbContext.Items.Where(x =>
                        x.ClientId == modifierGroup.ClientId && x.SenderId == modifierGroup.SenderId
                        && x.Status == 1
                        && x.ItemType == (int)ItemType.CHOICE
                        && modifier.item_ids.Contains(x.IntegrationId)).ToListAsync();

                for (int i = 0; i < menuItems.Count; i++)
                {
                    var menuItem = menuItems[i];

                    //Check existing modifier item map
                    var modifierItemMap = await _dbContext.ModifierItemMaps.FirstOrDefaultAsync(x => x.ClientId == modifierGroup.ClientId && x.SenderId == modifierGroup.SenderId
                    && x.ModifierGroupId == modifierGroup.Id && x.ItemId == menuItem.Id);

                    if (modifierItemMap != null)
                    {
                        modifierItemMap.DisplayOrder = i + 1;
                        modifierItemMap.Status = 1;
                        modifierItemMap.DeprecatedDate = null;
                        modifierItemMap.UpdatedBy = 0;
                        modifierItemMap.UpdatedDate = dateTimeNow;

                        _dbContext.Entry(modifierItemMap).State = EntityState.Modified;
                    }
                    else
                    {
                        modifierItemMap = new ModifierItemMap
                        {
                            ClientId = modifierGroup.ClientId,
                            SenderId = modifierGroup.SenderId,
                            Status = 1,
                            ItemId = menuItems[i].Id,
                            ModifierGroupId = modifierGroup.Id,
                            DisplayOrder = i + 1,
                            CreatedBy = 0,
                            CreatedDate = dateTimeNow,
                            UpdatedBy = 0,
                            UpdatedDate = dateTimeNow
                        };

                        await _dbContext.ModifierItemMaps.AddAsync(modifierItemMap);
                    }

                    //save context
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        private async Task ItemModifiersOps(ModifierGroup modifierGroup, CatalogDto catalog)
        {
            //Filter out the items which has this modifier
            var items = catalog.menu.items.Where(x => x.modifier_ids.Contains(modifierGroup.IntegrationId)).Select(x => x.id).ToList();

            //Add item modifiers
            if (items != null && items.Any())
            {
                var menuItems = await _dbContext.Items.Where(x =>
                        x.ClientId == modifierGroup.ClientId && x.SenderId == modifierGroup.SenderId
                        && x.Status == 1 
                        && items.Contains(x.IntegrationId)).ToListAsync();

                for (int i = 0; i < menuItems.Count; i++)
                {
                    var menuItem = menuItems[i];

                    //Check existing modifier item map
                    var itemModifierMap = await _dbContext.ItemModifierMaps.FirstOrDefaultAsync(x => x.ClientId == modifierGroup.ClientId && x.SenderId == modifierGroup.SenderId
                    && x.ModifierGroupId == modifierGroup.Id && x.ItemId == menuItem.Id);

                    if (itemModifierMap != null)
                    {
                        itemModifierMap.Status = 1;
                        itemModifierMap.DeprecatedDate = null;
                        itemModifierMap.DisplayOrder = i + 1;
                        itemModifierMap.UpdatedBy = 0;
                        itemModifierMap.UpdatedDate = dateTimeNow;

                        _dbContext.Entry(itemModifierMap).State = EntityState.Modified;
                    }
                    else
                    {
                        itemModifierMap = new ItemModifierMap
                        {
                            ClientId = modifierGroup.ClientId,
                            SenderId = modifierGroup.SenderId,
                            Status = 1,
                            ItemId = menuItem.Id,
                            ModifierGroupId = modifierGroup.Id,
                            DisplayOrder = i + 1,
                            CreatedBy = 0,
                            CreatedDate = dateTimeNow,
                            UpdatedBy = 0,
                            UpdatedDate = dateTimeNow
                        };

                        await _dbContext.ItemModifierMaps.AddAsync(itemModifierMap);
                    }

                    //save context
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        private async Task FlowRequiredOps(int clientId, int senderId)
        {
            var items = _dbContext.Items.Where(x => x.ClientId == clientId
                && x.SenderId == senderId
                && x.ItemType == (int)ItemType.ITEM
                && x.Status == 1).ToList();

            foreach (var item in items)
            {
                var itemModifierMaps = await _dbContext.ItemModifierMaps.Where(x => x.ClientId == clientId
                                      && x.SenderId == senderId
                                      && x.Status == 1
                                      && x.ItemId == item.Id).ToListAsync();

                //If item modifier map is present, we mark as flow required
                item.FlowRequired = itemModifierMaps.Any();
                item.UpdatedDate = dateTimeNow;

                _dbContext.Entry(item).State = EntityState.Modified;
            }

            //save context
            await _dbContext.SaveChangesAsync();
        }

        private async Task<FlowDTO> GenerateFlow(int clientId, int senderId, string localization, int flowId, WhatsAppAPISolutionDL.Models.Item item, List<ModifierGroup> modifierGroups)
        {
            var flowDto = new FlowDTO
            {
                SenderId = senderId,
                FlowId = flowId,
                FlowLanguage = localization,
                PublishToFB = true,
                FlowName = String.Concat(item.NameEn, "_", localization),
                FlowScreens = new List<FlowScreenDTO>()
            };

            for (int i = 0; i < modifierGroups.Count; i++)
            {
                var modifierGroup = modifierGroups[i];

                var screen = new FlowScreenDTO
                {
                    Name = "screen",
                    Title = localization.ToLower() == "en" ? modifierGroup.NameEn : modifierGroup.NameAr ?? modifierGroup.NameEn,
                    ScreenButtonText = (i == modifierGroups.Count - 1) ? "Submit" : "Next",
                    FlowChildren = new List<FlowChildrenDTO>()
                };

                int controlType = 0;
                bool required = true;
                if (modifierGroup.MinSelection == 1 && modifierGroup.MaxSelection == 1)
                {
                    controlType = (int)FlowControlType.RadioButtonsGroup;
                    required = true;
                }
                else
                {
                    required = false;
                    controlType = (int)FlowControlType.CheckboxGroup;
                }

                var children = new FlowChildrenDTO
                {
                    Text = localization.ToLower() == "en" ? modifierGroup.NameEn : modifierGroup.NameAr ?? modifierGroup.NameEn,
                    Required = required,
                    MinSelection = modifierGroup.MinSelection ?? 0,
                    MaxSelection = modifierGroup.MaxSelection ?? 0,
                    Type = controlType,
                    FlowOptions = new List<FlowOptionDTO>()
                };

                var modifierItems = await _dbContext.ModifierItemMaps.Where(x => x.ClientId == clientId
                                    && x.SenderId == senderId
                                    && x.Status == 1
                                    && x.ModifierGroupId == modifierGroup.Id).ToListAsync();

                foreach (var modifierItem in modifierItems)
                {
                    var modItem = await _dbContext.Items.FindAsync(modifierItem.ItemId);
                    if (modItem == null || modItem.Status == -1)
                        continue;

                    var flowOption = new FlowOptionDTO
                    {
                        OptionId = modItem.Id.ToString(),
                        OptionText = localization.ToLower() == "en" ? modItem.NameEn : modItem.NameAr ?? modItem.NameEn,
                        Metadata = String.Concat("(", (modItem.Price ?? 0).ToString("0.000"), ")")
                    };

                    children.FlowOptions.Add(flowOption);
                };

                screen.FlowChildren.Add(children);

                flowDto.FlowScreens.Add(screen);
            }

            return flowDto;
        }

        #endregion

        #region Methods

        public async Task ImportCatalog(int clientId, int senderId, CatalogDto catalog)
        {
            //Deactivate items and delete existing item mapping
            await DeactivateExistingCatalog(clientId, senderId);

            //Add/Update items
            await ItemsOps(clientId, senderId, catalog);

            //Add categories
            await CategoriesOps(clientId, senderId, catalog);

            //Add modifiers, item modifiers map and modifier items map
            await ModifiersOps(clientId, senderId, catalog);

            //Update flow required by items
            await FlowRequiredOps(clientId, senderId);

            var catalogImportHistory = new CatalogImportHistory
            {
                ClientId = clientId,
                SenderId = senderId,
                CreatedBy = 0,
                CreatedDate = dateTimeNow,
                UpdatedBy = 0,
                UpdatedDate = dateTimeNow
            };

            await _dbContext.CatalogImportHistories.AddAsync(catalogImportHistory);
            await _dbContext.SaveChangesAsync();

            await ExportCatalog(clientId, senderId);
        }

        public async Task ExportCatalog(int clientId, int senderId)
        {
            var response = await _dbContext2.CatalogExports.FromSqlInterpolated($"exec usp_GetItemsToExport @ClientId={clientId}, @SenderId={senderId}").ToListAsync();

            var enExport = _exportManager.ExportCatalogItemsENToCsv(response);
            var arExport = _exportManager.ExportCatalogItemsARToCsv(response);

            _mediaService.ExportCatalog(clientId, senderId, enExport, "en");
            _mediaService.ExportCatalog(clientId, senderId, arExport, "ar");
        }

        public async Task GenerateCatalogFlows(List<CatalogFlowGenerationDto> models)
        {
            foreach (var model in models)
            {
                var items = _dbContext.Items.Where(x => x.ClientId == model.ClientId
                        && x.SenderId == model.SenderId
                        && x.Id == model.ItemId
                        && x.Status == 1
                        && x.ItemType == (int)ItemType.ITEM
                        && x.FlowUpdated == false
                        && x.FlowRequired == true).ToList();

                foreach (var item in items)
                {
                    var itemModifierMaps = await _dbContext.ItemModifierMaps.Where(x => x.ClientId == model.ClientId
                                            && x.SenderId == model.SenderId
                                            && x.Status == 1
                                            && x.ItemId == item.Id).ToListAsync();

                    //If item modifier maps exists then only generate flows else no need for simple items
                    if (itemModifierMaps.Any())
                    {
                        var modifierGroups = await _dbContext.ModifierGroups
                            .Where(x => x.ClientId == model.ClientId
                            && x.SenderId == model.SenderId
                            && x.Status == 1
                            && itemModifierMaps.Select(x => x.ModifierGroupId).Contains(x.Id)).ToListAsync();

                        List<string> localizations = new List<string> { "EN", "AR" };

                        foreach (var locale in localizations)
                        {
                            var flowId = (locale == "EN" ? item.EnflowId : item.ArflowId) ?? 0;
                            var flow = await GenerateFlow(model.ClientId, model.SenderId, locale, flowId, item, modifierGroups);

                            if (flowId > 0)
                            {
                                var response = await _flowsService.UpdateFlowAsync(model.ClientId, 0, flow);
                                item.FlowUpdated = response.Status > 0 && response.Id > 0;
                                item.FlowRequestProcessed = response.Status > 0 && response.Id > 0;
                                item.UpdatedDate = DateTime.UtcNow;
                            }
                            else
                            {
                                var response = await _flowsService.AddFlowAsync(model.ClientId, 0, flow);
                                item.FlowUpdated = response.Status > 0 && response.Id > 0;
                                item.FlowRequestProcessed = response.Status > 0 && response.Id > 0;
                                item.UpdatedDate = DateTime.UtcNow;
                                if (locale == "EN")
                                    item.EnflowId = response.Id;
                                else if (locale == "AR")
                                    item.ArflowId = response.Id;
                            }
                        }

                        _dbContext.Entry(item).State = EntityState.Modified;
                        await _dbContext.SaveChangesAsync();
                    }
                }
            }
        }

        #endregion

    }
}
