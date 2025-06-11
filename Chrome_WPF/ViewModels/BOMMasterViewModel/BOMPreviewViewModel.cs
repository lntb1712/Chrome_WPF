using Chrome_WPF.Helpers;
using Chrome_WPF.Models.BOMComponentDTO;
using Chrome_WPF.Models.BOMMasterDTO;
using Chrome_WPF.Services.BOMComponentService;
using Chrome_WPF.Services.MessengerService;
using Chrome_WPF.Services.NavigationService;
using Chrome_WPF.Services.NotificationService;
using Chrome_WPF.Views.UserControls.BOMMaster;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chrome_WPF.ViewModels.BOMMasterViewModel
{
    public class BOMPreviewViewModel : INotifyPropertyChanged
    {
        private readonly IBOMComponentService _bomService;
        private readonly INotificationService _notificationService;
        private readonly IMessengerService _messengerService;
        private readonly INavigationService _navigationService;
        private readonly IMemoryCache _memoryCache;

        private ObservableCollection<BOMNodeViewModel> _bomTree;
        private ObservableCollection<BOMNodeDTO> _bomDtoTree;

        private const string BOM_DTO_CACHE_KEY_PREFIX = "BOMDtoTree_";
        private const string BOM_TREE_CACHE_KEY_PREFIX = "BOMTree_";
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5); // Cache expires after 5 minutes

        public ObservableCollection<BOMNodeViewModel> BOMTree
        {
            get => _bomTree;
            set
            {
                _bomTree = value;
                OnPropertyChanged(nameof(BOMTree));
            }
        }

        public ObservableCollection<BOMNodeDTO> BOMDtoTree
        {
            get => _bomDtoTree;
            set
            {
                _bomDtoTree = value;
                OnPropertyChanged(nameof(BOMDtoTree));
            }
        }

        public ICommand BackCommand { get; }

        public BOMPreviewViewModel(
            IBOMComponentService bomService,
            INotificationService notificationService,
            INavigationService navigationService,
            IMemoryCache memoryCache,
            IMessengerService messengerService)
        {
            _bomService = bomService ?? throw new ArgumentNullException(nameof(bomService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _messengerService = messengerService ?? throw new ArgumentNullException(nameof(messengerService));

            BackCommand = new RelayCommand(BackToBOMComponent);

            _bomDtoTree = new ObservableCollection<BOMNodeDTO>();
            _bomTree = new ObservableCollection<BOMNodeViewModel>();
        }

        private void BackToBOMComponent(object parameter)
        {
            var ucBOMComponent = App.ServiceProvider!.GetRequiredService<ucBOMComponent>();
            var viewModel = ucBOMComponent.DataContext as BOMComponentViewModel;
            if (viewModel != null)
            {
                // Gửi thông điệp để khôi phục BOMCode và BOMVersion
                _messengerService.SendMessageAsync("RestoreBOMState", new
                {
                    BOMCode = BOMDtoTree.FirstOrDefault()?.BOMCode,
                    BOMVersion = BOMDtoTree.FirstOrDefault()?.BOMVersion
                });
            }
            _navigationService.NavigateTo(ucBOMComponent);
        }

        public async Task InitializeAsync(BOMMasterRequestDTO bom, bool forceRefresh = false)
        {
            try
            {
                string dtoCacheKey = $"{BOM_DTO_CACHE_KEY_PREFIX}{bom.BOMCode}_{bom.BOMVersion}";
                string treeCacheKey = $"{BOM_TREE_CACHE_KEY_PREFIX}{bom.BOMCode}_{bom.BOMVersion}";

                // Try to get BOMDtoTree from cache
                if (!forceRefresh && _memoryCache.TryGetValue(dtoCacheKey, out ObservableCollection<BOMNodeDTO>? cachedDtoTree))
                {
                    BOMDtoTree = cachedDtoTree!;

                    // Try to get BOMTree from cache
                    if (_memoryCache.TryGetValue(treeCacheKey, out ObservableCollection<BOMNodeViewModel>? cachedTree))
                    {
                        BOMTree = cachedTree!;
                        return;
                    }
                    // If BOMTree not cached, build it from cached BOMDtoTree
                    BOMTree = BuildTree(BOMDtoTree, bom.ProductCode);
                    _memoryCache.Set(treeCacheKey, BOMTree, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = _cacheExpiration
                    });
                    return;
                }

                // Fetch data from service if not cached
                var result = await _bomService.GetRecursiveBOMAsync(bom.BOMCode, bom.BOMVersion);
                if (result.Success && result.Data != null)
                {
                    BOMDtoTree.Clear();
                    // Add root node based on bomCode if not present
                    if (!result.Data.Any(n => n.ComponentCode == bom.ProductCode))
                    {
                        BOMDtoTree.Add(new BOMNodeDTO
                        {
                            BOMCode = bom.BOMCode,
                            ProductCode = bom.ProductCode,
                            ProductName = result.Data.FirstOrDefault()?.ProductName ?? string.Empty,
                            ComponentCode = bom.ProductCode,
                            ComponentName = result.Data.FirstOrDefault()?.ProductName ?? string.Empty,
                            BOMVersion = bom.BOMVersion,
                            TotalQuantity = 1,
                            Level = 1
                        });
                    }
                    foreach (var bomNode in result.Data)
                    {
                        BOMDtoTree.Add(bomNode);
                    }
                    OnPropertyChanged(nameof(BOMDtoTree));

                    // Cache BOMDtoTree
                    _memoryCache.Set(dtoCacheKey, BOMDtoTree, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = _cacheExpiration
                    });

                    // Build and cache BOMTree
                    BOMTree = BuildTree(BOMDtoTree, bom.ProductCode);
                    _memoryCache.Set(treeCacheKey, BOMTree, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = _cacheExpiration
                    });
                }
                else
                {
                    _notificationService.ShowMessage("Không tìm thấy dữ liệu BOM.", "OK", isError: true);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowMessage($"Lỗi: {ex.Message}", "OK", isError: true);
            }
        }

        private ObservableCollection<BOMNodeViewModel> BuildTree(ObservableCollection<BOMNodeDTO> nodes, string rootProductCode)
        {
            var tree = new ObservableCollection<BOMNodeViewModel>();
            var nodeLookup = new Dictionary<string, BOMNodeViewModel>(); // Key: ComponentCode, Value: Node

            // Sort nodes by Level to process from highest level
            var sortedNodes = nodes.OrderBy(n => n.Level).ToList();

            if (!sortedNodes.Any())
            {
                _notificationService.ShowMessage("Không tìm thấy node nào để xây dựng cây.", "OK", isError: true);
                return tree;
            }

            BOMNodeViewModel rootNode = null!;

            // Find and add root node based on rootProductCode
            foreach (var node in sortedNodes)
            {
                if (node.ComponentCode == rootProductCode) // Identify root node
                {
                    rootNode = new BOMNodeViewModel(node)
                    {
                        Data = new BOMNodeDTO
                        {
                            Level = 0, // Root node at Level 0
                            BOMCode = node.BOMCode,
                            ProductCode = node.ProductCode,
                            ProductName = node.ProductName,
                            BOMVersion = node.BOMVersion,
                            ComponentCode = node.ComponentCode,
                            ComponentName = node.ComponentName,
                            TotalQuantity = node.TotalQuantity
                        }
                    };
                    tree.Add(rootNode);
                    nodeLookup[node.ComponentCode] = rootNode;
                    break; // Only add the first matching root node
                }
            }

            if (rootNode == null)
            {
                _notificationService.ShowMessage($"Không tìm thấy node gốc {rootProductCode}.", "OK", isError: true);
                return tree;
            }

            // Get unique ComponentCodes from Level 1 as direct children
            var level1Components = sortedNodes
                .Where(n => n.Level == 1 && n.ComponentCode != rootProductCode)
                .Select(n => n.ComponentCode)
                .Distinct();

            foreach (var component in level1Components)
            {
                var firstNode = sortedNodes.First(n => n.ComponentCode == component);
                var adjustedLevel = 1; // Child nodes at Level 1
                var nodeViewModel = new BOMNodeViewModel(firstNode)
                {
                    Data = new BOMNodeDTO
                    {
                        Level = adjustedLevel,
                        BOMCode = firstNode.BOMCode,
                        ProductCode = firstNode.ProductCode,
                        ProductName = firstNode.ProductName,
                        BOMVersion = firstNode.BOMVersion,
                        ComponentCode = firstNode.ComponentCode,
                        ComponentName = firstNode.ComponentName,
                        TotalQuantity = firstNode.TotalQuantity
                    }
                };
                rootNode.Children.Add(nodeViewModel);
                nodeLookup[component] = nodeViewModel;
            }

            // Process remaining nodes (Level > 1)
            foreach (var node in sortedNodes)
            {
                if (node.Level > 1 && node.ComponentCode != rootProductCode)
                {
                    var adjustedLevel = node.Level - 1; // Adjust level to match structure
                    var nodeViewModel = new BOMNodeViewModel(node)
                    {
                        Data = new BOMNodeDTO
                        {
                            Level = adjustedLevel,
                            BOMCode = node.BOMCode,
                            ProductCode = node.ProductCode,
                            ProductName = node.ProductName,
                            BOMVersion = node.BOMVersion,
                            ComponentCode = node.ComponentCode,
                            ComponentName = node.ComponentName,
                            TotalQuantity = node.TotalQuantity
                        }
                    };

                    // Store node in lookup by ComponentCode
                    nodeLookup[node.ComponentCode] = nodeViewModel;

                    // Find parent node based on ProductCode
                    if (nodeLookup.TryGetValue(node.ProductCode, out var parentNode))
                    {
                        parentNode.Children.Add(nodeViewModel);
                    }
                }
            }

            // Sort children by BOMCode in each node
            foreach (var node in tree.SelectMany(n => FindAllNodes(n)))
            {
                node.Children = new ObservableCollection<BOMNodeViewModel>(
                    node.Children.OrderBy(c => c.Data.BOMCode));
            }

            return tree;
        }

        private IEnumerable<BOMNodeViewModel> FindAllNodes(BOMNodeViewModel node)
        {
            yield return node;
            foreach (var child in node.Children.SelectMany(FindAllNodes))
            {
                yield return child;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Method to clear cache for a specific BOM
        public void ClearCache(string bomCode, string bomVersion)
        {
            string dtoCacheKey = $"{BOM_DTO_CACHE_KEY_PREFIX}{bomCode}_{bomVersion}";
            string treeCacheKey = $"{BOM_TREE_CACHE_KEY_PREFIX}{bomCode}_{bomVersion}";
            _memoryCache.Remove(dtoCacheKey);
            _memoryCache.Remove(treeCacheKey);
        }
    }
}